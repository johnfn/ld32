using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using JetBrains.Annotations;

[DisallowMultipleComponent]
public class UnconventionalGun : MonoBehaviour
{
    public bool HasGun;

    public LayerMask WallMask;

    public float distanceBetweenScopeIndicators;

    private CanTakeInput _canTakeInput;

    private ObjectPool _scopePool;

    private bool _isSucking = false;

    [UsedImplicitly]
    public void Start()
    {
        _scopePool = new ObjectPool(() => Manager.CreateScope(false, false).gameObject);
        _canTakeInput = GetComponent<CanTakeInput>();

        _canTakeInput.SwitchedOff += InputTurnedOff;
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

        if (Input.GetMouseButtonDown(0))
        {
            ShootCopy();
        }

        if (Input.GetMouseButtonDown(1))
        {
            _isSucking = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _isSucking = false;
        }
    }

    private void ShootCopy()
    {
        Debug.Log("TODO: I'm shooting a copy!!!1");
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

        for (var i = 0; i < direction.magnitude / distanceBetweenScopeIndicators - 1; i++)
        {
            var newScope = _scopePool.SpawnObject();
            var scopeController = newScope.GetComponent<ScopeController>();

            scopeController.Init(_isSucking, false);

            newScope.transform.position = start + normalizedDirection * distanceBetweenScopeIndicators * (i + 1);
        }
    }
}
