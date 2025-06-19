using DG.Tweening;
using UnityEngine;

public class MessagePanel : MonoBehaviour {
    private const float AnimationDuration = 1f; // Animation duration
    private readonly Vector2 _closedPosition = new(0f, -1500); // Closed position
    private readonly Vector2 _openPosition = Vector2.zero; // Open position
    
    [SerializeField] private RectTransform panel; // Reference to the panel
    
    /**
     * <summary>Initializes the singleton</summary>
     */
    private void Start() {
        panel.anchoredPosition = _closedPosition;
    }

    /**
     * <summary>Shows or hides the panel</summary>
     * <param name="show">Whether to show or hide the panel</param>
     */
    public void ShowPanel(bool show) {
        if (show) panel.DOAnchorPos(_openPosition, AnimationDuration).SetEase(Ease.OutBack);
        else panel.DOAnchorPos(_closedPosition, AnimationDuration).SetEase(Ease.InBack);
    }
}