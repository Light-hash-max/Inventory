using UnityEngine;

namespace LinkedSquad.SavingSystem.Data
{
    // just an interface to find such objects inside the scene
    public abstract class SaveableMonobehaviour : MonoBehaviour
    {
        public virtual void OnObjectLoaded() { }
        public virtual void SubscribeForSaving(ISaveSystem saveSystem) { }
    }
}