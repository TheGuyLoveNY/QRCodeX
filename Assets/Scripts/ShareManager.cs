public class ShareManager
{
    private static NativeShare _ns;

    public static UnityEngine.Events.UnityAction PostShareAction;

    public static void ShareFile(string filePath, string customMessage = "")
    {
        if(_ns == null)
            _ns = new NativeShare();

        _ns.AddFile(filePath);
        _ns.SetText(customMessage);
        _ns.SetCallback(Callback);
        _ns.Share();
    }

    private static void Callback(NativeShare.ShareResult result, string shareTarget)
    {
        PostShareAction?.Invoke();
    }
}
