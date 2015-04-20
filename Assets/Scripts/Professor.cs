using UnityEngine;
using System.Collections.Generic;

public class Professor : MonoBehaviour
{
    public static List<Professor> Professors = new List<Professor>();

    public bool HasTalked = false;

    public void Awake()
    {
        Professors.Add(this);
    }
}
