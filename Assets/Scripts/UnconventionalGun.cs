using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using JetBrains.Annotations;

public class UnconventionalGun : MonoBehaviour
{
    public bool HasGun;

    public LayerMask WallMask;

    public float distanceBetweenScopeIndicators;

    private List<GameObject> _scopePool = new List<GameObject>();

    [UsedImplicitly]
    public void Start()
    {
        for (var i = 0; i < 60; i++)
        {
            Debug.Log(i);

            _scopePool.Add(Manager.CreateScope());
        }
    }

    [UsedImplicitly]
    public void Update()
    {
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

        for (var i = 0; i < direction.magnitude / distanceBetweenScopeIndicators; i++)
        {
            _scopePool[i].transform.position = start + normalizedDirection * distanceBetweenScopeIndicators * (i + 1);
        }
    }
}
