using Dioinecail.ButtonUtil;
using Dioinecail.ServiceLocator;
using UnityEngine;

namespace LinkedSquad.SavingSystem.Debugger
{
    public class SaveDebugger : MonoBehaviour
    {
        [Button]
        public void Save()
        {
            var sm = ServiceLocator.Get<ISaveSystem>();

            sm.Save();
        }

        [Button]
        public void Load()
        {
            var sm = ServiceLocator.Get<ISaveSystem>();

            sm.Load();
        }
    }
}