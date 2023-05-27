using Dioinecail.ButtonUtil;
using System.Collections.Generic;
using UnityEngine;

namespace LinkedSquad.Utility
{
    [RequireComponent(typeof(LineRenderer))]
    public class TrajectoryRenderer : MonoBehaviour
    {
        private const float CALCULATION_PRECISION = 0.03f;
        private const int MAX_CALCULATION_COUNT = 600;

        [SerializeField] private int _pointCount = 30;

        private LineRenderer _line;

        //[SerializeField] private Rigidbody _targetBody;
        //[SerializeField] private Vector3 DEBUG_throwDirection;



        public bool Draw(Vector3 origin, Vector3 direction, float force, float mass, out Vector3 hitPoint)
        {
            var result = false;
            hitPoint = Vector3.zero;

            _line.positionCount = _pointCount;
            direction.Normalize();

            var position = origin;
            var speed = force / mass;
            var gravity = Physics.gravity;

            var positions = new List<Vector3>();
            positions.Add(position);

            var velocity = direction * speed;
            var velocityXZ = direction * speed;
            velocityXZ.y = 0f;

            for (int i = 1; i < MAX_CALCULATION_COUNT; i++)
            {
                var height = Height(origin.y, velocity.y, gravity.y, i * CALCULATION_PRECISION);
                var xz = Distance(origin, velocityXZ, i * CALCULATION_PRECISION);

                position = new Vector3(xz.x, height, xz.z);

                positions.Add(position);

                var oldPos = positions[i - 1];
                var raycastDir = position - oldPos;

                if (Physics.Raycast(oldPos, raycastDir.normalized, out var hit, raycastDir.magnitude))
                {
                    hitPoint = hit.point;
                    result = true;
                    break;
                }
            }

            var totalPoints = positions.Count;

            _line.positionCount = totalPoints;
            for (int i = 0; i < totalPoints; i++)
            {
                _line.SetPosition(i, positions[i]);
            }

            return result;
        }

        public void Show()
        {
            _line.enabled = true;
        }

        public void Hide()
        {
            _line.enabled = false;
        }

        private float Height(float y0, float v0, float gravity, float time)
        {
            var v0t = v0 * time;
            var gt2 = gravity * time * time / 2f;

            return y0 + v0t + gt2;
        }

        private Vector3 Distance(Vector3 origin, Vector3 v0, float time)
        {
            return origin + (v0 * time);
        }

#if UNITY_EDITOR

        //[Button]
        //public void Throw()
        //{
        //    if (_targetBody == null || !Application.isPlaying)
        //        return;

        //    _targetBody.position = transform.position;
        //    _targetBody.velocity = Vector3.zero;
        //    _targetBody.angularVelocity = Vector3.zero;

        //    var direction = DEBUG_throwDirection.normalized;
        //    var force = direction * (InventorySystem.Inventory.Instance.ThrowPower / _targetBody.mass);

        //    _targetBody.velocity = force;
        //}

#endif

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
        }

        private void OnValidate()
        {
            if (_line == null)
                _line = GetComponent<LineRenderer>();

            if (_pointCount < 3)
                _pointCount = 3;
        }

        //private void OnDrawGizmos()
        //{
        //    if (_targetBody != null && InventorySystem.Inventory.Instance != null)
        //        Draw(transform.position, DEBUG_throwDirection, InventorySystem.Inventory.Instance.ThrowPower, _targetBody.mass, out var hit);
        //}
    }
}