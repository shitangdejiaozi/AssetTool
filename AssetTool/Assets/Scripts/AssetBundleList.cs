using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class AssetBundleList 
{
    public string Platform = string.Empty; 
    public int TotalCount = 0;
    public List<AssetBundleInfo> BundleList = new List<AssetBundleInfo>();
    private List<AssetBundleInfo> m_loadingBundleList = new List<AssetBundleInfo>();
    private string Assetbundle_Ex = ".ab";

    public class AssetBundleInfo
    {
        public string FileName = string.Empty;
        public string Md5Name = string.Empty;
        private AssetBundle _assetBundle = null;
        private AssetBundleCreateRequest _loadRequest = null;
        public string[] DependNames = null;
        public int[] DependIndex = null;

        public AssetBundle assetBundle
        {
            get
            {
                return _assetBundle;
            }
            set
            {
                _assetBundle = value;
            }
        }

        public AssetBundleCreateRequest LoadRequest
        {
            get
            {
                return _loadRequest;
            }
            set
            {
                _loadRequest = value;
            }
        }

        public bool IsLoaded()
        {
            if (_assetBundle != null)
                return true;
            else
                return false;
        }

        public bool CheckLoading()
        {
            if (_loadRequest == null)
                return false;

            if (!_loadRequest.isDone)
                return true;

            if(_loadRequest.assetBundle != null)
            {
                assetBundle = _loadRequest.assetBundle;
            }
            _loadRequest = null;
            return false;
        }


    }

    public void SaveToBinary(Stream stream)
    {
        try
        {
            BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(Platform);
            bw.Write(TotalCount);

            for(int i = 0; i< BundleList.Count; i++)
            {
                AssetBundleInfo bundleInfo = BundleList[i];
                bw.Write(bundleInfo.FileName);
                bw.Write(bundleInfo.Md5Name);
                if(bundleInfo.DependNames != null)
                {
                    bw.Write(bundleInfo.DependNames.Length);
                    for(int j = 0; j < bundleInfo.DependNames.Length; j++)
                    {
                        bw.Write(bundleInfo.DependNames[j]);
                    }
                }
                else
                {
                    bw.Write(0);
                }
            }

            bw.Close();
            stream.Close();
        }
        catch(System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public void LoadFromBinary(Stream stream)
    {
        try
        {
            BinaryReader br = new BinaryReader(stream);
            Platform = br.ReadString();
            TotalCount = br.ReadInt32();
            Debug.LogError("total Count" + TotalCount);
            for(int i = 0; i< TotalCount; i++ )
            {
                AssetBundleInfo abInfo = new AssetBundleInfo();
                abInfo.FileName = string.Intern(br.ReadString());
             //   Debug.LogError("file name" + abInfo.FileName);
                abInfo.Md5Name = br.ReadString();
                int dependsCount = br.ReadInt32();
                if(dependsCount != 0)
                {
                    List<string> dependsName = new List<string>();
                    for(int j = 0; j < dependsCount; j++)
                    {
                        dependsName.Add(br.ReadString());

                    }
                    abInfo.DependNames = dependsName.ToArray();
                }
                BundleList.Add(abInfo);
            }
            br.Close();
            stream.Close();
        }
        catch(System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public AssetBundleInfo GetAssetInfo(string bundleName)
    {
        for(int i = 0; i< BundleList.Count; i++)
        {
            if(BundleList[i].FileName == bundleName)
            {
                return BundleList[i];

            }
        }
        return null;
    }

    public string GetBundleName(string assetName)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(Path.GetDirectoryName(assetName)).Append(Assetbundle_Ex);
        string abName = builder.ToString().ToLower();
        abName = abName.Replace("\\", "/");
        Debug.LogError("ab name :" + abName);
        string realName = null;
        var AssetInfo = GetAssetInfo(abName);

        if(AssetInfo != null)
        {
            realName = AssetInfo.FileName;
        }
        else
        {
            Debug.LogError("res is invalid");
        }

        return realName;
    }

    public void AddAssetInfoToLoadingList(AssetBundleInfo bundleInfo)
    {
        m_loadingBundleList.Add(bundleInfo);
    }

    public void CheckAsyncLoad()
    {
        for(int i = 0; i < m_loadingBundleList.Count; i++)
        {
            var bundleInfo = m_loadingBundleList[i];
            if (!bundleInfo.CheckLoading())
            {
                m_loadingBundleList.Remove(bundleInfo);
            }

        }
       

    }
}
