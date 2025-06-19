using DG.Tweening;
using UnityEngine;

public class FadeOutBlackScreen : MonoBehaviour {
    private const float AnimationDuration = 1.5f; // Animation duration
    [SerializeField] private CanvasGroup fadeOutCanvasGroup; // Reference to the fade out canvas group
    private Tween _fadeOutTween; // The fade out tween
    
    /**
     * <summary>Starts the fade out animation</summary>
     */
    private void Start() {
        _fadeOutTween = fadeOutCanvasGroup.DOFade(0f, AnimationDuration)
            .OnComplete(() => fadeOutCanvasGroup.gameObject.SetActive(false));
    }

    /**
     * <summary>Stops the fade out animation if it is running</summary>
     */
    private void OnDestroy() {
        _fadeOutTween?.Kill();
    }
}