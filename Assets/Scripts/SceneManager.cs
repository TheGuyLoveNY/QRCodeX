using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{

    public Text messageText;

    public void Start()
    {
        messageText.gameObject.SetActive(false);
    }


    public void ShowMessage()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "Hello World!";
    }
}
