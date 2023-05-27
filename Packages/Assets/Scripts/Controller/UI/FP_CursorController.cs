using UnityEngine;

namespace LinkedSquad.PlayerControls
{
    public class FP_CursorController : MonoBehaviour
    {
        [Header("dependency")]
        [SerializeField]
        private GameObject _playerController;
        private FP_Input _playerInput;
        private FP_CameraLook _playerCameraLook;
        private void Awake()
        {
            _playerInput = _playerController.GetComponent<FP_Input>();
            _playerCameraLook = _playerController.GetComponent<FP_CameraLook>();
        }

        private void OnEnable()
        {
            if (_playerInput.UseMobileInput)
            {
                return;
            }
            HideCursor();
        }
        private void OnDisable()
        {
            if (_playerInput.UseMobileInput)
            {
                return;
            }
            ShowCursor();
        }

        private void Update()
        {
            if (_playerInput.UseMobileInput)
            {
                return;
            }
            ProcessSpaceBar();
        }
        private void ProcessSpaceBar()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShowCursor();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                HideCursor();
            }
        }

        private void ShowCursor()
        {
            if (_playerCameraLook == null)
            {
                return;
            }
            //_playerCameraLook.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void HideCursor()
        {
            if (_playerCameraLook == null)
            {
                return;
            }
            //_playerCameraLook.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}