

[System.Serializable]
public struct CameraProperties
{
    public int requestedHeight;
    public int requestedWidth;
    public int requestedFPS;

    /// <summary>
    /// Uses Screen's Width/Height
    /// </summary>
    public bool useDefaultSettings;
}
