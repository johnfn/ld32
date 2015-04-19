using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class HasEnergy : MonoBehaviour
{
    public int HalfBatteriesLeft;

    public int HalfBatteriesTotal;

    public static event GenericEvent EnergyChange;

    public event GenericEvent Dead;

    public void AddEnergy(int halfBatteries)
    {
        HalfBatteriesLeft += halfBatteries;

        if (HalfBatteriesLeft > HalfBatteriesTotal)
        {
            HalfBatteriesLeft = HalfBatteriesTotal;
        }

        if (HalfBatteriesLeft < 0)
        {
            HalfBatteriesLeft = 0;

            if (Dead != null)
            {
                Dead();
            }
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
