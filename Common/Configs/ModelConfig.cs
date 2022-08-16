using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ModelConfig
{
    // size in AR mode / 3D mode
    // public static float scaleFactorInARMode = 0.15f;
    public static float scaleFactorInARMode = 0.15f;
    public static float scaleFactorForHightlightingSelectedObject = 1.5f;
    public static float durationForHightlightingSelectedObject = 0.12f;
    public static float rotationSpeed = 0.3f;
    // public static float rotationRate = 0.07f;
    public static float longTouchThreshold = 1f;
    public static Vector3 CENTER_POSITION = new Vector3(0f, 0f, 0f);
    public static string failedToDownloadModel = "Failed to download model";
    public static string downloadProcessType = "DOWNLOADING MODEL ...";
    public static string loadFromLocalProcessType = "LOADING MODEL ...";
    public static string wrapperGameObject = "WrapperGameObject";

}
