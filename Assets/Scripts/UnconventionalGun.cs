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
    }

    [UsedImplicitly]
    public void Update()
    {
        if (!_canTakeInput.ActivelyTakingInput) return;

        var start = transform.position;
        var end = Util.MousePosition();
        var raycastHits = Physics2D.RaycastAll(start, end - start);

        foreach (var hit in raycastHits)
        {
            if (hit.collider.gameObject == gameObject) continue;

            end = hit.point;
        }

        var direction = end - start;
        var normalizedDirection = direction.normalized;

        _scopePool.KillAllObjects();

        for (var i = 0; i < direction.magnitude / distanceBetweenScopeIndicators; i++)
        {
            _scopePool.SpawnObject().transform.position = start + normalizedDirection * distanceBetweenScopeIndicators * (i + 1);
        }
    }
}
