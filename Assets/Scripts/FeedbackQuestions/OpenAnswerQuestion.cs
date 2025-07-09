using TMPro;
using UnityEngine;

public class OpenAnswerQuestion : FeedbackQuestionTemplate {
    [SerializeField] private TMP_InputField answerField; // Answer
    
    /**
     * <summary>Returns the answer for the question</summary>
     */
    public override FeedbackAnswer GetAnswer() {
        return new OpenAnswer {
            Question = FeedbackQuestionSo.questionInDifferentLanguages[0],
            Answer = answerField.text
        };
    }
    
    /**
     * <summary>Initializes the components of the question</summary>
     */
    protected override void InitializeComponents() {}
}

/// <summary>Open answer</summary>
public class OpenAnswer : FeedbackAnswer {
    public string Answer;
}