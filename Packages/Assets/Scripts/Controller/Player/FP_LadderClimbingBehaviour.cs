using UnityEngine;

namespace LinkedSquad.PlayerControls
{
    public class FP_LadderClimbingBehaviour : MonoBehaviour
    {
        private const string LADDER_TAG = "Ladder";

        public bool IsClimbing { get; private set; }
        public float ClimbingSpeed => climbingSpeed;
        public float CameraDirectionCompensation => cameraDirectionCompensation;
        public float ClimbingVerticalCompensation => climbingVerticalCompensation;

        [SerializeField] private LayerMask ladderMask;
        [SerializeField] private float climbingVerticalCompensation = 0.15f;
        [SerializeField] private float climbingSpeed;
        [SerializeField] private float cameraDirectionCompensation;

        private CharacterController characterController;
        private Collider[] triggerCheckBuffer;



        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            triggerCheckBuffer = new Collider[1];
        }

        private void Update()
        {
            triggerCheckBuffer[0] = null;

            var bounds = characterController.bounds;

            var a = bounds.center - new Vector3(0, bounds.extents.y, 0);
            var b = bounds.center + new Vector3(0, bounds.extents.y, 0);
            var r = bounds.extents.x;

            Physics.OverlapCapsuleNonAlloc(a, b, r, triggerCheckBuffer, ladderMask, QueryTriggerInteraction.Collide);

            IsClimbing = triggerCheckBuffer[0] != null && triggerCheckBuffer[0].CompareTag(LADDER_TAG);
        }
    }
}