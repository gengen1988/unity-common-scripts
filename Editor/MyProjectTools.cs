using TMPro;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public static class MyProjectTools
{
    static AddRequest request;

    [MenuItem("Tools / My Project / Init HDR Project")]
    static void InitHDRProject()
    {
        // tmp
        TMP_PackageUtilities.ImportProjectResourcesMenu();

        // linear for hdr
        PlayerSettings.colorSpace = ColorSpace.Linear;

        // disable webgl compression - let nginx handle compression
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

        // disable auto graphic api for linear color in webgl
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, new[] { GraphicsDeviceType.OpenGLES3 });

        // enable hdr for glow
        TierSettings tierSettings;

        // standalone
        tierSettings = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3);
        tierSettings.hdr = true;
        EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3, tierSettings);

        // webgl
        tierSettings = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.WebGL, GraphicsTier.Tier3);
        tierSettings.hdr = true;
        EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.WebGL, GraphicsTier.Tier3, tierSettings);

        // remove bloats
        Client.Remove("com.unity.collab-proxy");

        // add post processing package
        request = Client.Add("com.unity.postprocessing");
        EditorApplication.update += Progress;
    }

    static void Progress()
    {
        if (request.IsCompleted)
        {
            switch (request.Status)
            {
                case StatusCode.Success:
                    Debug.Log("add post processing package success");
                    break;
                case StatusCode.Failure:
                    Debug.LogError($"add post processing package failure: {request.Error.message}");
                    break;
            }
            EditorApplication.update -= Progress;
        }
    }
}
