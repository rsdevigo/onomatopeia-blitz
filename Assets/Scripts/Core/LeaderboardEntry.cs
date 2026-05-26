using System;

namespace Blitz.Core
{
    /// <summary>One persisted leaderboard row (JSON-serializable via public fields).</summary>
    [Serializable]
    public struct LeaderboardEntry
    {
        public string Name;
        public int Score;
        public string MinigameId;
        public string DifficultyId;
        public string UtcIso;

        public LeaderboardEntry(
            string name,
            int score,
            string minigameId,
            string difficultyId,
            DateTime utc)
        {
            Name = name ?? string.Empty;
            Score = score;
            MinigameId = minigameId ?? string.Empty;
            DifficultyId = difficultyId ?? string.Empty;
            UtcIso = utc.ToUniversalTime().ToString("o");
        }
    }
}
