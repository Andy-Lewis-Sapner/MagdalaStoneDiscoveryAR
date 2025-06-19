using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Quiz", menuName = "ScriptableObjects/Quiz", order = 0)]
public class QuizDataSo : ScriptableObject {
    public string quizId; // The ID of the quiz
    public QuizQuestion[] questions; // The questions in the quiz

    [Serializable]
    public class QuizQuestion {
        public QuizQuestionPerLanguage[] questionsPerLanguage; // The questions in different languages
        [Range(0, 3)] public int correctAnswerIndex; // The index of the correct answer
    }
    
    [Serializable]
    public class QuizQuestionPerLanguage {
        [TextArea(2, 5)] public string questionText; // The text of the question
        [TextArea(1, 5)] public string[] answers; // The answers to the question
    }
}