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

    private string _shownText;

    private List<MonoBehaviour> _disabledObjects;

    private bool _showingDialog = false;

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
        _shownText = "";

        TurnOffLiterallyEverythingInTheWorld();

        DialogImage.gameObject.SetActive(true);
    }

    [UsedImplicitly]
    void Update()
    {
        if (!DialogImage.IsActive()) return;

        Debug.Log("Yer dialoggin now.");
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
    }
}
