using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class UnconventionalGun : MonoBehaviour
{
    public bool HasGun;

    public GameObject Scope;

    public LayerMask WallMask;

    [UsedImplicitly]
    public void Start()
    {
        Scope = Manager.CreateScope();
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

            Debug.Log(hit.collider.gameObject.name);

            end = hit.point;
        }

        Debug.DrawRay(start, end - start);
    }
}
