using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class FirebaseManager : MonoBehaviour {
    public static FirebaseManager instance { get; private set; } // Singleton
    private FirebaseDatabase _firebaseDatabase; // The Firebase Realtime Database
    private string _deviceId; // The device's unique identifier

    /**
     * <summary>Initializes the singleton</summary>
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeFirebase();
    }

    /**
     * <summary>Initializes the Firebase Realtime Database</summary>
     */
    private async void InitializeFirebase() {
        try {
            await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted && !task.IsFaulted) {
                    _firebaseDatabase = FirebaseDatabase.DefaultInstance;
                    _deviceId = SystemInfo.deviceUniqueIdentifier;
                } else {
                    print("Firebase initialization failed: " + task.Exception);
                }
            });
        
            FirebaseAIManager.instance.InitializeFirebaseAI();
        } catch (Exception) {
            // ignored
        }
    }

    /**
     * <summary>Submits an answer to the Firebase Realtime Database</summary>
     * <param name="quizId">The ID of the quiz</param>
     * <param name="questionId">The ID of the question</param>
     * <param name="isCorrect">Whether the answer was correct</param>
     */
    public async Task SubmitAnswer(string quizId, string questionId, bool isCorrect) {
        try {
            string userAttemptsPath = $"UserAttempts/{quizId}/{_deviceId}/Questions/{questionId}";
            Dictionary<string, object> attemptData = new Dictionary<string, object> {
                { "isCorrect", isCorrect },
                { "timestamp", ServerValue.Timestamp }
            };
            await _firebaseDatabase.GetReference(userAttemptsPath).SetValueAsync(attemptData);
        } catch (Exception e) {
            print($"Error submitting answer for {questionId}: {e.Message}");
        }
    }

    /**
     * <summary>Gets the percentage of correct answers for a question</summary>
     * <param name="quizId">The ID of the quiz</param>
     * <param name="questionId">The ID of the question</param>
     * <returns>The percentage of correct answers</returns>
     */
    public async Task<float> GetQuestionCorrectPercentage(string quizId, string questionId) {
        try {
            string attemptsPath = $"UserAttempts/{quizId}";
            DataSnapshot attemptsSnapshot = await _firebaseDatabase.GetReference(attemptsPath).GetValueAsync();
            if (!attemptsSnapshot.Exists || attemptsSnapshot.ChildrenCount == 0) return 0f;

            int correctAnswers = 0;
            int totalAnswers = 0;

            foreach (DataSnapshot deviceSnapshot in attemptsSnapshot.Children) {
                DataSnapshot questionSnapshot = deviceSnapshot.Child($"Questions/{questionId}");
                if (questionSnapshot.Exists) {
                    totalAnswers++;
                    if (Convert.ToBoolean(questionSnapshot.Child("isCorrect").Value)) correctAnswers++;
                }
            }
            
            return totalAnswers > 0 ? (float)correctAnswers / totalAnswers * 100f : 0f;
        } catch (Exception e) {
            print($"Error fetching percentage for {questionId}: {e.Message}");
            return -1f;
        }
    }

    /**
     * <summary>Submits a score to the Firebase Realtime Database</summary>
     * <param name="quizId">The ID of the quiz</param>
     * <param name="score">The score to submit</param>
     */
    public async Task SubmitScore(string quizId, int score) {
        try {
            string userScoresPath = $"UserScores/{quizId}/{_deviceId}";
            Dictionary<string, object> scoreData = new Dictionary<string, object> {
                { "currentScore", score },
                { "timestamp", ServerValue.Timestamp }
            };
            await _firebaseDatabase.GetReference(userScoresPath).SetValueAsync(scoreData);
        } catch (Exception e) {
            print($"Error submitting score for {quizId}: {e.Message}");
        }
    }

    /**
     * <summary>Gets the percentage of users with the same score</summary>
     * <param name="quizId">The ID of the quiz</param>
     * <param name="score">The score to get the percentage for</param>
     * <returns>The percentage of users with the same score</returns>
     */
    public async Task<float> GetScorePercentage(string quizId, int score) {
        try {
            string userScoresPath = $"UserScores/{quizId}";
            DataSnapshot userScoresSnapshot = await _firebaseDatabase.GetReference(userScoresPath).GetValueAsync();
            int totalUsers = userScoresSnapshot.ChildrenCount > 0 ? (int)userScoresSnapshot.ChildrenCount : 0;
            if (totalUsers == 0) return 0f;

            int sameScoreCount =
                userScoresSnapshot.Children.Count(user => Convert.ToInt32(user.Child("currentScore").Value) == score);
            return (float)sameScoreCount / totalUsers * 100f;
        } catch (Exception e) {
            print($"Error fetching percentage for {quizId}: {e.Message}");
            return -1f;
        }
    }

    /**
     * <summary>Submits a hint to the Firebase Realtime Database</summary>
     * <param name="symbol">The stone symbol to get a hint for</param>
     * <param name="hint">The hint to submit</param>
     */
    public async Task SubmitHintForSymbol(string symbol, string hint) {
        try {
            string language = LocaleSelector.instance.localeId == 0 ? "English" : "Hebrew";
            string hintsPath = $"Hints/{symbol}/{language}/hints";
            
            DataSnapshot hintsSnapshot = await _firebaseDatabase.GetReference(hintsPath).GetValueAsync();
            if (hintsSnapshot.Exists && hintsSnapshot.ChildrenCount > 0 && hintsSnapshot.Children
                    .Select(child => child.Child("hint").Value?.ToString()).Any(existingHint =>
                        string.Equals(existingHint, hint, StringComparison.OrdinalIgnoreCase)))
                return;
            
            DatabaseReference hintRef = _firebaseDatabase.GetReference(hintsPath).Push();
            Dictionary<string, object> hintData = new Dictionary<string, object> {
                { "hint", hint },
                { "timestamp", ServerValue.Timestamp }
            };
            await hintRef.SetValueAsync(hintData);
        } catch (Exception e) {
            print($"Error submitting hint for {symbol}: {e.Message}");
        }
    }
    
    /**
     * <summary>Gets a hint for a symbol from the Firebase Realtime Database</summary>
     * <param name="symbol">The stone symbol to get a hint for</param>
     * <returns>The hint for the symbol</returns>
     */
    public async Task<string> GetHintForSymbol(string symbol) {
        try {
            string language = LocaleSelector.instance.localeId == 0 ? "English" : "Hebrew";
            string hintsPath = $"Hints/{symbol}/{language}/hints";
        
            DataSnapshot hintsSnapshot = await _firebaseDatabase.GetReference(hintsPath).GetValueAsync();
            if (!hintsSnapshot.Exists || hintsSnapshot.ChildrenCount == 0) {
                print($"No hints found for {symbol} in {language}");
                return string.Empty;
            }

            List<string> hints = hintsSnapshot.Children.Select(child => child.Child("hint").Value?.ToString())
                .Where(hint => !string.IsNullOrEmpty(hint)).ToList();

            if (hints.Count <= 0) return string.Empty;
            
            int randomIndex = Random.Range(0, hints.Count);
            return hints[randomIndex];
        } catch (Exception e) {
            print($"Error fetching hint for {symbol}: {e.Message}");
            return string.Empty;
        }
    }
}