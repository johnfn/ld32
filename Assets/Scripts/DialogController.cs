using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JetBrains.Annotations;

public class DialogController : MonoBehaviour
{
    public Image DialogImage;

    public Text DialogText;

    private GameObject[] _disabledObjects;

    [UsedImplicitly]
    void Awake()
    {
        DialogImage.gameObject.SetActive(false);
    }

    [UsedImplicitly]
    public void ShowDialog()
    {
        
    }
}
