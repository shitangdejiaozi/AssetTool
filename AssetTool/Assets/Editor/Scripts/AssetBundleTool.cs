using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Security.Cryptography;

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
    public static string ResourcesDirName = "Resources";
    public static string DependsDir = "depends/";
    public static string PlatformName = string.Empty;
    public static string BundleListName = "Bundle_list.data";

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
        EditorUtility.SetDirty(buildCfg);
        AssetDatabase.SaveAssets();
        
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
        EditorUtility.SetDirty(buildCfg);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("AssetBundle/构建AssetBundle-非增量")]
    public static void ReBuildAssetBundle()
    {
        BuildAssetBundle(true);
    }

    [MenuItem("AssetBundle/构建AssetBundle-增量")]
    public static void BuildAssetBundle()
    {
        BuildAssetBundle(false);
    }

    public static void SetBundelModeOpen(bool isBundle)
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
        if(buildTarget == BuildTarget.Android)
        {
            targetGroup = BuildTargetGroup.Android;
        }
        else if(buildTarget == BuildTarget.iOS)
        {
            targetGroup = BuildTargetGroup.iOS;
        }
        else if(buildTarget == BuildTarget.StandaloneWindows64)
        {
            targetGroup = BuildTargetGroup.Standalone;
        }
        string cursymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        if(isBundle)
        {
            cursymbol += ";USE_ASSETBUNDLE";
        }
        else
        {

        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, cursymbol);
        


    }
    
    private static bool BuildAssetBundle(bool isRebuild)
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        PlatformName = GetPlatformName(buildTarget);
        BundleRealPath = BundleRealRootPath + "/" + PlatformName; //ab保存的完整目录
        BuildAssetBundleSavePath(isRebuild);
        List<AssetBundleBuild> bundleList = new List<AssetBundleBuild>();

        var buildCfg = AssetDatabase.LoadAssetAtPath<BuildConfig>(configPath);
        List<string> paths = buildCfg.buildPaths;
        for (int i = 0; i < paths.Count; i++)
        {
            bundleList.AddRange(BuildListByPath(paths[i]));
        }

        bundleList = BuildListByDepends(bundleList, buildCfg.collectPaths); //收集依赖
        Debug.LogError("bundle list :" + bundleList.Count);

        if (bundleList.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "资源列表为空", "OK");
            return false;
        }

        EditorUtility.DisplayProgressBar("开始构建", "正在分析", 0.5f);

        BuildAssetBundleOptions option = BuildAssetBundleOptions.ChunkBasedCompression |
                                         BuildAssetBundleOptions.IgnoreTypeTreeChanges |
                                         BuildAssetBundleOptions.DeterministicAssetBundle;
        if(isRebuild)
        {
            option |= BuildAssetBundleOptions.ForceRebuildAssetBundle; //默认是增量打包的
        }
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(BundleRealPath, bundleList.ToArray(), option, buildTarget);
        EditorUtility.ClearProgressBar();
        if(manifest == null)
        {
            EditorUtility.DisplayDialog("提示", "生成bundle失败", "OK");
            return false;
        }

        CreateBundleList(manifest);
        return true;
    }

    /// <summary>
    /// 根据目录下所有文件夹来构建AssetBuild列表,包含所有要打AB的资源
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
    /// 根据目录下文件的依赖来构建AssetBuild列表,为了防止资源冗余
    /// </summary>
    /// <returns></returns>
    private static List<AssetBundleBuild> BuildListByDepends(List<AssetBundleBuild> abList, List<string> paths)
    { 
        List<AssetBundleBuild> bundleList = new List<AssetBundleBuild>();
        string title = "收集依赖";
        EditorUtility.DisplayProgressBar(title, "正在收集...", 0f);
        Dictionary<string, List<string>> abDict = new Dictionary<string, List<string>>(); //保存ab:AssetList
        Dictionary<string, bool> assetDict = new Dictionary<string, bool>(); //保存所有的Assetname

        for(int i = 0; i< abList.Count; i++)
        {
            List<string> assets = new List<string>(abList[i].assetNames);
            abDict[abList[i].assetBundleName] = assets;
            for(int j = 0; j< assets.Count; j++)
            {
                assetDict[assets[j]] = true;
            }
        }
        List<string> dependsAssets = new List<string>();
        for(int i =  0; i< paths.Count; i++)
        {
            string dirPath = paths[i];
            EditorUtility.DisplayProgressBar(title, string.Format("正在收集-{0}", dirPath), i * 1.0f / paths.Count);
            if(Directory.Exists(dirPath))
            {
                List<string> collectAssets = new List<string>();
                string[] assetsList = Directory.GetFiles(dirPath);
                for(int j = 0; j < assetsList.Length; j ++)
                {
                    if(assetsList[j].EndsWith(".prefab")) //主要针对prefab，可能存在相互的资源引用
                    {
                        string assetName = assetsList[i].Replace("\\", "/");
                        if(assetDict.ContainsKey(assetName)) //已经对prfab自身打过包了，现在再收集它的依赖
                        {
                            collectAssets.Add(assetName);
                            Debug.LogError("collect assets" + assetName);
                        }
                    }
                }
                

                if(collectAssets.Count > 0)
                {
                    string[] depends = AssetDatabase.GetDependencies(collectAssets.ToArray());
                    dependsAssets.AddRange(depends);
                }
            }
        }

        EditorUtility.DisplayProgressBar(title, "正在收集...", 1.0f);
        for(int i = 0; i< dependsAssets.Count; i++)
        {
            string assetName = dependsAssets[i];

            if(assetDict.ContainsKey(assetName) || !CheckFileInvalid(assetName)) //资源已经包含了,或无效的
            {
                continue;
            }
            string abName = GetBundleName(assetName) + BundleExt;
            if (!abDict.ContainsKey(abName))
            {
                abDict[abName] = new List<string>();
            }

            abDict[abName].Add(assetName);
            assetDict[assetName] = true;
        }

        foreach(var kv in abDict)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = kv.Key;
            Debug.LogError("ab name :" + kv.Key);
            foreach(var asset in kv.Value)
            {
                Debug.LogError("aset:" + asset);
            }
            build.assetNames = kv.Value.ToArray();
            bundleList.Add(build);
        }
        EditorUtility.ClearProgressBar();
        return bundleList;
    }

    /// <summary>
    /// 根据manifest，生成bundlelist
    /// </summary>
    /// <param name="manifest"></param>
    private static void CreateBundleList(AssetBundleManifest manifest)
    {
        //EditorUtility.DisplayProgressBar("生成bundlelist", "正在生成...", 0f);
        AssetBundleList bundleLists = new AssetBundleList();
        List<string> bundles = new List<string>(manifest.GetAllAssetBundles());
        for(int i = 0; i< bundles.Count; i++)
        {
            Debug.LogError("bundles :" + bundles[i]);
            string abFileName = bundles[i];
            AssetBundleList.AssetBundleInfo bundleInfo = new AssetBundleList.AssetBundleInfo();
            bundleInfo.FileName = abFileName;
            bundleInfo.Md5Name = ConvertABNameToMd5(abFileName);

            string[] dependsName = manifest.GetAllDependencies(abFileName); //记录下ab的依赖的ab的列表
            bundleInfo.DependNames = dependsName;
            bundleLists.BundleList.Add(bundleInfo);
            EditorUtility.DisplayProgressBar("生成bundlelist", string.Format("正在生成 ...{0}/{1}", i, bundles.Count), i * 1.0f/ bundles.Count);
        }
        bundleLists.Platform = PlatformName;
        bundleLists.TotalCount = bundles.Count;
        EditorUtility.ClearProgressBar();
        SaveBundleList(bundleLists);
    }


    private static void SaveBundleList(AssetBundleList bundleList)
    {
        string bundleListPath = BundleRealPath + "/" + BundleListName;
        if (File.Exists(bundleListPath))
            File.Delete(bundleListPath);
        FileStream fs = new FileStream(bundleListPath, FileMode.Create);
        if(fs != null)
        {
            bundleList.SaveToBinary(fs);
        }
    }
    [MenuItem("AssetBundle/载入bundleList-测试")]
    public static void LoadBundleList()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        PlatformName = GetPlatformName(buildTarget);
        BundleRealPath = BundleRealRootPath + "/" + PlatformName; //ab保存的完整目录
        AssetBundleList info = new AssetBundleList();
        string bundleListPath = BundleRealPath + "/" + BundleListName;
        if(File.Exists(bundleListPath))
        {
            FileStream fs = new FileStream(bundleListPath, FileMode.Open);
            if(fs != null)
            {
                info.LoadFromBinary(fs);
            }
        }
    }
    /// <summary>
    /// 获取bundle名字
    /// </summary>
    /// <param name="dirPath"></param>
    /// <returns></returns>
    private static string GetBundleName(string dirPath)
    {
        string assetdir = "Assets/";
        string bundleName = dirPath.Replace("\\", "/");

        if(File.Exists(bundleName)) //如果是文件
        {
            string dir = Path.GetDirectoryName(bundleName);
            dir = dir.Replace("\\", "/");
            bundleName = dir;
        }

        int resourceIndex = bundleName.LastIndexOf(ResourcesDirName + "/"); //注意，要打包的资源，或者要查找依赖的根prefab都放在Resources目录下
        if(resourceIndex != -1)
        {
            bundleName = bundleName.Substring(resourceIndex + ResourcesDirName.Length + 1);//Resources目录下的资源

        }
        else
        {
            bundleName = DependsDir + bundleName.Substring(assetdir.Length); //通过依赖找到的
        }
        return bundleName.ToLower();
    }

    /// <summary>
    /// ab的名字转换为md5, 可以用来缩短路径名，有时候目录太深了，名字会很长
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>
    private static string ConvertABNameToMd5(string abName)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(abName);
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] res = md5.ComputeHash(bytes);
        string md5Val = string.Empty;
        foreach(var b in bytes)
        {
            md5Val += b.ToString("x2").ToLower();
        }
        return md5Val;
    }

    /// <summary>
    /// 检查文件的有效性，判断是否要打包, 待扩展，或者用配置表的方式
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool CheckFileInvalid(string path)
    {
        if(path.EndsWith(".meta") || path.EndsWith(".cs"))
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
                DeleteDirectory(BundleRealPath);
        }

        if (!Directory.Exists(BundleRealPath))
        {
            Directory.CreateDirectory(BundleRealPath);
        }
    }

    public static void DeleteDirectory(string dir)
    {
        if(Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }
        string meta = string.Format("{0}.meta", dir);
        if(File.Exists(meta))
        {
            File.Delete(meta);
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
