using UnityEngine;
using Vuforia;

public class ModelTrackingStatus : MonoBehaviour {
    public bool isTracking { get; private set; } // True if the model is being tracked
    
    private ObserverBehaviour _observer; // The observer that tracks the model
    private bool _lastTrackingStatus; // The last tracking status
    private int _currentGuideViewIndex; // The index of the current guide view
    
    public Transform trackedModel { get; private set; } // The model that is being tracked
    private static string modelTracked => LocaleSelector.instance.localeId == 0 ? "Stone is tracked" : "האבן זוהתה"; // The text to display when the model is tracked
    private static string modelNotTracked => LocaleSelector.instance.localeId == 0 ? "Stone is not tracked" : "האבן לא מזוהה"; // The text to display when the model is not tracked

    /**
     * <summary>Starts the tracking status</summary>
     */
    private void Start() {
        _observer = GetComponent<ObserverBehaviour>();
        if (_observer) _observer.OnTargetStatusChanged += OnStatusChanged;
    }

    /**
     * <summary>Updates the tracking status</summary>
     * <param name="behaviour">The observer that tracks the model</param>
     * <param name="status">The status of the model</param>
     */
    private void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status) {
        _lastTrackingStatus = isTracking;
        isTracking = status.Status is Status.TRACKED or Status.EXTENDED_TRACKED;
        if (_lastTrackingStatus == isTracking) return;

        if (NotificationPanel.instance)
            NotificationPanel.instance.SetNotification(isTracking ? modelTracked : modelNotTracked);

        if (isTracking) {
            StoneSceneProgress.instance?.StartProgress();
            trackedModel = behaviour.transform;
        } else {
            trackedModel = null;
        }
    }

    /**
     * <summary>Unsubscribes from the target status changed event</summary>
     */
    private void OnDestroy() {
        if (_observer) _observer.OnTargetStatusChanged -= OnStatusChanged;
    }
}