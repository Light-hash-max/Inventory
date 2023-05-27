using System.IO;
using UnityEngine;


namespace LinkedSquad.FileSystem
{
    public class WebGLFileManager : IFileManager
    {
        public void InitDeps() { }
        public void Start() { }
        public void Clean() { }

        public string ReadAllText(string path)
        {
            var asset = Resources.Load<TextAsset>(path);

            return asset.text;
        }

        public bool Exists(string path)
        {
            return Resources.Load(path) != null;
        }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public bool WriteAllText(string path, string content)
        {
            Debug.LogError("Cannot save data on WEBGL");

            return false;
        }

        public bool SaveFile(string path, object data)
        {
            Debug.LogError($"[{nameof(WebGLFileManager)}.SaveFile] Not supported on WebGL");
            return false;
        }

        public object LoadFile(string path)
        {
            Debug.LogError($"[{nameof(WebGLFileManager)}.LoadFile] Not supported on WebGL");
            return null;
        }
    }
}