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

    private List<LoadAssetTask> m_loadingAssetTask = new List<LoadAssetTask>();  //正在加载的任务
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

    public void Update()
    {
        for(int i = 0; i< m_loadingAssetTask.Count; i++)
        {
            var task = m_loadingAssetTask[i];
            try
            {
                task.CheckLoad();
            }
            catch(Exception e)
            {
                Debug.LogError(" error" + e.Message);
            }
            if(task.isComplete)
            {
                m_loadingAssetTask.Remove(task);
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
        LoadAssetTask loadTask;
        if (IsUseBundle(resPath))
        {
            loadTask = new LoadAssetFromBundle(resPath, typeof(T));
        }
        else
        {
            loadTask = new LoadAssetFromResource(resPath, typeof(T));
        }

        loadTask.isSync = true;
        loadTask.StartLoad();

        resObj = loadTask.LoadedAsset as T;
        if(resObj == null)
        {
            Debug.LogError("加载失败: " + resPath);
        }
        return resObj;
    }

    public LoadAssetTask LoadAsync<T>(string resPath, Action<LoadAssetTask> callBack = null) where T: UnityEngine.Object
    {

        if (string.IsNullOrEmpty(resPath))
        {
            return null;
        }
        LoadAssetTask loadTask;
        if (IsUseBundle(resPath))
        {
            loadTask = new LoadAssetFromBundle(resPath, typeof(T));
        }
        else
        {
            loadTask = new LoadAssetFromResource(resPath, typeof(T));
        }
        loadTask.isSync = false;
        loadTask.loadedCallback = callBack;
        loadTask.StartLoad();

        return loadTask;
    }

    private bool IsUseBundle(string resPath)
    {
        bool isUse = true;
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

    public void AddLoadAssetTask(LoadAssetTask task)
    {
        m_loadingAssetTask.Add(task);
    }
}
