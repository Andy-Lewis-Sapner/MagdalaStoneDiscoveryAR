using UnityEngine;
using UnityEngine.UI;

public class MenuSceneProgress : MonoBehaviour {
    [SerializeField] private Button stoneTourButton; // Stone Tour
    [SerializeField] private Button quizButton; // Quiz

    /**
     * <summary>Sets the visibility of the buttons based on the game progress</summary>
     */
    private void Start() {
        SetButtonsVisibility();
    }

    /**
     * <summary>Sets the visibility of the buttons based on the game progress</summary>
     */
    private void SetButtonsVisibility() {
        stoneTourButton.gameObject.SetActive(GameManager.instance.completedSynagogueTour);
        quizButton.gameObject.SetActive(GameManager.instance.completedStoneTour);
    }

    /**
     * <summary>Resets the game progress and retrieves the saved states</summary>
     */
    public void HandleResetProgress() {
        GameManager.instance.ResetProgressAndRetrieveStates();
        SetButtonsVisibility();
    }
}