using Blitz.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blitz.Gameplay.Navigation
{
    /// <summary>Loads UI/gameplay scenes in single mode. Index 0 in Build Settings should be <see cref="SceneNames.MainMenu"/>.</summary>
    public static class SceneFlow
    {
        public static void LoadMainMenu() =>
            SceneManager.LoadSceneAsync(SceneNames.MainMenu, LoadSceneMode.Single);

        public static void LoadOfflineGame() =>
            SceneManager.LoadSceneAsync(SceneNames.GameplayOffline, LoadSceneMode.Single);

        public static void LoadLeaderboard() =>
            SceneManager.LoadSceneAsync(SceneNames.Leaderboard, LoadSceneMode.Single);

        public static void LoadResults(int finalScore)
        {
            PlayerPrefs.SetInt(GameSessionPrefs.PendingResultsScore, finalScore);
            PlayerPrefs.Save();
            SceneManager.LoadSceneAsync(SceneNames.Results, LoadSceneMode.Single);
        }
    }
}
