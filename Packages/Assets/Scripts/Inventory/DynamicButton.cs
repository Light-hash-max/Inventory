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
            if (Input.GetKeyDown(KeyCode.E)) // ��� ������� ������ E
            {
                // �������� ������ InventoryUI � ID ���������� ��������
                InventoryUI inventoryUI = scriptInventoryUI;
                string selectedItemId = inventoryUI.SelectedItemId;

                // �������� �� ������� ������
                foreach (var buttonData in dynamicButton)
                {
                    // ���������� ID ���������� �������� � nameItem
                    if (buttonData.nameItem == selectedItemId)
                    {
                        // ���� ���������, ��������� ������� OnUse
                        buttonData.OnUse.Invoke();
                    }
                }
            }
        }

        public void EnableButtons()
        {
            // �������� ������ InventoryUI � ID ���������� ��������
            InventoryUI inventoryUI = scriptInventoryUI;
            string selectedItemId = inventoryUI.SelectedItemId;

            // �������� �� ������� ������
            foreach (var buttonData in dynamicButton)
            {
                // ���������� ID ���������� �������� � nameItem
                if (buttonData.nameItem == selectedItemId)
                {
#if UNITY_STANDALONE
{
               
                    // ���� ���������, ���������� ��������������� ������
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
                    // ���� �� ���������, �������� ������
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
