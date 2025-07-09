using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatingQuestion : FeedbackQuestionTemplate {
    [SerializeField] private Slider ratingSlider; // rating from 1 to 5
    [SerializeField] private TextMeshProUGUI ratingText; // rating from 1 to 5 text
    [SerializeField] private TMP_InputField suggestionField; // suggestion input
    
    /**
     * <summary>Returns the answer for the question</summary>
     */
    public override FeedbackAnswer GetAnswer() {
        return new RatingAnswer {
            Question = FeedbackQuestionSo.questionInDifferentLanguages[0],
            Rating = (int)ratingSlider.value,
            Suggestion = suggestionField.text
        };
    }
    
    /**
     * <summary>Initializes the components of the question</summary>
     */
    protected override void InitializeComponents() {
        ratingText.text = "5";
        ratingSlider.value = 5;
        
        ratingSlider.onValueChanged.AddListener(_ =>OnSliderValueChanged());
    }

    /**
     * <summary>Updates the rating text</summary>
     */
    private void OnSliderValueChanged() {
        ratingText.text = ratingSlider.value.ToString(CultureInfo.CurrentCulture);
    }

    /**
     * <summary>Destroys the object</summary>
     */
    protected override void OnDestroy() {
        base.OnDestroy();
        ratingSlider.onValueChanged.RemoveListener(_ => OnSliderValueChanged());
    }
}

/// <summary> Rating answer </summary>
public class RatingAnswer : FeedbackAnswer {
    public int Rating;
    public string Suggestion;
}