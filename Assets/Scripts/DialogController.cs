using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public class Dialogs
{
    public static List<List<string>> OnlyOneBattery
    {
        get
        {
            if (!_onlyOneBatterySeen)
            {
                _onlyOneBatterySeen = true;

                return new List<List<string>>
                {
                    new List<string> {"Professor", "You can't do that!"},
                    new List<string> {"You", "What?"},
                    new List<string> { "Professor", "If my observations on this ham radio are correct, you just tried to shoot the gun with only 1 battery left!" },
                    new List<string> {"You", "So?"},
                    new List<string> {"Professor", "You would die! It's too dangerous. I can't allow you to do it."},
                    new List<string> {"You", "Fine, whatever."},
                    new List<string> {"Professor", "..."},
                    new List<string> {"You", "A ham radio, seriously?"},
                    new List<string> {"Professor", "Shush, you. Go back to your puzzle solving or whatever."}
                };
            }

            return new List<List<string>>
            {
                new List<string> {"You", "I only have one battery. The professor would get mad."},
            };
        }
    }

    private static bool _onlyOneBatterySeen = false;
}

public class DialogController : MonoBehaviour
{
    public Image DialogImage;

    public Text DialogText;

    public Text SpeakerText;

    public Text ClickToContinue;

    private List<List<string>> _dialogContent;

    private int _dialogPosition;

    private string _visibleText;

    private List<MonoBehaviour> _disabledObjects;

    private bool _showingDialog = false;

    private int _ticks;

    private string CurrentFullText
    {
        get { return _dialogContent[_dialogPosition][1]; }
    }

    private string CurrentSpeaker
    {
        get { return _dialogContent[_dialogPosition][0]; }
    }

    [UsedImplicitly]
    void Awake()
    {
        DialogImage.gameObject.SetActive(false);
    }

    public void ShowDialog(List<List<string>> dialog)
    {
        if (_showingDialog)
        {
            Debug.LogError("I'm already showing a dialog, stupid.");
            return;
        }

        _dialogPosition = 0;
        _visibleText = "";
        _ticks = 0;

        _dialogContent = dialog;

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

        SpeakerText.text = CurrentSpeaker;

        if (_visibleText == CurrentFullText)
        {
            ClickToContinue.text = "Click to continue";

            if (Input.GetMouseButtonUp(0))
            {
                _ticks = 0;
                _visibleText = "";
                DialogText.text = "";

                if (_dialogPosition + 1 >= _dialogContent.Count)
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
            ClickToContinue.text = "Click to speed up";

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
