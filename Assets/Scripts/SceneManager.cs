using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    private const string nextScene = "UnityDemo";

    public void RequestCamera()
    {
        PermissionManager.RequestCameraPermissionAction.Invoke();
    }

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }


}
