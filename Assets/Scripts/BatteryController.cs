using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum BatteryState
{
    Full,
    Half,
    Empty
}

public class BatteryController : MonoBehaviour
{
    public List<Sprite> BatteryStates;

    public Image BatteryImage;

    private BatteryState _state;

    private BatteryState State
    {
        get { return _state; }
        set
        {
            UpdateGraphic(value);
            _state = value;
        }
    }

    public void Init(BatteryState state)
    {
        State = state;
    }

    private void UpdateGraphic(BatteryState state)
    {
        switch (state)
        {
            case BatteryState.Full:
                BatteryImage.sprite = BatteryStates[0];
                break;
            case BatteryState.Half:
                BatteryImage.sprite = BatteryStates[1];
                break;
            case BatteryState.Empty:
                BatteryImage.sprite = BatteryStates[2];
                break;
        }
    }
}
