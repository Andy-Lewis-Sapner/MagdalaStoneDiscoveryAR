using UnityEngine;
using UnityEngine.InputSystem;

public class ModelTargetTouchHandler : MonoBehaviour {
    [SerializeField] private ModelTrackingStatus trackingStatus; // For checking if model is being tracked
    [SerializeField] private Camera mainCamera; // For raycast

    private InputAction _tapAction; // For input
    private Vector3 _difference; // For raycast
    private string _currentDirection; // For raycast

    /**
     * <summary>Subscribes to input</summary>
     */
    private void OnEnable() {
        _tapAction = new InputAction(type: InputActionType.PassThrough, binding: "<Pointer>/press");
        _tapAction.performed += OnTapPreformed;
        _tapAction.Enable();
    }

    /**
     * <summary>Unsubscribes from input</summary>
     */
    private void OnDisable() {
        _tapAction.Disable();
        _tapAction.performed -= OnTapPreformed;
    }

    /**
     * <summary>Handles the tap</summary>
     * <param name="obj">The input</param>
     */
    private void OnTapPreformed(InputAction.CallbackContext obj) {
        if (!trackingStatus || !trackingStatus.isTracking) return;
        if (SettingsPanel.instance.isPanelVisible || HangmanController.instance.isPanelVisible) return;

        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        CheckHit(ray);
    }

    /**
     * <summary>Checks if the ray hit a symbol</summary>
     * <param name="ray">The ray</param>
     */
    private void CheckHit(Ray ray) {
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;
        if (!hit.collider.TryGetComponent(out StoneSymbol stoneSymbol)) return;
        stoneSymbol.ShowInfoAboutSymbol(mainCamera.transform.position, mainCamera.transform.forward);
        AudioManager.instance.PlayButtonClickSound();
    }
}