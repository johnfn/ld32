using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using JetBrains.Annotations;

public class UnconventionalGun : MonoBehaviour
{
    public bool HasGun;

    public LayerMask WallMask;

    public float distanceBetweenScopeIndicators;

    private CanTakeInput _canTakeInput;

    private ObjectPool _scopePool;

    [UsedImplicitly]
    public void Start()
    {
        _scopePool = new ObjectPool(Manager.CreateScope);
        _canTakeInput = GetComponent<CanTakeInput>();

        _canTakeInput.SwitchedOff += CleanUpScopes;
    }

    private void CleanUpScopes()
    {
        _scopePool.KillAllObjects();
    }

    [UsedImplicitly]
    public void Update()
    {
        if (!_canTakeInput.ActivelyTakingInput) return;

        DrawScopes();

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Ding");
        }
    }

    private void DrawScopes()
    {
        var start = transform.position;
        var end = Util.MousePosition();
        var raycastHits = Physics2D.RaycastAll(start, end - start);

        CleanUpScopes();

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
            _scopePool.SpawnObject().transform.position = start + normalizedDirection * distanceBetweenScopeIndicators * (i + 1);
        }
    }
}
