using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildTool 
{
    private static string CompanyName = "zxl";
    private static string App_Name = "AssetToolDemo";
    private static string Init_Scene = "Assets/Scenes/Main.unity";

    //[MenuItem("AssetBundle/PC")]
    public static void BuildPCPackage()
    {
        string arg;
        string targetPath = "";
        Dictionary<string, string> Args;
        GetCommandLineArgs(out Args);
        if(Args.TryGetValue("BUILD_PATH", out arg))
        {
            targetPath = arg;
        }
        Debug.LogError("target path" + targetPath);
        if(string.IsNullOrEmpty(targetPath))
        {
            targetPath = string.Format("{0}/{1}.exe", GetBuildDir(), App_Name);
        }
        
        
        PlayerSettings.companyName = CompanyName;
        PlayerSettings.productName = App_Name;

        string[] scenes = new string[] { Init_Scene };
        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();
        for(int i = 0; i< scenes.Length; i++)
        {
            buildScenes.Add(new EditorBuildSettingsScene(scenes[i], true));
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        BuildOptions buildOptions = BuildOptions.None;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
#if UNITY_2018_3_OR_NEWER
        var res = BuildPipeline.BuildPlayer(scenes, targetPath, BuildTarget.StandaloneWindows64, buildOptions);

#else
        string res = BuildPipeline.BuildPlayer(scenes, targetPath, BuildTarget.StandaloneWindows64, buildOptions);
        if(res.Length > 0)
        {
            Debug.LogError("build fail" + res);
        }
#endif


    }
    
    private static void GetCommandLineArgs(out Dictionary<string, string> Args)
    {
        Args = new Dictionary<string, string>();
        foreach(var arg in System.Environment.GetCommandLineArgs())
        {
            Debug.LogError("arg :" + arg);
            int index = arg.IndexOf("=");
            if(index > 0)
            {
                Args.Add(arg.Substring(0, index), arg.Substring(index + 1));
            }
        }
    }

    private static string GetBuildDir()
    {
        string Path = Application.dataPath.Remove(Application.dataPath.LastIndexOf("/Assets"), "/Assets".Length) + "/Build";
        if(Directory.Exists(Path))
        {
            Directory.Delete(Path,true);
        }
        Directory.CreateDirectory(Path);
        return Path;
    }
}
