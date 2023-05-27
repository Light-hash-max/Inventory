using LinkedSquad.Interactions.Effects;
using LinkedSquad.PlayerControls;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LinkedSquad.Interactions
{
    public class InteractionRaycaster : MonoBehaviour
    {
        public Transform raycastOrigin;
        public FP_Input input;
        public float raycastDistance;
        public LayerMask raycastMask;
        public float touchInputHeldPropagation = 0.5f;

        private RaycastHit raycastBuffer;
        private IInteractable _currentlyHoveredInteractable;
        private HoverHighlight _currentlyHoveredHighlight;



        private void Awake()
        {
            if (input.UseMobileInput)
            {
                var lookPad = FindObjectOfType<FP_Lookpad>();

                lookPad.OnPointerUpEvent += OnPointerUp;
            }
        }

        private void Update()
        {
            if (!input.UseMobileInput)
            {
                ProcessRaycast();
                ProcessInputs();
            }
        }

        private void ProcessRaycast()
        {
            raycastBuffer = new RaycastHit();

            Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out raycastBuffer, raycastDistance, raycastMask);

            ProcessHighlightRaycast(raycastBuffer);
            ProcessInteractableRaycast(raycastBuffer);
        }

        private void ProcessInputs()
        {
            if (Input.GetMouseButtonDown(0) && !input.InventoryButton())
            {
                _currentlyHoveredInteractable?.Interact();
            }
        }

        private void ProcessHighlightRaycast(RaycastHit raycast)
        {
            if (raycastBuffer.collider != null)
            {
                if (raycastBuffer.collider.TryGetComponent<HoverHighlight>(out var hoverableObject))
                {
                    if (_currentlyHoveredInteractable != null
                        && _currentlyHoveredHighlight != hoverableObject)
                    {
                        _currentlyHoveredHighlight.OnUnhover();
                        _currentlyHoveredHighlight = null;
                    }

                    if (_currentlyHoveredHighlight == null)
                    {
                        _currentlyHoveredHighlight = hoverableObject;
                        _currentlyHoveredHighlight.OnHover();
                    }
                }
                else
                {
                    _currentlyHoveredHighlight?.OnUnhover();
                    _currentlyHoveredHighlight = null;
                }
            }
            else
            {
                _currentlyHoveredHighlight?.OnUnhover();
                _currentlyHoveredHighlight = null;
            }
        }

        private void ProcessInteractableRaycast(RaycastHit raycast)
        {
            if (raycastBuffer.collider != null)
            {
                if (raycastBuffer.collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    _currentlyHoveredInteractable = interactable;

                    return;
                }
            }

            _currentlyHoveredInteractable = null;
        }

        private void OnPointerUp(PointerEventData data, float heldTime)
        {
            if (heldTime > touchInputHeldPropagation)
                return;

            var touchData = Camera.main.ScreenPointToRay(data.position);

            Physics.Raycast(touchData.origin, touchData.direction, out raycastBuffer, raycastDistance, raycastMask);

            if (raycastBuffer.collider != null)
            {
                if (raycastBuffer.collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.Interact();
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position + raycastOrigin.forward * raycastDistance);
        }
    }
}