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

    private Dictionary<string, AssetBundleList.AssetBundleInfo> m_abDict = new Dictionary<string, AssetBundleList.AssetBundleInfo>();

    public void Initialize()
    {
        RootPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        BundleRealRootPath = string.Format("{0}/{1}", RootPath, BundlePath);
       
        LoadBundleList();
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
            depends.AddRange(AssetInfo.DependNames);
        }
        return depends;
    }

    public void LoadAssetBundleByDepends(string abName, bool isSync)
    {
        m_abDict.Clear();
        List<string> depends = GetAllDepends(abName);
        for(int i  = 0; i< depends.Count; i++)
        {
            var Assetinfo = LoadAssetBundle(depends[i], isSync);
            if(Assetinfo != null)
            {
                m_abDict.Add(depends[i], Assetinfo);
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
            m_abDict.Add(abName, MainAssetInfo);
        }
    }

    public AssetBundleList.AssetBundleInfo LoadAssetBundle(string abName, bool isSync)
    {
        var assetInfo = m_bundleLists.GetAssetInfo(abName);
        if(assetInfo != null)
        {
            string abPath = GetBundleFullPath(abName);
            if(isSync)
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(abPath);
                Debug.LogError("load bundle" + abPath);
                if(assetBundle != null)
                {
                    assetInfo.assetBundle = assetBundle;
                }
            }
            return assetInfo;
        }
        return null;
    }

    public string GetBundleFullPath(string abName)
    {
        return FileManager.Instance.GetFullPath(abName);
    }

    
}
