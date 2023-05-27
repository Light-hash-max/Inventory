using UnityEngine;

namespace LinkedSquad.Interactions
{
    public interface IBreakable
    {
        void TryBreak(float mass, Vector3 direction);
    }
}