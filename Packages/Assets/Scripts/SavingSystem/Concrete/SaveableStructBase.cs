using UnityEngine;

namespace LinkedSquad.SavingSystem.Data
{
    [System.Serializable]
    public class SaveableStructBase<T> : SaveableBase<T> where T : struct
    {
        protected override T LoadData(object data)
        {
            var loaded = (T)data;

            return loaded;
        }

        protected override object SaveData()
        {
            var data = (object)Value;

            return data;
        }

        public static implicit operator T(SaveableStructBase<T> value) => value.Value;
    }

    [System.Serializable]
    public class SaveableFloat : SaveableStructBase<float> { }

    [System.Serializable]
    public class SaveableDouble : SaveableStructBase<double> { }

    [System.Serializable]
    public class SaveableInt : SaveableStructBase<int> { }

    [System.Serializable]
    public class SaveableShort : SaveableStructBase<short> { }

    [System.Serializable]
    public class SaveableUInt : SaveableStructBase<uint> { }

    [System.Serializable]
    public class SaveableLong : SaveableStructBase<long> { }

    [System.Serializable]
    public class SaveableULong : SaveableStructBase<ulong> { }

    [System.Serializable]
    public class SaveableBoolean : SaveableStructBase<bool> { }

    [System.Serializable]
    public class SaveableChar : SaveableStructBase<char> { }

    [System.Serializable]
    public class SaveableVector3 : SaveableStructBase<Vector3>
    {
        protected override Vector3 LoadData(object data)
        {
            var vec = (float[])data;

            return new Vector3(vec[0], vec[1], vec[2]);
        }

        protected override object SaveData()
        {
            var vec = new float[3];

            for (int i = 0; i < vec.Length; i++)
            {
                vec[i] = Value[i];
            }

            return vec;
        }

        public static Vector3 operator *(float value, SaveableVector3 vec3) => value * vec3.Value;
        public static Vector3 operator *(SaveableVector3 vec3, float value) => value * vec3.Value;
    }

    [System.Serializable]
    public class SaveableVector2 : SaveableStructBase<Vector2>
    {
        protected override Vector2 LoadData(object data)
        {
            var vec = (float[])data;

            return new Vector2(vec[0], vec[1]);
        }

        protected override object SaveData()
        {
            var vec = new float[2];

            for (int i = 0; i < vec.Length; i++)
            {
                vec[i] = Value[i];
            }

            return vec;
        }

        public static Vector2 operator *(float value, SaveableVector2 vec) => value * vec.Value;
        public static Vector2 operator *(SaveableVector2 vec, float value) => value * vec.Value;
    }

    [System.Serializable]
    public class SaveableQuaternion : SaveableStructBase<Quaternion>
    {
        protected override Quaternion LoadData(object data)
        {
            var quat = (float[])data;

            return new Quaternion(quat[0], quat[1], quat[2], quat[3]);
        }

        protected override object SaveData()
        {
            var vec = new float[4];

            for (int i = 0; i < vec.Length; i++)
            {
                vec[i] = Value[i];
            }

            return vec;
        }
        public static implicit operator Quaternion(SaveableQuaternion value) => value.Value;

        public static Vector3 operator *(SaveableQuaternion quat, Vector3 value) => quat.Value * value;
    }
}