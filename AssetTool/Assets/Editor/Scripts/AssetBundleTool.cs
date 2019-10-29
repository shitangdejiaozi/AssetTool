using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class AssetBundleTool
{
    public static string AssetPath = Application.dataPath;
    public static string RootPath = AssetPath.Substring(0, AssetPath.LastIndexOf("/"));
    public static string BundlePath = "AssetBundles";
    public static string BundleRealRootPath = string.Format("{0}/{1}", RootPath, BundlePath);
    public static string BundleRealPath = string.Empty;

    [MenuItem("AssetBundle/选择当前路径到copybuffer")]
    public static void CopySelectPath()
    {
        var guids = Selection.assetGUIDs;
        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        EditorGUIUtility.systemCopyBuffer = assetPath;
        Debug.LogError("path" + assetPath);
    }

    

    [MenuItem("AssetBundle/设置选中的路径为打包目录")]
    public static void SetBuildPathToConfig()
    {
        string configPath = "Assets/buildConfig.asset";

        List<string> pathList = new List<string>();
        var guids = Selection.assetGUIDs;
        for(int i = 0; i< guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            pathList.Add(assetPath);
        }
        var buildCfg = AssetDatabase.LoadAssetAtPath<BuildConfig>(configPath);
        buildCfg.buildPaths = pathList;
        AssetDatabase.Refresh();
    }

    [MenuItem("AssetBundle/设置选中的路径为收集依赖目录")]
    public static void SetCollectPathToConfig()
    {
        string configPath = "Assets/buildConfig.asset";

        List<string> pathList = new List<string>();
        var guids = Selection.assetGUIDs;
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            pathList.Add(assetPath);
        }
        var buildCfg = AssetDatabase.LoadAssetAtPath<BuildConfig>(configPath);
        buildCfg.collectPaths = pathList;
        AssetDatabase.Refresh();
    }

    [MenuItem("AssetBundle/构建AssetBundle")]
    public static void BuildAssetBundle()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        string platformName = GetPlatformName(buildTarget);
        BundleRealPath = BundleRealRootPath + "/" + platformName; //ab保存的完整目录
        BuildAssetBundleSavePath(true);



    }

    /// <summary>
    /// 构建ab的保存目录
    /// </summary>
    /// <param name="isRebuild"></param>
    static void BuildAssetBundleSavePath(bool isRebuild)
    {
        if (!Directory.Exists(BundleRealRootPath))
        {
            Directory.CreateDirectory(BundleRealRootPath);
        }

        if(isRebuild)
        {
            if (Directory.Exists(BundleRealPath))
                Directory.Delete(BundleRealPath);
        }
        if (!Directory.Exists(BundleRealPath))
        {
            Directory.CreateDirectory(BundleRealPath);
        }
    }

    /// <summary>
    /// 获取构建的平台名字
    /// </summary>
    /// <param name="buildtarget"></param>
    /// <returns></returns>
    static string GetPlatformName(BuildTarget buildtarget)
    {
        switch(buildtarget)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.StandaloneWindows64:
                return "Window";
            case BuildTarget.iOS:
                return "IOS";
        }
        return string.Empty;
    }
}
