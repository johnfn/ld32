using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class CanTakeInput : MonoBehaviour
{
    /** Don't use. Just for inspector. */
    public bool _activelyTakingInput;

    public bool ActivelyTakingInput
    {
        get { return _activelyTakingInput; }
        set
        {
            if (value)
            {
                ActiveInputGuy.ActivelyTakingInput = false;
                ActiveInputGuy = this;
            }

            _activelyTakingInput = value;
        }
    }

    public static List<CanTakeInput> InputTargets = new List<CanTakeInput>();

    public static CanTakeInput ActiveInputGuy;

    private BoxCollider2D _collider;

    [UsedImplicitly]
    public void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();

        InputTargets.Add(this);
    }

    [UsedImplicitly]
    public void Update()
    {
        if (ActivelyTakingInput)
        {
            var position = gameObject.transform.position;

            position.y += _collider.bounds.size.y / 2 + 0.1f;

            Manager.Instance.Indicator.transform.position = position;
        }
    }
}
