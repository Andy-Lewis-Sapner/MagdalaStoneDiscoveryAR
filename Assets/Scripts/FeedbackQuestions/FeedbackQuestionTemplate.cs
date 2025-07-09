using TMPro;
using UnityEngine;

public abstract class FeedbackQuestionTemplate : MonoBehaviour {
    [SerializeField] protected TextMeshProUGUI questionText; // The question
    protected FeedbackQuestionSo FeedbackQuestionSo; // The question object

    public abstract FeedbackAnswer GetAnswer(); // Returns the answer
    protected abstract void InitializeComponents(); // Initializes the components

    /**
     * <summary>Initializes the components</summary>
     */
    private void Start() {
        LocaleSelector.OnLocaleChanged += LocaleSelectorOnLocaleChanged;
        InitializeComponents();
    }

    /**
     * <summary>Changes the question based on the selected locale</summary>
     */
    private void LocaleSelectorOnLocaleChanged(object sender, int localeId) {
        questionText.text = FeedbackQuestionSo.questionInDifferentLanguages[localeId];
    }

    /**
     * <summary>Sets the question</summary>
     * <param name="questionSo">The question object</param>
     */
    public void SetQuestion(FeedbackQuestionSo questionSo) {
        FeedbackQuestionSo = questionSo;
        questionText.text = questionSo.questionInDifferentLanguages[LocaleSelector.instance.localeId];
    }
    
    /**
     * <summary>Destroys the object</summary>
     */
    protected virtual void OnDestroy() {
        LocaleSelector.OnLocaleChanged -= LocaleSelectorOnLocaleChanged;
    }
}

/// <summary>Base class for feedback answers</summary>
public class FeedbackAnswer {
    public string Question;
}