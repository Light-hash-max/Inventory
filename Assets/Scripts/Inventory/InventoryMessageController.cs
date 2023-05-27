using LinkedSquad.SavingSystem.Data;
using System.Collections;
using TMPro;
using UnityEngine;

namespace LinkedSquad.InventorySystem
{
    public class InventoryMessageController : SaveableMonobehaviour
    {
        public static InventoryMessageController Instance;

        public TMP_Text itemInfoMessage;

        private Coroutine _coroutineMesasge;




        public void DisplayMessage(string message, float duration)
        {
            if (_coroutineMesasge != null)
                StopCoroutine(Instance._coroutineMesasge);
            _coroutineMesasge = StartCoroutine(CoroutineItemSelected(message, duration));
        }

        private void Awake()
        {
            Instance = this;
        }

        private IEnumerator CoroutineItemSelected(string message, float duration)
        {
            itemInfoMessage.text = message;

            yield return new WaitForSeconds(duration);

            itemInfoMessage.text = string.Empty;

            _coroutineMesasge = null;
        }
    }
}