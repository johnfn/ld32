using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class CanTakeInput : MonoBehaviour
{
    public bool ActivelyTakingInput;

    public bool JustSwitched;

    public static List<CanTakeInput> InputTargets = new List<CanTakeInput>();

    public static CanTakeInput ActiveInputGuy
    {
        get { return InputTargets.Find(t => t.ActivelyTakingInput); }
    }

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

        InputTargets[currentIndex].ActivelyTakingInput = false;
        InputTargets[nextIndex].ActivelyTakingInput = true;
        InputTargets[nextIndex].JustSwitched = true;
    }
}
