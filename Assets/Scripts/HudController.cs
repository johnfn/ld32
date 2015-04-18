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
            var battery = _batteryPool.SpawnObject();
            var transform = battery.GetComponent<RectTransform>();

            transform.SetParent(BatteryContainer.GetComponent<RectTransform>(), false);
            transform.anchoredPosition = new Vector2(transform.rect.width * i, 0f);
        }
    }

    [UsedImplicitly]
    public void Update()
    {
    }
}
