using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

[DisallowMultipleComponent]
public class CanTakeInput : MonoBehaviour
{
    public bool ActivelyTakingInput;

    [HideInInspector] public bool JustSwitched;

    public static List<CanTakeInput> InputTargets = new List<CanTakeInput>();

    public static CanTakeInput ActiveInputGuy
    {
        get { return InputTargets.Find(t => t.ActivelyTakingInput); }
    }

    public event GenericEvent SwitchedOn;

    public event GenericEvent SwitchedOff;

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
        if (ActivelyTakingInput && !JustSwitched)
        {
            var position = gameObject.transform.position;

            position.y += _collider.bounds.size.y / 2 + 0.1f;

            Manager.Instance.Indicator.transform.position = position;

            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                SwitchInput();
            }
        }

        JustSwitched = false;
    }

    private void SwitchInput()
    {
        var currentIndex = InputTargets.IndexOf(this);
        var nextIndex = (currentIndex + 1) % InputTargets.Count;

        var currentTarget = InputTargets[currentIndex];
        var nextTarget = InputTargets[nextIndex];

        currentTarget.ActivelyTakingInput = false;
        currentTarget.JustSwitched = true;
        if (currentTarget.SwitchedOff != null) currentTarget.SwitchedOff();

        nextTarget.ActivelyTakingInput = true;
        nextTarget.JustSwitched = true;
        if (nextTarget.SwitchedOn != null) nextTarget.SwitchedOn();

        Manager.CustomCamera.Target = nextTarget.gameObject;
    }
}
