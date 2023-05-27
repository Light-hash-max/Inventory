 using UnityEngine;
using LinkedSquad.SavingSystem.Data;
using LinkedSquad.Interactions;

namespace LinkedSquad.InventorySystem
{
    // 'physical' representation of an Item in the Game World
    [RequireComponent(typeof(Interactable), typeof(SaveableObject))]
    public class Item : MonoBehaviour
    {
        [Header("������ � ��������")]

        [Tooltip("�������� ��� �������������� � ������� ��������� (���������������, �������)")]
        public string id;
        [Tooltip("�������� �������� ��� ���������")]
        public string itemName;
        public Sprite inventoryIcon;
        [Tooltip("������� ��� ����� ������������ �������. -1 ���� ����������")]
        public SaveableInt usageCount = new SaveableInt() { Value = -1 };

        private Rigidbody _itemRigidBody;
        public Rigidbody ItemRigidBody
        {
            get
            {
                if (_itemRigidBody == null)
                    _itemRigidBody = GetComponent<Rigidbody>();

                return _itemRigidBody;
            }
        }

        private SaveableObject _saveableObject;
        public SaveableObject SaveableObject
        {
            get
            {
                if (_saveableObject == null)
                    _saveableObject = GetComponent<SaveableObject>();

                return _saveableObject;
            }
        }



        public bool Use()
        {
            usageCount.Value--;

            return usageCount.Value == 0;
        }
    }
}