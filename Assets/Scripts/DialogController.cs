using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public class DialogController : MonoBehaviour
{
    public Image DialogImage;

    public Text DialogText;

    public Text ClickToContinue;

    private List<List<string>> DialogContent = new List<List<string>>
    {
        new List<string> { "You", "Lol, cool." },
        new List<string> { "You", "Aiite, I'm out. ayyyyy" },
    };

    private int _dialogPosition;

    private string _visibleText;

    private List<MonoBehaviour> _disabledObjects;

    private bool _showingDialog = false;

    private int _ticks;

    private string CurrentFullText
    {
        get { return DialogContent[_dialogPosition][1]; }
    }

    private string CurrentSpeaker
    {
        get { return DialogContent[_dialogPosition][1]; }
    }

    [UsedImplicitly]
    void Awake()
    {
        DialogImage.gameObject.SetActive(false);
    }

    public void ShowDialog()
    {
        if (_showingDialog)
        {
            Debug.LogError("I'm already showing a dialog, stupid.");
            return;
        }

        _dialogPosition = 0;
        _visibleText = "";
        _ticks = 0;

        TurnOffLiterallyEverythingInTheWorld();

        DialogImage.gameObject.SetActive(true);
    }

    [UsedImplicitly]
    void Update()
    {
        if (!DialogImage.IsActive())
        {
            return;
        }

        _ticks++;

        if (_visibleText == CurrentFullText)
        {
            if (Input.GetMouseButtonUp(0))
            {
                _ticks = 0;
                _visibleText = "";
                DialogText.text = "";

                if (_dialogPosition + 1 >= DialogContent.Count)
                {
                    CloseDialog();

                    return;
                }

                _dialogPosition += 1;
            }

            return;
        }

        if (_ticks > 2 || Input.GetMouseButton(0))
        {
            _visibleText += CurrentFullText[_visibleText.Length];
            DialogText.text = _visibleText;

            _ticks = 0;
        }
    }

    private void CloseDialog()
    {
        _dialogPosition = 0;

        TurnOnLiterallyEverythingInTheWorld();
    }

    private void TurnOffLiterallyEverythingInTheWorld()
    {
        var allComponents = FindObjectsOfType<GameObject>()
            .Where(t => t != gameObject)
            .SelectMany(t => t.GetComponents<MonoBehaviour>())
            .ToList();

        foreach (var component in allComponents)
        {
            component.enabled = false;
        }

        _disabledObjects = allComponents;
    }

    private void TurnOnLiterallyEverythingInTheWorld()
    {
        foreach (var component in _disabledObjects)
        {
            component.enabled = true;
        }

        Awake();
    }
}
