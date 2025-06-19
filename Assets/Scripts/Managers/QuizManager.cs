using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour {
    private const float AnimationDuration = 0.5f; // Animation duration
    private const string FinalQuizMessageEnglish = "You answered {0}/{1} questions correctly!\n{2}% of participants got this score."; // Final quiz message
    private const string FinalQuizMessageHebrew = "ענית על {0}/{1} תשובות נכונות!\n%{2} מהמשתתפים קיבלו ציון זה."; // Final quiz message
    private const string QuestionStatsEnglish = "{0}% of participants answered correctly."; // Question stats
    private const string QuestionStatsHebrew = "%{0} מתוך המשתתפים ענו נכון."; // Question stats
    
    [SerializeField] private QuizDataSo quizData; // Quiz data
    [SerializeField] private MessagePanel messagePanel; // Message panel
    [SerializeField] private GameObject spinner; // Spinner
    [SerializeField] private Button menuButton; // Menu button
    
    [Header("Quiz Interfaces")]
    [SerializeField] private CanvasGroup initialQuizInterface; // Initial quiz interface
    [SerializeField] private CanvasGroup quizInterface; // Quiz interface
    [SerializeField] private CanvasGroup finalQuizInterface; // Final quiz interface

    [Header("Quiz Button Colors")] 
    [SerializeField] private Color baseColor = Color.white; // Base color
    [SerializeField] private Color correctColor = Color.green; // Correct color
    [SerializeField] private Color incorrectColor = Color.red; // Incorrect color
    
    [Header("Quiz UI Elements")] 
    [SerializeField] private TextMeshProUGUI questionNumber; // Question number
    [SerializeField] private TypeWriterEffect questionText; // Question text
    [SerializeField] private TextMeshProUGUI finalScoreText; // Final score text
    [SerializeField] private TextMeshProUGUI statsText; // Stats text
    [SerializeField] private CanvasGroup nextButton; // Next button
    [SerializeField] private Button repeatQuizButton; // Repeat quiz button
    [SerializeField] private TypeWriterEffect[] answers; // Answers
    [SerializeField] private Button[] answerButtons; // Answer buttons
    
    private readonly List<Image> _answerButtonImages = new(); // Answer button images
    private int _currentQuestionIndex, _correctAnswerIndex, _correctAnswerCount; // Statistics and current question
    private Coroutine _messagePanelCoroutine; // Message panel coroutine
    private string _quizId; // Quiz ID
    private float _answerPercentage, _scorePercentage; // Answer percentage and score percentage
    
    /**
     * <summary>Prepares states of all UI elements</summary>
     */
    private void Start() {
        _quizId = quizData.quizId;
        spinner.SetActive(false);
        
        ShowQuizInterface(initialQuizInterface, true);
        ShowQuizInterface(quizInterface, false);
        ShowQuizInterface(finalQuizInterface, false);
        
        LocaleSelector.OnLocaleChanged += ChangeContentPerLanguage;
        foreach (Button button in answerButtons)
            _answerButtonImages.Add(button.GetComponent<Image>());

        statsText.text = string.Empty;
    }

    /**
     * <summary>Starts the quiz (resets statistics and starts the first question)</summary>
     */
    public void StartQuiz() {
        nextButton.interactable = false;
        nextButton.alpha = 0;
        
        ShowQuizInterface(initialQuizInterface, false);
        ShowQuizInterface(finalQuizInterface, false);
        ShowQuizInterface(quizInterface, true);
        
        _currentQuestionIndex = -1;
        _correctAnswerCount = 0;
        _answerPercentage = -1;
        _scorePercentage = -1;
        SetNextQuestion();
    }

    /**
     * <summary>Sets the next question in the quiz</summary>
     */
    public void SetNextQuestion() {
        statsText.text = string.Empty;
            
        nextButton.interactable = false;
        if (SettingsPanel.instance.toggleAnimationsIsOn)
            nextButton.DOFade(0, AnimationDuration);
        else
            nextButton.alpha = 0;
            
        _currentQuestionIndex++;
        if (_currentQuestionIndex == quizData.questions.Length) {
            EndQuiz();
            return;
        }
        
        QuizDataSo.QuizQuestionPerLanguage question =
            quizData.questions[_currentQuestionIndex].questionsPerLanguage[LocaleSelector.instance.localeId];
        
        questionNumber.text = $"{_currentQuestionIndex + 1}/{quizData.questions.Length}";
        questionText.SetText(question.questionText);
        _correctAnswerIndex = quizData.questions[_currentQuestionIndex].correctAnswerIndex;
        
        for (int i = 0; i < answerButtons.Length; i++) {
            answers[i].SetText(question.answers[i]);
            if (SettingsPanel.instance.toggleAnimationsIsOn)
                _answerButtonImages[i].DOColor(baseColor, AnimationDuration);
            else 
                _answerButtonImages[i].color = baseColor;
            answerButtons[i].interactable = true;
        }
    }
    
    /**
     * <summary>Changes the content per language</summary>
     */
    private void ChangeContentPerLanguage(object sender, int localeId) {
        if (Mathf.Approximately(quizInterface.alpha, 1)) {
            QuizDataSo.QuizQuestionPerLanguage question =
                quizData.questions[_currentQuestionIndex].questionsPerLanguage[localeId];
        
            questionText.SetText(question.questionText);
            for (int i = 0; i < answers.Length; i++)
                answers[i].SetText(question.answers[i]);

            if (statsText.text == string.Empty) return;
            if (_answerPercentage >= 0) {
                string format = localeId == 0 ? QuestionStatsEnglish : QuestionStatsHebrew;
                string answerPercentage = localeId == 0
                    ? ((int)_answerPercentage).ToString()
                    : ReverseInt((int)_answerPercentage);
                statsText.text = string.Format(format, answerPercentage);
            }
        } else if (Mathf.Approximately(finalQuizInterface.alpha, 1)) {
            SetFinalScoreText(_scorePercentage);
        }
    }

    /**
     * <summary>Answers a question (a button is clicked)</summary>
     * <param name="answerIndex">The index of the answer</param>
     */
    public async void AnswerQuestion(int answerIndex) {
        try {
            spinner.SetActive(true);
            bool isCorrect = answerIndex == _correctAnswerIndex;
            if (isCorrect) _correctAnswerCount++;
        
            string questionId = $"Q{_currentQuestionIndex + 1}";
            await FirebaseManager.instance.SubmitAnswer(_quizId, questionId, isCorrect);

            _answerPercentage = await FirebaseManager.instance.GetQuestionCorrectPercentage(_quizId, questionId);
            spinner.SetActive(false);
            if (_answerPercentage >= 0) {
                string format = LocaleSelector.instance.localeId == 0 ? QuestionStatsEnglish : QuestionStatsHebrew;
                string answerPercentage = LocaleSelector.instance.localeId == 0
                    ? ((int)_answerPercentage).ToString()
                    : ReverseInt((int)_answerPercentage);
                statsText.text = string.Format(format, answerPercentage);
            }
            
            for (int i = 0; i < answerButtons.Length; i++) {
                answerButtons[i].interactable = false;
                Color buttonColor = i == _correctAnswerIndex ? correctColor : incorrectColor;
                if (SettingsPanel.instance.toggleAnimationsIsOn)
                    _answerButtonImages[i].DOColor(buttonColor, AnimationDuration);
                else 
                    _answerButtonImages[i].color = buttonColor;
            }
            
            if (isCorrect) AudioManager.instance.PlayCorrectAnswer();
            else AudioManager.instance.PlayWrongAnswer();

            nextButton.interactable = true;
            if (SettingsPanel.instance.toggleAnimationsIsOn)
                nextButton.DOFade(1f, AnimationDuration);
            else
                nextButton.alpha = 1f;
        } catch (Exception) {
            // ignored
        }
    }

    /**
     * <summary>Ends the quiz and shows the final score</summary>
     */
    private async void EndQuiz() {
        try {
            ShowQuizInterface(quizInterface, false);
            ShowQuizInterface(finalQuizInterface, true);
            finalScoreText.text = string.Empty;
            spinner.SetActive(true);

            await FirebaseManager.instance.SubmitScore(_quizId, _correctAnswerCount);
            _scorePercentage = await FirebaseManager.instance.GetScorePercentage(_quizId, _correctAnswerCount);
            
            spinner.SetActive(false);
            SetFinalScoreText(_scorePercentage);

            if (GameManager.instance.completedQuiz) return;
            repeatQuizButton.interactable = menuButton.interactable = false;
            GameManager.instance.completedQuiz = true;
            _messagePanelCoroutine ??= StartCoroutine(ShowMessagePanel());
        } catch (Exception) {
            // ignored
        }
    }

    /**
     * <summary>Shows the message panel</summary>
     */
    private IEnumerator ShowMessagePanel() {
        yield return new WaitForSeconds(0.5f);
        messagePanel.ShowPanel(true);
    }

    /**
     * <summary>Called when the message panel is closed</summary>
     */
    public void MessagePanelClosed() {
        if (!repeatQuizButton.interactable)
            repeatQuizButton.interactable = true;
        if (!menuButton.interactable)
            menuButton.interactable = true;
    }

    /**
     * <summary>Sets the final score text</summary>
     * <param name="scorePercentage">The score percentage</param>
     */
    private void SetFinalScoreText(float scorePercentage = -1f) {
        string format = LocaleSelector.instance.localeId == 0 ? FinalQuizMessageEnglish : FinalQuizMessageHebrew;
        string correctAnswerCount = LocaleSelector.instance.localeId == 0
            ? _correctAnswerCount.ToString()
            : ReverseInt(_correctAnswerCount);
        string questionCount = LocaleSelector.instance.localeId == 0
            ? quizData.questions.Length.ToString()
            : ReverseInt(quizData.questions.Length);
        string percentageText = scorePercentage >= 0
            ? LocaleSelector.instance.localeId == 0
                ? ((int)scorePercentage).ToString()
                : ReverseInt((int)scorePercentage)
            : LocaleSelector.instance.localeId == 0
                ? "No data"
                : "אין נתונים";
        finalScoreText.text = string.Format(format, correctAnswerCount, questionCount, percentageText);
    }

    /**
     * <summary>Shows or hides a canvas group</summary>
     * <param name="interfaceCanvas">The canvas group</param>
     * <param name="show">Whether to show or hide</param>
     */
    private static void ShowQuizInterface(CanvasGroup interfaceCanvas, bool show) {
        interfaceCanvas.alpha = show ? 1 : 0;
        interfaceCanvas.interactable = show;
        interfaceCanvas.blocksRaycasts = show;
    }

    /**
     * <summary>Reverses an integer</summary>
     * <param name="num">The integer to reverse</param>
     * <returns>The reversed integer as string</returns>
     */
    private static string ReverseInt(int num) {
        char[] array = num.ToString().ToCharArray();
        Array.Reverse(array);
        return new string(array);
    }

    /**
     * <summary>Unsubscribes from the locale changed event</summary>
     */
    private void OnDestroy() {
        LocaleSelector.OnLocaleChanged -= ChangeContentPerLanguage;
        _answerButtonImages.Clear();
        if (_messagePanelCoroutine != null) StopCoroutine(_messagePanelCoroutine);
    }
}