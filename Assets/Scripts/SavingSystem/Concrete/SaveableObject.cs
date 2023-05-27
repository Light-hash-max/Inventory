using Dioinecail.ServiceLocator;
using System;
using UnityEngine;

namespace LinkedSquad.SavingSystem.Data
{
    [Serializable]
    public class SaveableParameters
    {
        public string Guid;
        public SaveableVector3 Position;
        public SaveableQuaternion Rotation;
        public SaveableBoolean IsActive;
        public SaveableBoolean IsEnabled;
        public SaveableBoolean IsParented;
        public SaveableBoolean WasDestroyed;
    }

    public class SaveableObject : SaveableMonobehaviour
    {
        [SerializeField] protected SaveableParameters saveableParameters;

        public string ObjectGuid => saveableParameters.Guid;



        public override void OnObjectLoaded()
        {
            base.OnObjectLoaded();

            HandleIsEnabledLoaded(saveableParameters.IsEnabled.Value);
            HandleIsActiveLoaded(saveableParameters.IsActive.Value);
            HandleRotationLoaded(saveableParameters.Rotation.Value);
            HandlePositionLoaded(saveableParameters.Position.Value);
            HandleWasDestroyedLoaded(saveableParameters.WasDestroyed.Value);
        }

        public override void SubscribeForSaving(ISaveSystem saveSystem)
        {
            saveSystem.OnDataLoaded += HandleLevelLoaded;

            SubscribeSaving();
        }

        private void SubscribeSaving()
        {
            saveableParameters.Position.OnBeforeDataSaved += OnBeforePositionSaved;
            saveableParameters.Rotation.OnBeforeDataSaved += OnBeforeRotationSaved;
            saveableParameters.IsActive.OnBeforeDataSaved += OnBeforeIsActiveSaved;
            saveableParameters.IsEnabled.OnBeforeDataSaved += OnBeforeIsEnabledSaved;
            saveableParameters.IsParented.OnBeforeDataSaved += OnBeforeIsParentedSaved;
        }

        private void HandleLevelLoaded()
        {
            HandlePositionLoaded(saveableParameters.Position.Value);
            HandleRotationLoaded(saveableParameters.Rotation.Value);
            HandleIsActiveLoaded(saveableParameters.IsActive.Value);
            HandleIsEnabledLoaded(saveableParameters.IsEnabled.Value);
            HandleIsParentedLoaded(saveableParameters.IsParented.Value);
            HandleWasDestroyedLoaded(saveableParameters.WasDestroyed.Value);
        }

        #region SAVING

        private void OnBeforePositionSaved()
        {
            saveableParameters.Position.Value = transform.position;
        }

        private void OnBeforeRotationSaved()
        {
            saveableParameters.Rotation.Value = transform.rotation;
        }

        private void OnBeforeIsActiveSaved()
        {
            saveableParameters.IsActive.Value = gameObject.activeSelf;
        }

        private void OnBeforeIsEnabledSaved()
        {
            saveableParameters.IsEnabled.Value = enabled;
        }

        private void OnBeforeIsParentedSaved()
        {
            saveableParameters.IsParented.Value = transform.parent != null;
        }

        #endregion

        #region LOADING

        private void HandleIsEnabledLoaded(bool state)
        {
            enabled = state;
        }

        private void HandleIsActiveLoaded(bool state)
        {
            gameObject.SetActive(state);
        }

        private void HandleRotationLoaded(Quaternion rot)
        {
            transform.rotation = rot;
        }

        private void HandlePositionLoaded(Vector3 pos)
        {
            if (TryGetComponent<CharacterController>(out var cc))
            {
                // FUCKING CHARACTER CONTROLLLLLLLLLEEEEER!!!!!
                cc.enabled = false;
                transform.position = pos;
                cc.enabled = true;
                return;
            }

            transform.position = pos;
        }

        private void HandleWasDestroyedLoaded(bool state)
        {
            saveableParameters.WasDestroyed.Value = state;

            if (state)
                Destroy(this);
        }

        private void HandleIsParentedLoaded(bool isParented)
        {
            if (!isParented)
                transform.SetParent(null);
        }

        #endregion

        private void OnDestroy()
        {
            saveableParameters.WasDestroyed.Value = true;

            var sm = ServiceLocator.Get<ISaveSystem>();

            if(sm != null)
                sm.SaveObject(this);
        }
    }
}