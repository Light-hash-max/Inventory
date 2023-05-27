using UnityEngine;
using UnityEngine.Events;


namespace LinkedSquad.InventorySystem
{

    [System.Serializable]
    public class DynamicButtonData
    {
        public string nameItem;
        public GameObject buttonPC;
        public GameObject buttonMobile;
        public UnityEvent OnUse;
    }

    public class DynamicButton : MonoBehaviour
    {
        public InventoryUI scriptInventoryUI;
        public DynamicButtonData[] dynamicButton;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E)) // При нажатии кнопки E
            {
                // получаем объект InventoryUI и ID выбранного предмета
                InventoryUI inventoryUI = scriptInventoryUI;
                string selectedItemId = inventoryUI.SelectedItemId;

                // проходим по массиву кнопок
                foreach (var buttonData in dynamicButton)
                {
                    // сравниваем ID выбранного предмета с nameItem
                    if (buttonData.nameItem == selectedItemId)
                    {
                        // если совпадают, выполняем событие OnUse
                        buttonData.OnUse.Invoke();
                    }
                }
            }
        }

        public void EnableButtons()
        {
            // получаем объект InventoryUI и ID выбранного предмета
            InventoryUI inventoryUI = scriptInventoryUI;
            string selectedItemId = inventoryUI.SelectedItemId;

            // проходим по массиву кнопок
            foreach (var buttonData in dynamicButton)
            {
                // сравниваем ID выбранного предмета с nameItem
                if (buttonData.nameItem == selectedItemId)
                {
#if UNITY_STANDALONE
{
               
                    // если совпадают, отображаем соответствующую кнопку
                    if (buttonData.buttonPC != null)
                    {
                        buttonData.buttonPC.SetActive(true);
                    }
}
#endif
#if UNITY_ANDROID || UNITY_IOS
{
                        if (buttonData.buttonMobile != null)
                    {
                        buttonData.buttonMobile.SetActive(true);
                    }
}
#endif

            }
                else
                {
                    // если не совпадают, скрываем кнопку
                    if (buttonData.buttonPC != null)
                    {
                        buttonData.buttonPC.SetActive(false);
                    }

                    if (buttonData.buttonMobile != null)
                    {
                        buttonData.buttonMobile.SetActive(false);
                    }
                }
            }
        }
    }
}
