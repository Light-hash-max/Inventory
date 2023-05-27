using System;

namespace LinkedSquad.InventorySystem
{
    [Serializable]
    public class ItemId : IEquatable<string>, IEquatable<ItemId>
    {
        public string Id;



        public static implicit operator string(ItemId id)
        {
            return id.Id;
        }

        public static implicit operator ItemId(string id)
        {
            return new ItemId() { Id = id };
        }

        public bool Equals(string other)
        {
            return string.Equals(Id, other);
        }

        public bool Equals(ItemId other)
        {
            return string.Equals(Id, other.Id);
        }
    }
}