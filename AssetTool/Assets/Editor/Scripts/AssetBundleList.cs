using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AssetBundleList 
{
    public string Platform = string.Empty; 
    public int TotalCount = 0;
    public List<AssetBundleInfo> BundleList = new List<AssetBundleInfo>();

    public class AssetBundleInfo
    {
        public string FileName = string.Empty;
        public string Md5Name = string.Empty;
        private AssetBundle _assetBundle = null;
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
                Debug.LogError("file name" + abInfo.FileName);
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

}
