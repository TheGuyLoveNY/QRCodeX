using UnityEngine;

[System.Serializable]
public struct CameraProperties
{
    /// <summary>
    /// Uses device's native resolution!
    /// </summary>
    [Tooltip("Uses device's native resolution")]
    public bool useDefaultSettings;

    public int requestedHeight;
    public int requestedWidth;
    public int requestedFPS;
}
