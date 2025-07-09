using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YesNoQuestion : FeedbackQuestionTemplate {
    [SerializeField] private Toggle yesToggle; // Yes toggle
    [SerializeField] private Toggle noToggle; // No toggle
    [SerializeField] private TMP_InputField suggestionInputField; // Suggestion input field

    /** <summary>Returns the answer for the question</summary> */
    public override FeedbackAnswer GetAnswer() {
        return new YesNoAnswer {
            Question = FeedbackQuestionSo.questionInDifferentLanguages[0],
            Answer = yesToggle.isOn,
            Suggestion = suggestionInputField.text
        };
    }
    
    /** <summary>Initializes the components of the question</summary> */
    protected override void InitializeComponents() {
        yesToggle.isOn = noToggle.isOn = false;
    }
}

/// <summary>Yes or no answer</summary> 
public class YesNoAnswer : FeedbackAnswer {
    public bool Answer;
    public string Suggestion;
}