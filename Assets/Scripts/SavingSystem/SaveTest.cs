using UnityEngine;
using LinkedSquad.SavingSystem.Data;
using Dioinecail.ServiceLocator;

namespace LinkedSquad.SavingSystem.Debugger
{
    public class SaveTest : SaveableMonobehaviour
    {
        [SerializeField] private SaveableFloat floatValue;
        [SerializeField] private SaveableDouble doubleValue;
        [SerializeField] private SaveableInt intValue;
        [SerializeField] private SaveableUInt uintValue;
        [SerializeField] private SaveableLong longValue;
        [SerializeField] private SaveableULong uLongValue;
        [SerializeField] private SaveableBoolean boolValue;
        [SerializeField] private SaveableChar charValue;
        [SerializeField] private SaveableVector3 vec3Value;



        [ContextMenu("Save")]
        private void Save()
        {
            var saveSystem = ServiceLocator.Get<ISaveSystem>();

            saveSystem.Save();
        }

        [ContextMenu("Load")]
        private void Load()
        {
            var saveSystem = ServiceLocator.Get<ISaveSystem>();

            saveSystem.Load();
        }
    }
}