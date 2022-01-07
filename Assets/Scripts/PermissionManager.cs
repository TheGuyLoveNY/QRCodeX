using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public class PermissionManager
{

    public UnityAction RequestCameraPermissionAction => RequestCameraAccess;
    public bool HaveCameraPermission
    {
        get => HasUserAuthorizedCamera();
    }

    private bool HasUserAuthorizedCamera()
    {
        return Permission.HasUserAuthorizedPermission(Permission.Camera);
    }

    private void RequestCameraAccess()
    {
        //Already Authorized.
        if (!HaveCameraPermission)
            Permission.RequestUserPermission(Permission.Camera);
    }

}
