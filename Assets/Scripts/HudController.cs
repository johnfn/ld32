using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JetBrains.Annotations;

public class HudController : MonoBehaviour
{
    public Text EnergyText;

    [UsedImplicitly]
    public void Update()
    {
        var activeGuy = CanTakeInput.ActiveInputGuy;
        var energy = activeGuy.GetComponent<HasEnergy>();

        EnergyText.text = string.Format("{0} of {1} health", energy.HalfBatteriesLeft, energy.HalfBatteriesTotal);
    }
}
