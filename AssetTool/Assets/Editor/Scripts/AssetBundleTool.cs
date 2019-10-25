using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class AssetBundleTool 
{
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
}
