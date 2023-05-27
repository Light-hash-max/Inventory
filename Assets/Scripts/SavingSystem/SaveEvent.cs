using UnityEngine;
using Dioinecail.ServiceLocator;

namespace LinkedSquad.SavingSystem
{
    public class SaveEvent : MonoBehaviour
    {
        [ContextMenu("Save")]
        public void Save()
        {
            var saveSystem = ServiceLocator.Get<ISaveSystem>();

            saveSystem.Save();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            var saveSystem = ServiceLocator.Get<ISaveSystem>();

            saveSystem.Load();
        }
    }
}