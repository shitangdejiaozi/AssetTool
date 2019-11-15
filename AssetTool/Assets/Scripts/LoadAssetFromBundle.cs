using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadAssetFromBundle : LoadAssetTask
{
    private Dictionary<string, AssetBundleList.AssetBundleInfo> m_abDict = new Dictionary<string, AssetBundleList.AssetBundleInfo>();
    private string mainAbName = string.Empty;
    private AssetBundleRequest m_loadAssetRequest = null;

    public LoadAssetFromBundle(string path, Type type) : base(path, type)
    {

    }

    public override void StartLoad()
    {
        base.StartLoad();

        mainAbName = AssetBundleManager.Instance.GetAssetBundleName(assetPath);
        if(string.IsNullOrEmpty(mainAbName))
        {
            Debug.LogError("ab name is null" + assetPath);
            isComplete = true;
            DoCallBack();
            return;
        }
        AssetBundleManager.Instance.LoadAssetBundleByDepends(mainAbName, isSync, ref m_abDict);

        if(isSync)
        {
            CheckLoad();
        }
    }

    public override void CheckLoad()
    {
        base.CheckLoad();
        if(CheckAllLoad())
        {
            AssetBundleList.AssetBundleInfo assetInfo = null;
            AssetBundle mainAb = null;
            m_abDict.TryGetValue(mainAbName, out assetInfo);
            if (assetInfo == null)
                return;
            mainAb = assetInfo.assetBundle;
            if(mainAb == null)
            {
                Debug.LogError("load assetbunlde is null" + mainAbName);
                isComplete = true;
                return;
            }

            if(isSync)
            {
                string assetName = AssetBundleManager.Instance.GetAssetName(assetPath);
                Debug.LogError("assetname :" + assetName);
                if(!string.IsNullOrEmpty(assetName))
                {
                    LoadedAsset = mainAb.LoadAsset(assetName, assetType);
                }
                isComplete = true;
            }
            else
            {
                if(m_loadAssetRequest == null)
                {
                    string assetName = AssetBundleManager.Instance.GetAssetName(assetPath);
                    if (!string.IsNullOrEmpty(assetName))
                    {
                        m_loadAssetRequest = mainAb.LoadAssetAsync(assetName, assetType);
                    }
                    return;
                }

                if(m_loadAssetRequest.isDone)
                {
                    if(m_loadAssetRequest.asset != null)
                    {
                        LoadedAsset = m_loadAssetRequest.asset;
                    }
                    m_loadAssetRequest = null;
                    isComplete = true;
                }
            }

            DoCallBack();
        }
    }

    public bool CheckAllLoad()
    {
        bool load = true;
        foreach(KeyValuePair<string , AssetBundleList.AssetBundleInfo> p in m_abDict)
        {
            if(p.Value.IsLoaded() == false)
            {
                load = false;
                break;
            }
        }

        return load;
    }
}
