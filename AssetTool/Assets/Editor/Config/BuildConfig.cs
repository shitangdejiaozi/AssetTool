using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在Assets/Create下
/// </summary>
[CreateAssetMenu(menuName = "AssetBunlde/Build config", fileName = "Assets/buildConfig")]
public class BuildConfig : ScriptableObject
{
    [SerializeField]
    public List<string> buildPaths = new List<string>(); //按目录打包的路径
    [SerializeField]
    public List<string> collectPaths = new List<string>(); //收集依赖打包的路径
}
