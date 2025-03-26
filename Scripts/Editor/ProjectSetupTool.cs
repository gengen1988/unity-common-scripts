using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public static class ProjectSetupTool
{
    private const string PKG_SOFTMASK = "https://github.com/mob-sakai/SoftMaskForUGUI.git?path=Packages/src";
    private const string PKG_UNITASK = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";
    private const string PKG_NUGET = "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity";
    private const string PKG_R3UNITY = "https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity";
    private const string PKG_ZLINQUNITY = "https://github.com/Cysharp/ZLinq.git?path=src/ZLinq.Unity/Assets/ZLinq.Unity";

    private const string ASSET_DOTWEEN_PRO = "Demigiant/Editor ExtensionsVisual Scripting/DOTween Pro.unitypackage";



    [MenuItem("Tools / Project Setup / Remove Unnecessary Packages")]
    private static void RemoveUnnecessaryPackages()
    {
        Client.Remove("com.unity.collab-proxy");
    }

    [MenuItem("Tools / Project Setup / Import DOTween Pro")]
    private static void ImportDOTweenPro()
    {
        AssetStoreUtil.ImportAssetPackage(ASSET_DOTWEEN_PRO);
    }

    [MenuItem("Tools / Project Setup / Install SoftMask")]
    private static void InstallSoftMask()
    {
        PackageManagerUtil.InstallPackages(PKG_SOFTMASK);
    }

    [MenuItem("Tools / Project Setup / Install NuGet")]
    private static void InstallNuGet()
    {
        PackageManagerUtil.InstallPackages(PKG_NUGET);
    }

    [MenuItem("Tools / Project Setup / Install UniTask")]
    private static void InstallUniTask()
    {
        PackageManagerUtil.InstallPackages(PKG_UNITASK);
    }

    [MenuItem("Tools / Project Setup / Install R3.Unity (install core pacakge in nuget first)")]
    private static void InstallR3Unity()
    {
        PackageManagerUtil.InstallPackages(PKG_R3UNITY);
    }

    [MenuItem("Tools / Project Setup / Install ZLinq.Unity (install core pacakge in nuget first)")]
    private static void InstallZLinqUnity()
    {
        PackageManagerUtil.InstallPackages(PKG_ZLINQUNITY);
    }
}

public static class AssetStoreUtil
{
    private const string UNITY_DOWNLOADED_ASSETS_PATH = "Unity/Asset Store-5.x";

    public static void ImportAssetPackage(string assetPath)
    {
        var baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var assetsFolder = Path.Combine(baseFolder, UNITY_DOWNLOADED_ASSETS_PATH);
        var packagePath = Path.Combine(assetsFolder, assetPath);

        if (File.Exists(packagePath))
        {
            AssetDatabase.ImportPackage(packagePath, false);
        }
        else
        {
            Debug.LogWarning($"Asset not found at path: {packagePath}");
        }
    }
}

public static class PackageManagerUtil
{
    private static AddRequest currentRequest;
    private static readonly Queue<string> installQueue = new();

    public static void InstallPackages(params string[] packages)
    {
        foreach (var pkg in packages)
        {
            installQueue.Enqueue(pkg);
        }

        if (installQueue.Count > 0)
        {
            StartNextInstallation();
        }
    }

    private static async void StartNextInstallation()
    {
        currentRequest = Client.Add(installQueue.Dequeue());
        while (!currentRequest.IsCompleted)
        {
            await Task.Delay(10);
        }

        if (currentRequest.Status == StatusCode.Success)
        {
            Debug.Log($"Successfully installed package: {currentRequest.Result.packageId}");
        }
        else if (currentRequest.Status >= StatusCode.Failure)
        {
            Debug.LogError($"Failed to install package: {currentRequest.Error.message}");
        }

        if (installQueue.Count > 0)
        {
            await Task.Delay(1000);
            StartNextInstallation();
        }
    }
}