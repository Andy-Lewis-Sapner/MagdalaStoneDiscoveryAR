using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class FeedbackQuestionnaireManager : MonoBehaviour {
    [SerializeField] private FeedbackQuestionSo[] feedbackQuestions; // All feedback questions
    [SerializeField] private CanvasGroup feedbackCanvasGroup; // The canvas group of the feedback screen
    [SerializeField] private Transform questionsContainer; // The container for the feedback questions
    
    [Header("Feedback Questions Templates")]
    [SerializeField] private GameObject yesNoQuestionTemplate; // The template for the yes/no question
    [SerializeField] private GameObject ratingQuestionTemplate; // The template for the rating question
    [SerializeField] private GameObject openAnswerQuestionTemplate; // The template for the open answer question
    
    private readonly List<FeedbackQuestionTemplate> _questions = new(); // The list of generated feedback questions

    /**
     * <summary>Hides the feedback questionnaire</summary>
     */
    private void Start() {
        feedbackCanvasGroup.alpha = 0f;
        feedbackCanvasGroup.interactable = false;
        feedbackCanvasGroup.blocksRaycasts = false;
    }

    /**
     * <summary>Opens the feedback questionnaire by generating the feedback questions</summary>
     */
    public void OpenFeedbackScreen() {
        EmptyQuestionsContainer();
        
        if (SettingsPanel.instance.toggleAnimationsIsOn)
            feedbackCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutBack);
        else
            feedbackCanvasGroup.alpha = 1f;
        
        feedbackCanvasGroup.interactable = true;
        feedbackCanvasGroup.blocksRaycasts = true;

        _questions.Clear();
        foreach (FeedbackQuestionSo feedbackQuestion in feedbackQuestions) {
            GameObject template = SwitchTypeToTemplate(feedbackQuestion.feedbackQuestionType);
            if (!template) continue;

            FeedbackQuestionTemplate questionTemplate =
                Instantiate(template, questionsContainer).GetComponent<FeedbackQuestionTemplate>();
            questionTemplate.SetQuestion(feedbackQuestion);
            _questions.Add(questionTemplate);
        }
    }

    /**
     * <summary>Submits the answers to the Firebase Realtime Database</summary>
     */
    public async void SubmitAnswers() {
        try {
            List<FeedbackAnswer> answers = _questions.Select(questionTemplate => questionTemplate.GetAnswer()).ToList();
            await FirebaseManager.instance.SubmitFeedback(answers);
            CloseFeedbackScreen();
        } catch (Exception) {
            // ignored
        }
    }
    
    /**
     * <summary>Closes the feedback questionnaire</summary>
     */
    public void CloseFeedbackScreen() {
        if (SettingsPanel.instance.toggleAnimationsIsOn)
            feedbackCanvasGroup.DOFade(0, 0.5f).SetEase(Ease.InBack);
        else
            feedbackCanvasGroup.alpha = 0f;
        
        feedbackCanvasGroup.interactable = false;
        feedbackCanvasGroup.blocksRaycasts = false;
    }

    /**
     * <summary>Switches the type of the feedback question to the corresponding template</summary>
     * <param name="type">The type of the feedback question</param>
     * <returns>The template for the feedback question</returns>
     */
    private GameObject SwitchTypeToTemplate(FeedbackQuestionType type) {
        return type switch {
            FeedbackQuestionType.YesNo => yesNoQuestionTemplate,
            FeedbackQuestionType.Rating => ratingQuestionTemplate,
            FeedbackQuestionType.Open => openAnswerQuestionTemplate,
            _ => null
        };
    }

    /**
     * <summary>Empties the questions container</summary>
     */
    private void EmptyQuestionsContainer() {
        foreach (Transform child in questionsContainer) {
            Destroy(child.gameObject);
        }
    }
}