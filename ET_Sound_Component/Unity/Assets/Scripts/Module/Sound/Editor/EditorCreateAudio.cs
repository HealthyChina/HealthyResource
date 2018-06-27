using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ETModel
{
    public class EditorCreateAudio : MonoBehaviour
    {
        //音效资源路径
        private static string audiosDir = "assets/ETAB/audios";
        //导出预制体路径
        private static string prefabDir = "assets/ETAB/SoundPrefabs";

        [MenuItem("Tools/创建音效预设", priority = 1004)]
        static void CreateAudioPrefab()
        {
            string[] _patterns = new string[] { "*.mp3","*.wav", "*.ogg" };//识别不同的后缀名
            List<string> _allFilePaths = new List<string>();
            foreach (var item in _patterns)
            {
                string[] _temp = Directory.GetFiles(audiosDir, item, SearchOption.AllDirectories);
                _allFilePaths.AddRange(_temp);
            }
            foreach (var item in _allFilePaths)
            {
                System.IO.FileInfo _fi = new System.IO.FileInfo(item);
                var _tempName = _fi.Name.Replace(_fi.Extension, "").ToLower();
                AudioClip _clip = AssetDatabase.LoadAssetAtPath<AudioClip>(item);
                if (null != _clip)
                {
                    GameObject _go = new GameObject();
                    _go.name = _tempName;
                    AudioSource _as = _go.AddComponent<AudioSource>();
                    _as.playOnAwake = false;
                    SoundData _data = _go.AddComponent<SoundData>();
                    _data.audio = _as;
                    _data.audio.clip = _clip;
                    string path = $"{prefabDir}/{_tempName}.prefab";
                    var temp = PrefabUtility.CreatePrefab(path, _go);

                    //添加ab标记
                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    if (importer == null || temp == null)
                    {
                        Debug.LogError("error: " + path);
                        return;
                    }
                    importer.assetBundleName = "sound.unity3d";

                    GameObject.DestroyImmediate(_go);
                    EditorUtility.SetDirty(temp);
                    Resources.UnloadAsset(_clip);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}