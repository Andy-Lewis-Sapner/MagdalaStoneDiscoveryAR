using TMPro;
using UnityEngine;

public class NotificationPanel : MonoBehaviour {
    public static NotificationPanel instance { get; private set; } // Singleton

    [SerializeField] private GameObject notificationPanel; // Notification panel
    [SerializeField] private TextMeshProUGUI notificationText; // Notification text

    private bool _isNotificationActive; // Whether a notification is currently active
    
    /**
     * <summary>Initializes the singleton</summary>
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
    }

    /**
     * <summary>Initializes the notification panel</summary>
     */
    private void Start() {
        notificationPanel.SetActive(false);
    }

    /**
     * <summary>Sets the notification text and shows the notification panel</summary>
     * <param name="notification">The notification text</param>
     */
    public void SetNotification(string notification) {
        if (_isNotificationActive) return;
        
        notificationText.text = notification;
        notificationPanel.SetActive(true);
        _isNotificationActive = true;
        Invoke(nameof(CloseNotification), 2f);
    }
    
    /**
     * <summary>Closes the notification panel</summary>
     */
    private void CloseNotification() {
        notificationPanel.SetActive(false);
        _isNotificationActive = false;
    }
}
