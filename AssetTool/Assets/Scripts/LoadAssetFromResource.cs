using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadAssetFromResource : LoadAssetTask
{
    public LoadAssetFromResource(string path, Type type) : base(path, type)
    {

    }

    private ResourceRequest assetRequest;

    public override void StartLoad()
    {
        base.StartLoad();

        if(isSync)
        {
            LoadedAsset = Resources.Load(assetPath, assetType);
            CheckLoad();
        }
        else
        {
            assetRequest = Resources.LoadAsync(assetPath, assetType);
        }
        
    }

    public override void CheckLoad()
    {
        base.CheckLoad();
        if(isSync)
        {
            isComplete = true;
        }
        else
        {
            if(assetRequest != null && assetRequest.isDone)
            {
                LoadedAsset = assetRequest.asset;
                isComplete = true;
                assetRequest = null;
            }
        }
        DoCallBack();
    }
}
