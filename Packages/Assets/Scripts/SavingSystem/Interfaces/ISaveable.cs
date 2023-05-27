using System;

namespace LinkedSquad.SavingSystem
{
    public interface ISaveable
    {
        bool IsLoaded { get; set; }

        object Save();
        void Load(object value);
    }

    public interface ISaveable<T> : ISaveable
    {
        event Action OnBeforeDataSaved;
        event Action OnAfterDataSaved;
    }
}