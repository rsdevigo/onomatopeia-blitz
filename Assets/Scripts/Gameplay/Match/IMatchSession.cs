using System;
using Blitz.Core;

namespace Blitz.Gameplay
{
    public interface IMatchSession
    {
        MatchPhase Phase { get; }
        int CurrentRoundIndex { get; }
        int Score { get; }
        GeneratedCard? CurrentCard { get; }
        ActiveLetterSoundSet? ActiveSet { get; }
        float GrabTimeRemaining { get; }

        void StartMatch(MatchRules rules, int seed);

        void Tick(float deltaTime);

        bool TrySubmitGrab(SoundObjectId id);

        event Action? StateChanged;
    }
}
