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
        ActiveOnomatopoeiaSet? ActiveSet { get; }
        float GrabTimeRemaining { get; }

        void StartMatch(MatchRules rules, int seed);

        void StartMatch(MatchRules rules, ActiveOnomatopoeiaSet activeSet, int cardGenSeed);

        void Tick(float deltaTime);

        bool TrySubmitGrab(SoundObjectId id);

        event Action? StateChanged;
    }
}
