using Blitz.Core;
using UnityEngine;

namespace Blitz.Gameplay.Content
{
    [CreateAssetMenu(menuName = "Blitz/Difficulty Profile", fileName = "DifficultyProfile")]
    public sealed class DifficultyProfile : ScriptableObject
    {
        [SerializeField] string _difficultyId = DifficultyIds.Easy;
        [SerializeField] string _displayName = "Fácil";
        [SerializeField] int _totalRounds = 5;
        [SerializeField] float _grabWindowSeconds = 3f;
        [SerializeField] int _matchSeedOffset;

        public string DifficultyId => _difficultyId;

        public string DisplayName => _displayName;

        public int TotalRounds => _totalRounds;

        public float GrabWindowSeconds => _grabWindowSeconds;

        public int MatchSeedOffset => _matchSeedOffset;

        public MatchRules ToMatchRules() =>
            new(
                Mathf.Clamp(_totalRounds, 1, CardGenerator.MaxDistinctCardsPerMatch),
                Mathf.Max(0.5f, _grabWindowSeconds));
    }
}
