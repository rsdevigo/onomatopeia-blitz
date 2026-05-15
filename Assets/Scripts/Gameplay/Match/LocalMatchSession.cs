using System;
using Blitz.Core;
using UnityEngine;

namespace Blitz.Gameplay
{
    public sealed class LocalMatchSession : MonoBehaviour, IMatchSession
    {
        readonly RoundController _round = new();

        void Awake() => _round.RoundResolved += OnRoundResolved;

        void OnDestroy() => _round.RoundResolved -= OnRoundResolved;

        public MatchPhase Phase => _round.Phase;

        public int CurrentRoundIndex => _round.CurrentRoundIndex;

        public int Score => _round.Score;

        public GeneratedCard? CurrentCard => _round.CurrentCard;

        public ActiveLetterSoundSet? ActiveSet => _round.ActiveSet;

        public float GrabTimeRemaining => _round.GrabTimeRemaining;

        public event Action? StateChanged;

        public void StartMatch(MatchRules rules, int seed)
        {
            _round.BeginMatch(rules, seed);
            StateChanged?.Invoke();
        }

        void Update() => Tick(Time.deltaTime);

        public void Tick(float deltaTime)
        {
            var phaseBefore = _round.Phase;
            _round.Tick(deltaTime);
            if (phaseBefore != _round.Phase || _round.Phase == MatchPhase.GrabPhase)
                StateChanged?.Invoke();
        }

        public bool TrySubmitGrab(SoundObjectId id)
        {
            var ok = _round.TryRegisterGrab(id);
            if (ok)
                StateChanged?.Invoke();
            return ok;
        }

        void OnRoundResolved(RoundOutcome _) => StateChanged?.Invoke();
    }
}
