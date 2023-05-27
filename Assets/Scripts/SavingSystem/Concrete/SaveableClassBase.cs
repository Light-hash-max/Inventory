using System.Collections.Generic;

namespace LinkedSquad.SavingSystem.Data
{
    [System.Serializable]
    public class SaveableClassBase<T> : SaveableBase<T> where T : class
    {
        protected override T LoadData(object data)
        {
            var loaded = (T)data;

            return loaded;
        }

        protected override object SaveData()
        {
            var data = (object)Value;

            return data;
        }
    }

    [System.Serializable]
    public class SaveableString : SaveableClassBase<string> { }

    [System.Serializable]
    public class SaveableArray<T> : SaveableClassBase<T[]> { }

    [System.Serializable]
    public class SaveableList<T> : SaveableClassBase<List<T>> { }
}