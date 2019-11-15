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

    private AssetBundleList info = new AssetBundleList();
    // Start is called before the first frame update
    void Start()
    {

        AssetBundleManager.Instance.Initialize();
        FileManager.Instance.InitPathInfo();
        ResourceManager.Instance.Initialize();

        ResourceManager.Instance.LoadAsync<GameObject>(resPath, (LoadAssetTask task) =>
        {
            GameObject prefab = task.LoadedAsset as GameObject;
            prefab = Instantiate(prefab);
        });

        ResourceManager.Instance.LoadAsync<Sprite>(imgPath, (LoadAssetTask task) =>
        {
            Sprite img = task.LoadedAsset as Sprite;
            m_image.sprite = img;
        });

        //ResourceManager.Instance.LoadAsync<Sprite>(imgPath2, (LoadAssetTask task) =>
        //{
        //    Sprite img = task.LoadedAsset as Sprite;
        //    m_image2.sprite = img;
        //});

        //GameObject prefab = ResourceManager.Instance.Load<GameObject>(resPath);
        //prefab = Instantiate(prefab);


        // FileManager.Instance.InitPathInfo();
    }

   

    

    // Update is called once per frame
    void Update()
    {
        ResourceManager.Instance.Update();
        AssetBundleManager.Instance.Update();
    }
}
