using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class AssetBundleManager : Singleton<AssetBundleManager>
{

    private AssetBundleList m_bundleLists = new AssetBundleList();
    public string RootPath;
    public string BundlePath = "AssetBundles";
    public string BundleRealRootPath;
    public string BundleListName = "Bundle_list.data";


    public void Initialize()
    {
        RootPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        BundleRealRootPath = string.Format("{0}/{1}", RootPath, BundlePath);
       
        LoadBundleList();
    }

    public void Update()
    {
        bool usebunlde = true;
        if(m_bundleLists != null && usebunlde)
        {
            m_bundleLists.CheckAsyncLoad();
        }
    }

    private void LoadBundleList()
    {
        string PlatformName = "Window";
        string BundleRealPath = BundleRealRootPath + "/" + PlatformName; //ab保存的完整目录

        string bundleListPath = BundleRealPath + "/" + BundleListName;
        if (File.Exists(bundleListPath))
        {
            FileStream fs = new FileStream(bundleListPath, FileMode.Open);
            if (fs != null)
            {
                m_bundleLists.LoadFromBinary(fs);
            }
        }
    }

    public string GetAssetBundleName(string assetName)
    {
        if(m_bundleLists != null)
        {
            return m_bundleLists.GetBundleName(assetName);
        }
        return null;
    }

    /// <summary>
    /// 获取ab的依赖的ab列表
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>
    public List<string> GetAllDepends(string abName)
    {
        List<string> depends = new List<string>();
        var AssetInfo = m_bundleLists.GetAssetInfo(abName);
        if(AssetInfo != null)
        {
            if(AssetInfo.DependNames != null)
                depends.AddRange(AssetInfo.DependNames);
        }
        return depends;
    }

    public void LoadAssetBundleByDepends(string abName, bool isSync, ref Dictionary<string, AssetBundleList.AssetBundleInfo> abDict)
    {
        abDict.Clear();
        List<string> depends = GetAllDepends(abName);
        for(int i  = 0; i< depends.Count; i++)
        {
            var Assetinfo = LoadAssetBundle(depends[i], isSync);
            if(Assetinfo != null)
            {
                abDict.Add(depends[i], Assetinfo);
            }
            else
            {
                Debug.LogError("load asset bunldle info is null" + depends[i]);
                continue;
            }
        }

        var MainAssetInfo = LoadAssetBundle(abName, isSync);
        if(MainAssetInfo != null)
        {
            abDict.Add(abName, MainAssetInfo);
        }
    }

    public AssetBundleList.AssetBundleInfo LoadAssetBundle(string abName, bool isSync)
    {
        var assetInfo = m_bundleLists.GetAssetInfo(abName);
        if(assetInfo != null)
        {
            if(assetInfo.IsCanLoad()) //判断是否可以加载，是否已经加载过了
            {
                string abPath = GetBundleFullPath(abName);
                if (isSync)
                {
                    if(assetInfo.LoadRequest != null) //同步的会取消掉异步的检查,不用等待了,但是还是会重复的加载
                    {
                        assetInfo.LoadRequest = null; 
                    }

                    AssetBundle assetBundle = AssetBundle.LoadFromFile(abPath);
                    Debug.LogError("load bundle" + abPath);
                    if (assetBundle != null)
                    {
                        assetInfo.assetBundle = assetBundle;
                    }
                }
                else
                {
                    if(assetInfo.LoadRequest == null) //避免两个异步同时加载
                    {
                        var requeset = AssetBundle.LoadFromFileAsync(abPath);
                        Debug.LogError("load bundle" + abPath);
                        if (requeset != null)
                        {
                            assetInfo.LoadRequest = requeset;
                            m_bundleLists.AddAssetInfoToLoadingList(assetInfo);
                        }
                    }
                    

                }
                return assetInfo;
            }
            
        }
        return null;
    }

    /// <summary>
    /// 卸载ab
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="unloadAllLoadedObjects">为true,不仅会卸载ab,而且会卸载ab中加载的资源</param>
    public void UnloadAssetBundle(string bundleName, bool unloadAllLoadedObjects = false)
    {
        if (string.IsNullOrEmpty(bundleName))
            return;

        m_bundleLists.UnloadAssetBundle(bundleName, unloadAllLoadedObjects);
    }

    /// <summary>
    /// 通过assetname来卸载对应的ab
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="unloadAllLoadedObjects"></param>
    public void UnloadAssetBundleByAssetName(string assetName, bool unloadAllLoadedObjects = false)
    {
        if (string.IsNullOrEmpty(assetName))
            return;
        string bundleName = GetAssetBundleName(assetName);
        Debug.LogErrorFormat("unload ab {0}, by asset :{1}", bundleName, assetName);

        if (!string.IsNullOrEmpty(bundleName))
        {
            m_bundleLists.UnloadAssetBundle(bundleName, unloadAllLoadedObjects);
        }
    }

    public string GetBundleFullPath(string abName)
    {
        return FileManager.Instance.GetFullPath(abName);
    }

    public string GetAssetName(string resPath)
    {
        return Path.GetFileName(resPath);
    }
    
}
