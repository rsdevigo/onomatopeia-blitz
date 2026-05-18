namespace Blitz.Gameplay.Navigation
{
    /// <summary>Build Settings scene names (must match assets under <c>Assets/Scenes/</c>).</summary>
    public static class SceneNames
    {
        public const string MainMenu = "10_MainMenu";
        public const string GameplayCore = "30_Gameplay_Core";
        public const string MinigameBlitz = "31_Minigame_Blitz";
        public const string MinigameFantasma = "32_Minigame_Fantasma";

        [System.Obsolete("Use GameplayCore + additive minigame scenes.")]
        public const string GameplayOffline = "30_Gameplay_Offline";

        public const string Results = "40_Results";
        public const string Leaderboard = "50_Leaderboard";
    }
}
