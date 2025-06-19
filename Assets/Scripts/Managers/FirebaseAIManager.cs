using System;
using System.Threading.Tasks;
using Firebase.AI;
using UnityEngine;

public class FirebaseAIManager : MonoBehaviour {
    public static FirebaseAIManager instance { get; private set; } // Singleton
    
    private const string ModelName = "gemini-2.0-flash"; // The name of the GenAI model
    private const string ModelContentInText =
        "You are a helpful assistant, capable of giving one sentence hints in a json format for the user " +
        "to get a better understanding of a symbol on the magdala stone."; // The content of the GenAI model
    private const string SymbolPrompt =
        "Please provide, in one sentence, a description of a {0}, without mentioning the word {0} in it in the {1} language. " +
        "The answer must be in a json containing the key \"description\" and the value must be the sentence in the {1} language" +
        "and it mustn't contain the word {0} in this language. Thanks!"; // The prompt for the GenAI model
    
    public bool modelInitialized { get; private set; } // Whether the GenAI model is initialized
    private GenerativeModel _geminiModel; // The GenAI model

    /**
     * <summary>Initializes the singleton</summary>
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /**
     * <summary>Initializes the GenAI model, called after the firebase is initialized</summary>
     */
    public void InitializeFirebaseAI() {
        FirebaseAI ai = FirebaseAI.GetInstance(FirebaseAI.Backend.GoogleAI());
        _geminiModel = ai.GetGenerativeModel(
            modelName: ModelName,
            systemInstruction: ModelContent.Text(ModelContentInText)
        );
        
        modelInitialized = true;
    }

    /**
     * <param name="symbol">The stone symbol to get a hint for</param>
     * <summary>Requests a hint for a symbol from the GenAI model</summary>
     */
    public async Task<string> RequestHint(string symbol) {
        string prompt = string.Format(SymbolPrompt, symbol, LocaleSelector.instance.localeId == 0 ? "English" : "Hebrew");
        try {
            Chat chat = _geminiModel.StartChat();
            GenerateContentResponse response = await chat.SendMessageAsync(prompt);
            string jsonResponse = response.Text?.Trim() ?? "";
            jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "").Trim();
            ModelJsonResponse responseJson = JsonUtility.FromJson<ModelJsonResponse>(jsonResponse);
            string hint = responseJson.description;
            // Submit the hint to the database, so it can be used later if the request fails
            await FirebaseManager.instance.SubmitHintForSymbol(symbol, hint);
            return hint;
        } catch (Exception) {
            // If the request fails, return a hint from the database
            string hint = await FirebaseManager.instance.GetHintForSymbol(symbol);
            if (hint == string.Empty)
                return LocaleSelector.instance.localeId == 0
                    ? "There was an error, please try again"
                    : "אירעה שגיאה, נסו שוב";
            return hint;
        }
    }

    /**
     * <summary>Represents a json response from the GenAI model</summary>
     */
    [Serializable]
    private class ModelJsonResponse {
        public string description;
    }
}