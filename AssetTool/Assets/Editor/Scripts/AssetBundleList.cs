using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


}
