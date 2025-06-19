using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Information", menuName = "ScriptableObjects/Information", order = 0)]
public class InformationSo : ScriptableObject {
    public GameObject symbolPrefab; // The prefab of the symbol
    public RotationAxis rotationAxis; // The axis to rotate the symbol on click
    public string[] titles; // The names of the symbols in different languages
    public InformationDialogue[] informationDialogue; // The dialogues of the symbols in different languages

    [Serializable]
    public class InformationDialogue {
        public InformationDialoguePerLanguage[] informationDialoguePerLanguages; // The dialogues of the symbols in different languages
    }
    
    [Serializable]
    public class InformationDialoguePerLanguage {
        [TextArea(3, 7)] public string dialogueText; // The text of the dialogue
        public AudioClip dialogueAudioClip; // The audio clip of the dialogue
    }
}

/**
 * <summary>The axis to rotate the symbol on click</summary>
 */
public enum RotationAxis {
    Up, Forward, Right
}