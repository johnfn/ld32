using System.Collections;
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
                    new List<string>
                    {
                        "Professor",
                        "If my observations on this ham radio are correct, you just tried to shoot the gun with only 1 battery left!"
                    },
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

    public static List<List<string>> YoureDumb = new List<List<string>>
    {
        new List<string> {"Professor", "Alright, listen up, because I'm only gonna say this once." },
        new List<string> {"Professor", "I heard that players like being treated like utter idiots, so I'm going to give you some basic information about how to play this game." },
        new List<string> {"You", "..." },
        new List<string> {"You", "WHO ARE YOU?" },
        new List<string> {"You", "HOW DID YOU GET INSIDE MY MIND?!?" },
        new List<string> {"Professor", "..." },
        new List<string> {"Professor", "Wow, it's even worse than I thought."},
        new List<string> {"You", "AH!" },
        new List<string> {"You", "..." },
        new List<string> {"Professor", "..." },
        new List<string> {"You", "AH! AH! AH! AH! AH! AH! AH! AH!"},
        new List<string> {"You", "..."},
        new List<string> {"You", "*passes out*"},
        new List<string> {"Professor", "..."},
        new List<string> {"Professor", "Finally, some quiet."},
        new List<string> {"Professor", "Anyway, I do believe I was talking about giving you a basic run down on how to play this game."},
        new List<string> {"Professor", "So, you see that blue stuff above this dialog? That is the sky."},
        new List<string> {"Professor", "This stuff you're reading right now? This is dialog text. Just how dumb are you not to know that. Seriously, I'm constantly amazed..."},
        new List<string> {"Professor", "Do you see that blue stuff BELOW this dialog? That is water."},
        new List<string> {"Professor", "Yes, I know it's pretty tricky. How do you tell one blue from the other?? It baffles me every day."},
        new List<string> {"Professor", "(It doesn't really.)"},
        new List<string> {"Professor", "Alright, that should be enough."},
        new List<string> {"Professor", "If you still can't figure out how to play the game, I can't say that I'll be concerned."},
    };

    public static List<List<string>> GiveGun = new List<List<string>>
    {
        new List<string> {"Professor", "Hello there."},
        new List<string> {"You", "Um... Hi."},
        new List<string> {"You", "Who are you?"},
        new List<string> {"Professor", "Huh? Why do you want to know that?"},
        new List<string>
        {
            "You",
            "Uh.. I was only asking to be courteous. I can clearly read under your picture on the dialog screen that you are the Professor."
        },
        new List<string> {"Professor", "...What?"},
        new List<string> {"You", "Eh, don't worry about it."},
        new List<string> {"Professor", "..."},
        new List<string> {"You", "..."},
        new List<string> {"You", "So what's up?"},
        new List<string> {"Professor", "I've recently developed something amazing!"},
        new List<string> {"Professor", "A gun SO UNCONVENTIONAL that I don't even know what it does!"},
        new List<string> {"You", "Er... Ok."},
        new List<string> {"Professor", "Anyway, it's battery powered, and it takes a whole battery to fire."},
        new List<string> {"Professor", "You can see how many batteries you have in the top right of your screen."},
        new List<string> {"You", "I have no clue what you're talking about right now."},
        new List<string> {"You", "But I only have one battery in my chasis."},
        new List<string> {"Professor", "Aw, crap. I'll give it to you now. In the mean time, go find a battery."},
        new List<string> {"You", "Ok. Thanks! I guess."},
    };

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

        StartCoroutine(WaitASecAndThenGo());

    }

    private IEnumerator WaitASecAndThenGo()
    {
        yield return null;

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
