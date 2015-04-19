using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JetBrains.Annotations;

public class FollowText : MonoBehaviour
{
    public Text FollowingText;

    private bool _visible = false;

    private string _fullText;

    private string _visibleText;

    private int _ticks = 0;

    public void SayText(string text)
    {
        FollowingText.gameObject.SetActive(true);

        _ticks = 0;
        _fullText = text;
        _visibleText = "";
        FollowingText.text = "";
    }

    [UsedImplicitly]
    void Awake()
    {
        FollowingText.gameObject.SetActive(false);
    }

    [UsedImplicitly]
    void Update()
    {
        if (!FollowingText.IsActive())
        {
            return;
        }

        _ticks++;

        if (_visibleText == _fullText)
        {
            if (_ticks > 100)
            {
                FollowingText.gameObject.SetActive(false);
            }

            return;
        }

        if (_ticks > 4)
        {
            _visibleText += _fullText[_visibleText.Length];
            FollowingText.text = _visibleText;
            _ticks = 0;
        }
    }
}
