using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResourceManager : Singleton<ResourceManager>
{
    public string configPath = "Assets/buildConfig.asset";
    private List<string> bundleResRoot = new List<string>(); //保存打包的资源的相对于Resources的路径
    private string strRes = "Resources";

    public void Initialize()
    {
        var buildCfg = AssetDatabase.LoadAssetAtPath<BuildConfig>(configPath);
        var buildList = buildCfg.buildPaths;
        for (int i = 0; i < buildList.Count; i++)
        {
            int index = buildList[i].IndexOf(strRes);
            if (index != -1)
            {
                string path = buildList[i].Substring(index + strRes.Length + 1);
                bundleResRoot.Add(path);
            }
        }
    }

    public T Load<T>(string resPath) where T: UnityEngine.Object
    {
        if(string.IsNullOrEmpty(resPath))
        {
            return null;
        }



        return null;
    }

    private bool IsUseBundle(string resPath)
    {
        bool isUse = false;
#if USE_ASSETBUNDLE
        
        for(int i = 0; i< bundleResRoot.Count; i++)
        {
            if(resPath.StartsWith(bundleResRoot[i]))
            {
                isUse = true;
                break;
            }
        }
#endif
        return isUse;
    }

    private void LoadFromResource(string resPath)
    {

    }

    private void LoadFromBundle(string resPath)
    {

    }
}
