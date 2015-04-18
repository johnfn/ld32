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
        SetColor();
    }

    void SetColor()
    {
        if (IsFirstScope)
        {
            _renderer.color = Color.blue;
            return;
        }

        _renderer.color = SuckModeOn ? Color.red : Color.white;
    }

    public void Init(bool suckMode, bool isFirstScope)
    {
        SuckModeOn = suckMode;
        IsFirstScope = isFirstScope;

        if (IsFirstScope)
        {
            transform.localScale = new Vector3(2f, 2f, 1f);
        }

        SetColor();
    }

    public void Update()
    {
        if (IsFirstScope)
        {
            transform.Rotate(Vector3.forward * 2.0f);

            SetColor();
        }
    }
}
