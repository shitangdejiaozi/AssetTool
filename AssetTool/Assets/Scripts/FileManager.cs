using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class FileManager : Singleton<FileManager>
{
    private string m_updatePath = string.Empty;
    private string m_BundlePath = string.Empty;
    private string m_platform = string.Empty;
    private string m_dataPath = string.Empty;
    private string m_assetsName = "Assets";
    private string m_BasePath = string.Empty;


    public void InitPathInfo()
    {
        switch(Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                m_platform = "Window";
                break;
            case RuntimePlatform.Android:
                m_platform = "Android";
                break;
        }

        m_updatePath = Application.persistentDataPath;
        m_dataPath = Application.dataPath;
        m_BasePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));

        StringBuilder builder = new StringBuilder();
        builder.Append(m_BasePath).Append("/AssetBundles/").Append(m_platform);
        m_BundlePath = builder.ToString();
        Debug.LogError("bundlePath" + m_BundlePath);
    }

    public string GetFullPath(string path)
    {
#if UNITY_EDITOR 
        return m_BundlePath +"/" + path;
#endif
        string updatePath = GetUpdatePath(path);
        if (File.Exists(updatePath))
        {
            return updatePath;
        }
        else
        {
            return GetAppPath(path);
        }
    }

    /// <summary>
    /// 获取包内的路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetAppPath(string path)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(Application.streamingAssetsPath).Append("/").Append(path);
        return builder.ToString();
    }

    /// <summary>
    /// 获取更新目录下的路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetUpdatePath(string path)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(m_updatePath).Append("/").Append(path);
        return builder.ToString();
    }
}
