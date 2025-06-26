using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {
    [SerializeField] private GameObject loadingSpinner; // The loading spinner object

    /**
     * <summary>Deactivates the loading spinner</summary>
     */
    private void Start() {
        if (loadingSpinner) loadingSpinner.SetActive(false);
    }

    /**
     * <summary>Loads the specified scene</summary>
     * <param name="scene">The name of the scene to load</param>
     */
    public void LoadScene(string scene) {
        if (!Enum.TryParse(scene, out SceneNames sceneName)) return;
        if (loadingSpinner) loadingSpinner.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName.ToString()));
    }

    /**
     * <summary>Loads the specified scene asynchronously</summary>
     * <param name="scene">The name of the scene to load</param>
     */
    private static IEnumerator LoadSceneAsync(string scene) {
        yield return SceneManager.LoadSceneAsync(scene);
    }

    /**
     * <summary>Exits the game</summary>
     */
    public void ExitGame() { 
        Application.Quit();
    }
}

/**
 * <summary>The names of the scenes in the game</summary>
 */
public enum SceneNames {
    MenuScene, StoneSceneAR, StoneScene3D, QuizScene, SynagogueScene
}