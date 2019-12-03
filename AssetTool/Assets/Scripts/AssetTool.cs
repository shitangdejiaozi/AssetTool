using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class AssetTool : MonoBehaviour
{
    public Image m_image;
    public Image m_image2;
    string resPath = "prefabs/cube1";
    string imgPath = "atlas/sprite1/Blood1";
    string imgPath2 = "atlas/sprite1/Blood2";
    public Button button1;
    public Button button2;

    private AssetBundleList info = new AssetBundleList();
    // Start is called before the first frame update
    void Start()
    {

        AssetBundleManager.Instance.Initialize();
        FileManager.Instance.InitPathInfo();
        ResourceManager.Instance.Initialize();

        button1.onClick.AddListener(UnloadBundle);
        button2.onClick.AddListener(LoadAsset);

        ResourceManager.Instance.LoadAsync<GameObject>(resPath, (LoadAssetTask task) =>
        {
            GameObject prefab = task.LoadedAsset as GameObject;
            prefab = Instantiate(prefab);
        });

        ResourceManager.Instance.LoadAsync<Sprite>(imgPath, (LoadAssetTask task) =>
        {
            Sprite img1 = task.LoadedAsset as Sprite;
           // m_image.sprite = img1;
        });

       
        
        //ResourceManager.Instance.LoadAsync<Sprite>(imgPath2, (LoadAssetTask task) =>
        //{
        //    Sprite img2 = task.LoadedAsset as Sprite;
        //    m_image2.sprite = img;
        //});

        //GameObject prefab = ResourceManager.Instance.Load<GameObject>(resPath);
        //prefab = Instantiate(prefab);


        // FileManager.Instance.InitPathInfo();
    }

   
    private void UnloadBundle()
    {
        string bundleName = AssetBundleManager.Instance.GetAssetBundleName(imgPath2);

        AssetBundleManager.Instance.UnloadAssetBundleByAssetName(imgPath2, false);
    }
    
    private  void LoadAsset()
    {
        Sprite img = ResourceManager.Instance.Load<Sprite>(imgPath);
    }

    // Update is called once per frame
    void Update()
    {
        ResourceManager.Instance.Update();
        AssetBundleManager.Instance.Update();
    }
}
