﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using JetBrains.Annotations;

[DisallowMultipleComponent]
public class UnconventionalGun : MonoBehaviour
{
    public bool HasGun;

    public LayerMask WallMask;

    public float SuckPower;

    public float FirePower;

    public float DistanceBetweenScopeIndicators;

    private Ray _shotPath;

    private CanTakeInput _canTakeInput;

    private ObjectPool _scopePool;

    private ScopeController _finalScope;

    private HasEnergy _energy;

    private Character _character;

    private bool _isSucking = false;

    private static bool _hasEverShot = false;

    [UsedImplicitly]
    public void Start()
    {
        _scopePool = new ObjectPool(() => Manager.CreateScope(false, false).gameObject);
        _canTakeInput = GetComponent<CanTakeInput>();
        _finalScope = Manager.CreateScope(false, true);
        _energy = GetComponent<HasEnergy>();
        _character = GetComponent<Character>();

        _canTakeInput.SwitchedOff += InputTurnedOff;

        if (Manager.Instance.Debug)
        {
            _hasEverShot = true;
        }
    }

    private void InputTurnedOff()
    {
        _scopePool.KillAllObjects();

        _isSucking = false;
    }

    [UsedImplicitly]
    public void Update()
    {
        if (!_canTakeInput.ActivelyTakingInput) return;

        DrawScopes();

        if (Input.GetMouseButtonUp(0))
        {
            if (!Character.ProfActivatedGun)
            {
                Manager.Instance.Dialog.ShowDialog(Dialogs.ShouldTalkToProf);
            }
            else
            {
                if (_energy.HalfBatteriesLeft > 2)
                {
                    _energy.AddEnergy(-2);
                    _energy.AddTotalEnergy(-2);

                    ShootCopy();
                }
                else if (_energy.HalfBatteriesLeft == 2)
                {
                    Debug.Log("Special case: Time to die");

                    Manager.Instance.Dialog.ShowDialog(Dialogs.OnlyOneBattery);
                }
                else
                {
                    // Manager.Instance.Dialog.ShowDialog();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            _isSucking = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _isSucking = false;
        }

        if (_isSucking)
        {
            Suck();
        }
    }

    private void ShootCopy()
    {
        var copy = Manager.CreateCharacter();
        var hisPhysics = copy.GetComponent<PhysicsController2D>();
        var start = transform.position;
        var direction = Util.MousePosition() - start;

        copy.transform.position = start + direction.normalized * 0.5f;

        hisPhysics.AddHorizontalForce(direction.normalized.x / 10f);
        hisPhysics.AddVerticalForce(direction.normalized.y / 10f);

        if (!_hasEverShot)
        {
            _hasEverShot = true;
            StartCoroutine(WaitAndThenTalk());
        }
    }

    private IEnumerator WaitAndThenTalk()
    {
        yield return new WaitForSeconds(2.0f);

        Manager.Instance.Dialog.ShowDialog(Dialogs.WhatOnEarth);
    }

    private void Suck()
    {
        var allTargets = CanTakeInput.InputTargets
            .Where(it => it != _canTakeInput)
            .Select(it => it.gameObject);

        foreach (var target in allTargets)
        {
            var distance = Util.DistanceToLine(_shotPath, target.transform.position);

            if (distance > .6) continue;

            var character = target.GetComponent<Character>();
            var physics = target.GetComponent<PhysicsController2D>();
            var collisionPoint = _shotPath.origin +
                                 _shotPath.direction * Vector3.Dot(_shotPath.direction, target.transform.position - _shotPath.origin) / 2;

            var dir = collisionPoint - target.transform.position;
            dir.Normalize();

            physics.AddVerticalForce(dir.y * Time.deltaTime);
            physics.AddHorizontalForce(dir.x * Time.deltaTime);

            character.SaySuckFlavorText();
        }
    }

    private void DrawScopes()
    {
        var start = transform.position;
        var end = Util.MousePosition();
        var raycastHits = Physics2D.RaycastAll(start, end - start);

        _scopePool.KillAllObjects();

        foreach (var hit in raycastHits)
        {
            if (hit.collider.gameObject == gameObject) continue;

            if ((start - end).magnitude > (new Vector2(start.x, start.y) - hit.point).magnitude)
            {
                end = hit.point;
            }
        }

        var direction = end - start;
        var normalizedDirection = direction.normalized;

        for (var i = 0; i < direction.magnitude / DistanceBetweenScopeIndicators - 1; i++)
        {
            var newScope = _scopePool.SpawnObject();
            var scopeController = newScope.GetComponent<ScopeController>();

            scopeController.Init(_isSucking, false);

            newScope.transform.position = start + normalizedDirection * DistanceBetweenScopeIndicators * (i + 1);
        }

        // Add final scope

        _finalScope.SuckModeOn = _isSucking;
        _finalScope.transform.position = end;

        _shotPath = new Ray(start, direction);
    }
}
