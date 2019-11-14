using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetTool : MonoBehaviour
{
    string resPath = "prefabs/cube1";
   
    private AssetBundleList info = new AssetBundleList();
    // Start is called before the first frame update
    void Start()
    {

        //AssetBundleManager.Instance.Initialize();
        //string abName = AssetBundleManager.Instance.GetAssetBundleName(resPath);
        Debug.LogError("abname" + Application.dataPath);
        FileManager.Instance.InitPathInfo();
    }

   

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
