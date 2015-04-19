using UnityEngine;
using System.Collections;

public class Parallaxer : MonoBehaviour
{
    public GameObject ParallaxTarget;

    public Vector3 ParallaxFactor;

    private Vector3 _initialTargetPosition;

    private Vector3 _initialParallaxScrollerPosition;

    public void Start()
    {
        _initialTargetPosition = ParallaxTarget.transform.position;
        _initialParallaxScrollerPosition = gameObject.transform.position;
    }

    public void LateUpdate()
    {
        var targetDiff = ParallaxTarget.transform.position - _initialTargetPosition;

        gameObject.transform.position = _initialParallaxScrollerPosition + Vector3.Scale(targetDiff, ParallaxFactor);
    }
}
