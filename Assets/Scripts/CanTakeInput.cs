using UnityEngine;
using System.Collections;

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

    public static CanTakeInput ActiveInputGuy;

    public void Update()
    {
        if (ActivelyTakingInput)
        {
            Debug.Log(gameObject.name);
        }
    }
}
