using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 图集打包
/// </summary>
public class AtlasPackerEditor: Editor
{
    /// <summary>
    /// 图集图片的路径
    /// </summary>
    public static string AtlasPath = "Assets/codeandweb.com/Example/SpriteSheet";
    /// <summary>
    /// 打包后预制体存放路径
    /// </summary>
    public static string UISharePath = "Assets/codeandweb.com/Example/";

    [MenuItem("渐渐/打包所有图集")]
    public static void PackAtlas()
    {
        string[] _patterns = new string[] { "*.png" };
        string multiTag = "_multi_";
        Dictionary<string, List<string>> _allFilePaths = new Dictionary<string, List<string>>();
        string _tempName = String.Empty;
        foreach (var item in _patterns)
        {
            string[] _temp = Directory.GetFiles(AtlasPath, item, SearchOption.AllDirectories);
            foreach (string path in _temp)
            {
                System.IO.FileInfo _fi = new System.IO.FileInfo(path);
                _tempName = _fi.Name.Replace(_fi.Extension, "").ToLower();
                if (!_tempName.Contains(multiTag))
                {
                    _tempName = "atl_" + _tempName;
                }
                else
                {
                    _tempName = "atl_"+ _tempName.Split(multiTag)[0];
                }
                if (!_allFilePaths.ContainsKey(_tempName))
                {
                    _allFilePaths.Add(_tempName, new List<string>());
                }
                _allFilePaths[_tempName].Add(path);
            }
        }

        foreach (var item in _allFilePaths)
        {
            CreateAtlasPrefab(item.Key, item.Value.ToArray());
        }
    }

    //创建图集预设
    public static void CreateAtlasPrefab(string atlasName, string[] atlPath)
    {
        List<Sprite> _texs = new List<Sprite>();
        foreach (string path in atlPath)
        {
            _texs.AddRange(AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray());
        }
        if (null != _texs)
        {
            GameObject _go = new GameObject();
            _go.name = atlasName;
            SpriteData _spData = _go.AddComponent<SpriteData>();
            _spData.SetSP = _texs.ToArray();
            string path1 = UISharePath + atlasName + ".prefab";
            GameObject temp = PrefabUtility.CreatePrefab(path1, _go);

            #region 添加ab标记
            //此处自动添加ab标记
            //如果加载方式是Resources.load()等不需要ab标记的可以把此处注释掉
            AssetImporter importer = AssetImporter.GetAtPath(path1);
            if (importer == null || temp == null)
            {
                Debug.LogError("error: " + path1);
                return;
            }
            importer.assetBundleName = "ui-share.unity3d";

            #endregion

            GameObject.DestroyImmediate(_go);
            EditorUtility.SetDirty(temp);
            AssetDatabase.SaveAssets();
        }
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }
}

public static class Tools_Z
{
    /// <summary>
    /// 根据字符串来分割字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string[] Split(this string str, string separator)
    {
		return Regex.Split(str, separator, RegexOptions.IgnoreCase);
    }
}