using Dioinecail.ServiceLocator;
using LinkedSquad.SavingSystem.Data;
using System;

namespace LinkedSquad.SavingSystem
{
    public interface ISaveSystem : IService
    {
        event Action OnDataSaved;
        event Action OnDataLoaded;

        void Save();
        void Load();
        void SaveObject(SaveableMonobehaviour target);
        void LoadObject(SaveableMonobehaviour target);
    }
}