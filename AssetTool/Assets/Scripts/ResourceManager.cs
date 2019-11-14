using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
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
        T resObj;
        if(IsUseBundle(resPath))
        {
            resObj = LoadFromBundle(resPath, typeof(T) ,true) as T;
        }
        else
        {
            resObj = LoadFromResource(resPath, typeof(T), true) as T;
        }


        return resObj;
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

    private Object LoadFromResource(string resPath, Type type, bool isSync)
    {
        Object loadAssets = new Object();
        if(isSync)
        {
            loadAssets = Resources.Load(resPath, type);
        }
        else
        {
            var request = Resources.LoadAsync(resPath, type);
        }
        return loadAssets;
    }

    private Object LoadFromBundle(string resPath, Type type, bool isSync)
    {
        Object loadAssets = new Object();
        string abName = AssetBundleManager.Instance.GetAssetBundleName(resPath);

        if(isSync)
        {


        }
        return null;
    }
}
