using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JetBrains.Annotations;

public class HudController : MonoBehaviour
{
    public GameObject BatteryContainer;

    private ObjectPool _batteryPool;

    [UsedImplicitly]
    public void Awake()
    {
        _batteryPool = new ObjectPool(() => Manager.CreateBattery().gameObject);
    }

    [UsedImplicitly]
    public void Start()
    {
        CanTakeInput.InputHolderChanged += OnChangeActiveGuy;
    }

    private void OnChangeActiveGuy()
    {
        var activeGuy = CanTakeInput.ActiveInputGuy;
        var energy = activeGuy.GetComponent<HasEnergy>();

        _batteryPool.KillAllObjects();

        for (var i = 0; i < energy.HalfBatteriesTotal / 2; i++)
        {
            var obj = _batteryPool.SpawnObject();
            var transform = obj.GetComponent<RectTransform>();
            var battery = obj.GetComponent<BatteryController>();

            transform.SetParent(BatteryContainer.GetComponent<RectTransform>(), false);
            transform.anchoredPosition = new Vector2(transform.rect.width * i, 0f);

            BatteryState state;

            if (i * 2 + 2 <= energy.HalfBatteriesLeft)
            {
                state = BatteryState.Full;
            }
            else if (i * 2 + 1 <= energy.HalfBatteriesLeft)
            {
                state = BatteryState.Half;
            }
            else
            {
                state = BatteryState.Empty;
            }

            battery.Init(state);
        }
    }

    [UsedImplicitly]
    public void Update()
    {
    }
}
