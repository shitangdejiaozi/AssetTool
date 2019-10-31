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
    public static string AssetName = "Assets";
    public static string configPath = "Assets/buildConfig.asset";
    public static string BundleExt = ".ab";

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
        List<AssetBundleBuild> bundleList = new List<AssetBundleBuild>();

        var buildCfg = AssetDatabase.LoadAssetAtPath<BuildConfig>(configPath);
        List<string> paths = buildCfg.buildPaths;
        for (int i = 0; i < paths.Count; i++)
        {
            bundleList.AddRange(BuildListByPath(paths[i]));
        }

        bundleList = BuildListByDepends(bundleList, buildCfg.collectPaths); //收集依赖

        Debug.LogError("bundle list :" + bundleList.Count);

    }

    /// <summary>
    /// 根据目录下所有文件夹来构建AssetBuild列表
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static List<AssetBundleBuild> BuildListByPath(string path)
    {
        List<AssetBundleBuild> bundleList = new List<AssetBundleBuild>();
        string title = string.Format("收集目录{0}",path);
        EditorUtility.DisplayProgressBar(title, "正在收集...", 0f);
        string[] childDir = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        
        List<string> allDir = new List<string>();
        allDir.AddRange(childDir);
        allDir.Add(path);
        for (int i = 0; i < allDir.Count; i++)
        {
            allDir[i] = allDir[i].Replace("\\", "/");
           // Debug.LogError("child dir" + allDir[i]);
            List<string> assets = new List<string>();
            string[] files = Directory.GetFiles(allDir[i]); //按目录打包，一个目录下的文件打成一个ab
            for(int j = 0; j< files.Length; j++)
            {
                
                if (!CheckFileInvalid(files[j]))
                    continue;
                files[j] = files[j].Replace("\\", "/");
                string assetName = files[j];
                assets.Add(assetName);
               // Debug.LogError("file is:" + files[j]);

            }

            if(assets.Count > 0) //目录下有文件的才会打成ab
            {
                string abName = GetBundleName(allDir[i]);
                AssetBundleBuild abinfo = new AssetBundleBuild();
                abinfo.assetBundleName = abName + BundleExt;
                abinfo.assetNames = assets.ToArray();
                string baseName = Path.GetFileName(abinfo.assetBundleName);
                Debug.LogError("base name;" + baseName);
                bundleList.Add(abinfo);
            }

            EditorUtility.DisplayProgressBar(title, string.Format("正在收集...{0}/{1}", i, allDir.Count), i / allDir.Count);

        }
        EditorUtility.ClearProgressBar();

        return bundleList;
    }

    /// <summary>
    /// 根据目录下文件的依赖来构建AssetBuild列表
    /// </summary>
    /// <returns></returns>
    private static List<AssetBundleBuild> BuildListByDepends(List<AssetBundleBuild> abList, List<string> paths)
    { 
        List<AssetBundleBuild> bundleList = new List<AssetBundleBuild>();
        string title = "收集依赖";
        EditorUtility.DisplayProgressBar(title, "正在收集...", 0f);

        return bundleList;
    }

    /// <summary>
    /// 获取目录的bundle名字
    /// </summary>
    /// <param name="dirPath"></param>
    /// <returns></returns>
    private static string GetBundleName(string dirPath)
    {
        string bundleName = dirPath.Replace("\\", "/");
        return bundleName.ToLower();
    }

    /// <summary>
    /// 检查文件的有效性，判断是否要打包
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool CheckFileInvalid(string path)
    {
        if(path.EndsWith(".meta"))
        {
            return false;
        }

        return true;
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
