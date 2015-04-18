using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public delegate void GenericEvent();

class Util
{
    public static float MapWidth = 8.0f;
    public static float MapHeight = 8.0f;

    public static Vector3 MousePosition()
    {
        Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pz.z = 0;

        return pz;
    }
}