using UnityEngine.SceneManagement;

public class CustomSceneManager
{
    public static string ScanSceneName => scanScene;
    public static string GenerateSceneName => generateScene;
    public static string MainMenuSceneName => mainMenuScene;

    public static string CurrentSceneName => GetCurrentSceneName();

    private const string mainMenuScene = "MainMenu";
    private const string scanScene = "QRScan";
    private const string generateScene = "QRGenerate";


    private static string GetCurrentSceneName()
    {
       return SceneManager.GetActiveScene().name;
    }

    public static void LoadScanScene()
    {
        SceneManager.LoadScene(scanScene);
    }

    public static void LoadGenerateScene()
    {
        SceneManager.LoadScene(generateScene);

    }

    public static void LoadMainMenuScene()
    {
        SceneManager.LoadScene(mainMenuScene);

    }


}
