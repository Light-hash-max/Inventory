using System;
using UnityEngine;

namespace LinkedSquad.SavingSystem.Data
{
    [Serializable]
    public abstract class SaveableBase<T> : ISaveable<T>
    {
        public event Action OnBeforeDataSaved;
        public event Action OnAfterDataSaved;
        public event Action<T> OnDataLoaded;

        public string Guid;
        public T Value;
        public bool IsSave = true;
        public bool IsLoaded { get; set; }



        public virtual void Load(object value)
        {
            var data = LoadData(value);

            this.Value = data;

            OnDataLoaded?.Invoke(data);
        }

        public virtual object Save()
        {
            OnBeforeDataSaved?.Invoke();

            var data = SaveData();

            OnAfterDataSaved?.Invoke();

            return data;
        }

        protected abstract T LoadData(object data);
        protected abstract object SaveData();
    }
}