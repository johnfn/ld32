using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class ScopeController : MonoBehaviour
{
    public bool SuckModeOn = false;

    public bool IsFirstScope = false;

    private SpriteRenderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();

        _renderer.color = Color.red;
    }

    void Start()
    {
        _renderer.color = SuckModeOn ? Color.red : Color.white;
    }

    public void Init(bool suckMode, bool isFirstScope)
    {
        SuckModeOn = suckMode;
        IsFirstScope = isFirstScope;

        Start();
    }

    public void Update()
    {
        if (IsFirstScope)
        {
            Debug.Log("I'm the first scope!!!");
        }
    }
}
