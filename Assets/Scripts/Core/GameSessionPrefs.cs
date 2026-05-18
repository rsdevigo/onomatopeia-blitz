namespace Blitz.Core
{
    /// <summary>PlayerPrefs keys for cross-scene session data (menu → gameplay → results).</summary>
    public static class GameSessionPrefs
    {
        public const string PlayerName = "Blitz.PlayerName";
        public const string SelectedDifficultyId = "Blitz.SelectedDifficultyId";

        /// <summary>Legacy menu index (0–2). Migrated once to <see cref="SelectedDifficultyId"/>.</summary>
        public const string DifficultyIndex = "Blitz.DifficultyIndex";
        public const string PendingResultsScore = "Blitz.PendingResultsScore";
        public const string SelectedMinigameId = "Blitz.SelectedMinigameId";
    }
}
