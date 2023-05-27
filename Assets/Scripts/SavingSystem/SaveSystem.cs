using Dioinecail.ServiceLocator;
using LinkedSquad.FileSystem;
using LinkedSquad.SavingSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LinkedSquad.SavingSystem
{
    [Serializable]
    public class SaveData
    {
        public string FieldName;
        public object Data;
    }

    [DefaultImplementation(typeof(ISaveSystem))]
    public class SaveSystem : ISaveSystem
    {
        public static string SaveFilePath => Application.persistentDataPath + "/SomeData/d.snap";

        public event Action OnDataSaved;
        public event Action OnDataLoaded;

        private IFileManager fileManager;
        private Dictionary<string, SaveData> dataDictionary = new Dictionary<string, SaveData>();



        public void Load()
        {
            SceneManager.sceneLoaded += HandleSceneReload;
            SceneManager.LoadScene(0);
        }

        private void HandleSceneReload(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= HandleSceneReload;

            OnDataLoaded = null;

            var loadableObjects = UnityEngine.Object.FindObjectsOfType<SaveableMonobehaviour>(true);

            foreach (var loadable in loadableObjects)
            {
                loadable.SubscribeForSaving(this);
            }

            dataDictionary = (Dictionary<string, SaveData>)fileManager.LoadFile(SaveFilePath);

            if (dataDictionary == null)
                dataDictionary = new Dictionary<string, SaveData>();

            foreach (var obj in loadableObjects)
            {
                LoadObject(obj);
            }

            OnDataLoaded?.Invoke();
        }

        public void Save()
        {
             var saveableObjects = UnityEngine.Object.FindObjectsOfType<SaveableMonobehaviour>(true);

            foreach (var obj in saveableObjects)
            {
                SaveObject(obj);
            }

            fileManager.SaveFile(SaveFilePath, dataDictionary);

            OnDataSaved?.Invoke();
        }

        public void SaveObject(SaveableMonobehaviour obj)
        {
            var fields = obj.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.FieldType.GetInterface(nameof(ISaveable)) != null 
                        || f.FieldType.Equals(typeof(SaveableParameters)));

            foreach (var field in fields)
            {
                var value = field.GetValue(obj);

                SaveObjectRecursive(obj.name, field, value, 0);
            }
        }

        private void SaveObjectRecursive(string parentFieldName, FieldInfo field, object value, int recursiveIndex)
        {
            if (recursiveIndex > 3)
                return;

            var fieldName = field.Name;
            var guidField = field.FieldType.GetField("Guid", BindingFlags.Public | BindingFlags.Instance);

            if (guidField != null)
            {
                var guid = (string)guidField.GetValue(value);

                if (string.IsNullOrEmpty(guid))
                {
                    guid = Guid.NewGuid().ToString();

                    guidField.SetValue(value, guid);
                }

                var isSaveField = field.FieldType.GetField("IsSave", BindingFlags.Public | BindingFlags.Instance);

                if (isSaveField == null)
                {
                    if (!field.FieldType.Equals(typeof(SaveableParameters)))
                        return;
                }
                else
                {
                    var isSave = (bool)isSaveField.GetValue(value);

                    if (!isSave)
                        return;

                    var fieldType = field.FieldType;
                    var saveMethod = fieldType.GetMethod("Save", BindingFlags.Public | BindingFlags.Instance);

                    var data = saveMethod.Invoke(value, null);
                    var saveData = new SaveData() { FieldName = $"{parentFieldName}.{fieldName}", Data = data };

                    if (dataDictionary.ContainsKey(guid))
                        dataDictionary[guid] = saveData;
                    else
                        dataDictionary.Add(guid, saveData);
                }
            }

            var fields = field.FieldType
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (value == null)
                return;

            foreach (var f in fields)
            {
                var recursiveValue = f.GetValue(value);

                SaveObjectRecursive($"{parentFieldName}.{fieldName}", f, recursiveValue, recursiveIndex + 1);
            }
        }

        public void LoadObject(SaveableMonobehaviour obj)
        {
            var fields = obj.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var value = field.GetValue(obj);

                LoadFieldRecursive(obj.name, field, value, 0);
            }

            obj.OnObjectLoaded();
        }

        private void LoadFieldRecursive(string objName, FieldInfo field, object value, int recursiveIndex)
        {
            if (recursiveIndex > 3)
                return;

            var isLoaded = field.FieldType.GetProperty("IsLoaded", BindingFlags.Public | BindingFlags.Instance);

            if (isLoaded != null && value != null)
            {
                if ((bool)isLoaded.GetValue(value))
                    return;
            }

            var guidField = field.FieldType.GetField("Guid", BindingFlags.Public | BindingFlags.Instance);

            if (guidField != null)
            {
                var guid = (string)guidField.GetValue(value);

                if (string.IsNullOrEmpty(guid))
                {
                    Debug.LogError($"[{typeof(SaveSystem)}.LoadObject] [{objName}.{field.Name}] has no Guid assigned. WTF?!");
                    return;
                }

                var isSaveField = field.FieldType.GetField("IsSave", BindingFlags.Public | BindingFlags.Instance);

                if (isSaveField == null)
                {
                    if(!field.FieldType.Equals(typeof(SaveableParameters)))
                        return;
                }
                else
                {
                    var isSave = (bool)isSaveField.GetValue(value);

                    if (!isSave)
                        return;

                    if (dataDictionary.TryGetValue(guid, out var data))
                    {
                        var fieldType = field.FieldType;
                        var loadMethod = fieldType.GetMethod("Load", BindingFlags.Public | BindingFlags.Instance);

                        loadMethod.Invoke(value, new object[] { data.Data });
                        isLoaded.SetValue(value, true);
                    }
                    else
                    {
                        Debug.LogError($"[{typeof(SaveSystem)}.LoadObject] Tried to load a [{objName}.{field.Name}] that wasn't previously saved. Mark it saveable and re-save");
                    }
                }
            }

            if (value == null)
                return;

            var fields = field.FieldType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var f in fields)
            {
                try
                {
                    var reursiveFieldValue = f.GetValue(value);

                    LoadFieldRecursive($"{objName}.{field.Name}", f, reursiveFieldValue, recursiveIndex + 1);
                }
                catch (TargetException)
                {
                    Debug.LogError($"[{nameof(SaveSystem)}.LoadFieldRecursive] Trying to get a value from null field. Field:'{objName}.{field.Name}'");
                }
            }
        }

        public void InitDeps()
        {
            fileManager = ServiceLocator.Get<IFileManager>();
            
#if UNITY_EDITOR

            if (fileManager == null)
                fileManager = new StandaloneFileManager();

#endif
        }

        public void Clean() { }
        public void Start() 
        {
            var loadableObjects = UnityEngine.Object.FindObjectsOfType<SaveableMonobehaviour>(true);

            foreach (var loadable in loadableObjects)
            {
                loadable.SubscribeForSaving(this);
            }
        }
    }
}