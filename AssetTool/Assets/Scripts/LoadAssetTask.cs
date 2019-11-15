using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class LoadAssetTask 
{
    public bool isSync;

    public bool isComplete;
    public bool isLoading;
    private Object m_loadedAsset = null;
    public string assetPath;
    public Type assetType;
    public Action<LoadAssetTask> loadedCallback; //加载后回调

    public Object LoadedAsset
    {
        get
        {
            return m_loadedAsset;
        }
        set
        {
            m_loadedAsset = value;
        }
    }

    public LoadAssetTask()
    {

    }

    public LoadAssetTask(string  path, Type type)
    {
        assetPath = path;
        assetType = type;

        isSync = true;
        isComplete = false;
        m_loadedAsset = null;
        isLoading = false;
    }

    public virtual void StartLoad()
    {
        isLoading = true;
        if(!isSync)
        {
            ResourceManager.Instance.AddLoadAssetTask(this);
        }
    }

    public virtual void CheckLoad()
    {

    }

    public void DoCallBack()
    {
        if(isComplete && loadedCallback != null)
        {
            try
            {
                loadedCallback(this);
                Debug.LogError(" execute call back" + assetPath + ":load complete");
            }
            catch (Exception e)
            {
                Debug.LogError("load asset call back error" + assetPath + e.Message);
            }
            finally
            {
                loadedCallback = null;
            }  
            
        }
    }
    
}
