using UnityEngine;

[CreateAssetMenu(fileName = "FeedbackQuestion", menuName = "ScriptableObjects/FeedbackQuestion")] 
public class FeedbackQuestionSo : ScriptableObject {
    public string[] questionInDifferentLanguages; // The question in different languages
    public FeedbackQuestionType feedbackQuestionType; // The type of the feedback question
}

// The different types of the feedback question
public enum FeedbackQuestionType {
    YesNo, Rating, Open
}