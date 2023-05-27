using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace LinkedSquad.FileSystem
{
    public class StandaloneFileManager : IFileManager
    {
        private object m_Locker = new object();



        public void InitDeps() { }
        public void Start() { }
        public void Clean() { }

        public string ReadAllText(string path)
        {
            lock (m_Locker)
            {
                if (!File.Exists(path))
                {
                    Debug.LogError($"[{nameof(StandaloneFileManager)}:ReadAllText] File {path} does not exist!");
                    return null;
                }

                return File.ReadAllText(path);
            }
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public bool WriteAllText(string path, string content)
        {
            lock (m_Locker)
            {
                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (File.Exists(path))
                    File.Delete(path);

                try
                {
                    File.WriteAllText(path, content);
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                    return true;
                }
                catch (System.Exception ex)
                {
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                    Debug.LogError(ex);
                    return false;
                }
            }
        }

        public bool SaveFile(string path, object data)
        {
            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(fs, data);

                fs.Close();

                return true;
            }
            catch (SerializationException ex)
            {
                Debug.LogError($"[{nameof(StandaloneFileManager)}.SaveFile] Error occured while saving file at path: '{path}'. Error: '{ex}'");
                return false;
            }
        }

        public object LoadFile(string path)
        {
            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            FileStream fs = new FileStream(path, FileMode.Open);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                var data = formatter.Deserialize(fs);

                fs.Close();

                return data;
            }
            catch (SerializationException ex)
            {
                Debug.LogError($"[{nameof(StandaloneFileManager)}.LoadFile] Error occured while loading file at path: '{path}'. Error: '{ex}'");

                return null;
            }
        }
    }
}