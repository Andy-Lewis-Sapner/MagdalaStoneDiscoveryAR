using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SynagogueSceneProgress : MonoBehaviour {
    [SerializeField] private InformationSo synagogueInformationSo; // Information about synagogue
    [SerializeField] private FadeOutBlackScreen fadeOutBlackScreen; // Fade out black screen
    [SerializeField] private Button continueTourButton; // Button to continue the tour

    /**
     * <summary>Starts the progress by subscribing to events</summary>
     * <returns>IEnumerator</returns>
     * */
    private IEnumerator Start() {
        continueTourButton.gameObject.SetActive(false);
        InformationManager.OnInformationPanelClosed += InformationManagerOnInformationPanelClosed;
        yield return new WaitUntil(() => !fadeOutBlackScreen.gameObject.activeSelf);
        InformationManager.instance.ShowInformation(synagogueInformationSo);
    }

    /**
     * <summary>Show the continue tour button</summary>
     */
    private void InformationManagerOnInformationPanelClosed(object sender, EventArgs e) {
        if (GameManager.instance.completedSynagogueTour) return;
        GameManager.instance.completedSynagogueTour = true;
        continueTourButton.gameObject.SetActive(true);
    }

    /**
     * <summary>Unsubscribes from events</summary>
     */
    private void OnDestroy() {
        InformationManager.OnInformationPanelClosed -= InformationManagerOnInformationPanelClosed;
    }
}