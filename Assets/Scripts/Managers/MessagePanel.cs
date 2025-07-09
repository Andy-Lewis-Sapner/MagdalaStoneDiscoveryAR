using DG.Tweening;
using UnityEngine;

public class MessagePanel : MonoBehaviour {
    private const float AnimationDuration = 1f; // Animation duration
    private readonly Vector2 _closedPosition = new(0f, -1500); // Closed position
    private readonly Vector2 _openPosition = Vector2.zero; // Open position
    // URL to the Beit Igal Alon Center website
    private const string MagdalaStoneURL =
        "https://yigal-allon-centre.org.il/%D7%9E%D7%95%D7%96%D7%99%D7%90%D7%95%D7%9F-%D7%90%D7%93%D7%9D-%D7%91%D7%92%D7%9C%D7%99%D7%9C/%D7%AA%D7%A2%D7%A8%D7%95%D7%9B%D7%95%D7%AA-%D7%94%D7%91%D7%99%D7%AA-2/";
    
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

    /**
     * <summary>Opens the Igal Alon Center website that features the Magdala Stone</summary>
     */
    public void OpenMagdalaStoneURL() {
        Application.OpenURL(MagdalaStoneURL);
    }
}