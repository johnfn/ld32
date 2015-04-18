using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class HasEnergy : MonoBehaviour
{
    public int HalfBatteriesLeft;

    public int HalfBatteriesTotal;

    public static event GenericEvent EnergyChange;

    public void AddEnergy(int halfBatteries)
    {
        HalfBatteriesLeft += halfBatteries;

        if (HalfBatteriesLeft > HalfBatteriesTotal)
        {
            HalfBatteriesLeft = HalfBatteriesTotal;
        }

        if (EnergyChange != null)
        {
            EnergyChange();
        }
    }

    public void AddTotalEnergy(int halfBatteries)
    {
        HalfBatteriesTotal += halfBatteries;

        if (EnergyChange != null)
        {
            EnergyChange();
        }
    }
}
