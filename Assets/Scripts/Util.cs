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

    public static void ToggleAnything(GameObject obj, bool visible)
    {
        obj.SetActive(visible);
        /*
        var sr = obj.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.enabled = visible;
            return;
        }
        */
    }

    public static float DistanceToLine(Ray ray, Vector3 point)
    {
        return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
    }
}