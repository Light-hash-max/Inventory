using LinkedSquad.InventorySystem;
using System.Linq;
using UnityEngine;

namespace LinkedSquad.Interactions
{
    [System.Serializable]
    public class InteractionMessage
    {
        public ItemId itemId;
        public string message;
    }

    public class MessageInteraction : MonoBehaviour
    {
        public float messageDuration = 3f;

        public InteractionMessage[] messages;

        private Interactable _targetInteractable;



        private void Awake()
        {
            _targetInteractable = GetComponent<Interactable>();
        }

        private void OnEnable()
        {
            _targetInteractable.OnInteract += OnInteract;
        }

        private void OnDisable()
        {
            _targetInteractable.OnInteract -= OnInteract;
        }

        private void OnValidate()
        {
            if(messages == null || messages.Length == 0)
            {
                messages = new InteractionMessage[1]
                {
                    new InteractionMessage() { itemId = new ItemId() { Id = string.Empty }, message = string.Empty }
                };
            }
        }

        private void OnInteract()
        {
            if (_targetInteractable.IsLocked)
            {
                var itemId = InventoryUI.Instance.SelectedItemId;
                var message = string.Empty;

                if (messages.Any (m => m.itemId.Equals(itemId)))
                {
                    message = messages.First(m => m.itemId.Equals(itemId)).message;
                }
                else
                {
                    message = messages.First(m => m.itemId.Equals(string.Empty)).message;
                }

                InventoryMessageController.Instance.DisplayMessage(message, messageDuration);
            }
        }
    }
}