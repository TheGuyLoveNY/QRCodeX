using UnityEngine;

public static class URLBrowser
{
    public static void OpenURL(string url)
    {
        if (string.IsNullOrEmpty(url))
            return;

        Application.OpenURL(url);
    }
}
