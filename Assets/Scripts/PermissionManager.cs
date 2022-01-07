﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public static class PermissionManager
{

    public static UnityAction RequestCameraPermissionAction => RequestCameraAccess;
    public static bool HaveCameraPermission
    {
        get => HasUserAuthorizedCamera();
    }

    private static bool HasUserAuthorizedCamera()
    {
        return Permission.HasUserAuthorizedPermission(Permission.Camera);
    }

    private static void RequestCameraAccess()
    {
        Debug.Log("Requesting Permission!");
        //Already Authorized.
        if (!HaveCameraPermission)
            Permission.RequestUserPermission(Permission.Camera);
    }

}
