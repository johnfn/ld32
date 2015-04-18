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
        EnergyText.text = "Woo";
    }
}
