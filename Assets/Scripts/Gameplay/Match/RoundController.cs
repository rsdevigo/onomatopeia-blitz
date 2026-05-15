using System;
using Blitz.Core;

namespace Blitz.Gameplay
{
    public sealed class RoundController
    {
        readonly IAnswerResolver _resolver = new AnswerResolver();

        public MatchPhase Phase { get; private set; } = MatchPhase.MatchInit;

        public int CurrentRoundIndex { get; private set; }

        public int Score { get; private set; }

        public GeneratedCard? CurrentCard { get; private set; }

        public ActiveLetterSoundSet? ActiveSet { get; private set; }

        public float GrabTimeRemaining { get; private set; }

        MatchRules _rules;

        CardGenerator _generator = null!;

        float _phaseTimer;

        SoundObjectId? _grab;

        bool _grabConsumed;

        public event Action<RoundOutcome>? RoundResolved;

        public void BeginMatch(MatchRules rules, int seed)
        {
            _rules = rules;
            _generator = new CardGenerator(seed);
            CurrentRoundIndex = 0;
            Score = 0;
            Phase = MatchPhase.MatchInit;
            _phaseTimer = 0f;
            _grab = null;
            _grabConsumed = false;
            CurrentCard = null;
            ActiveSet = null;
            GrabTimeRemaining = 0f;
        }

        public void Tick(float deltaTime)
        {
            switch (Phase)
            {
                case MatchPhase.MatchInit:
                    StartNextRoundFromInit();
                    break;

                case MatchPhase.RoundPrepare:
                    PrepareRoundContent();
                    Phase = MatchPhase.RoundPresent;
                    _phaseTimer = 0f;
                    break;

                case MatchPhase.RoundPresent:
                    _phaseTimer += deltaTime;
                    if (_phaseTimer >= 0.35f)
                    {
                        Phase = MatchPhase.GrabPhase;
                        _phaseTimer = 0f;
                        GrabTimeRemaining = _rules.GrabWindowSeconds;
                    }

                    break;

                case MatchPhase.GrabPhase:
                    _phaseTimer += deltaTime;
                    GrabTimeRemaining = MathF.Max(0f, _rules.GrabWindowSeconds - _phaseTimer);

                    if (_grab.HasValue && !_grabConsumed)
                    {
                        _grabConsumed = true;
                        Phase = MatchPhase.SpeakPhase;
                        _phaseTimer = 0f;
                        break;
                    }

                    if (_phaseTimer >= _rules.GrabWindowSeconds)
                        EnterResolve(claimed: null);

                    break;

                case MatchPhase.SpeakPhase:
                    _phaseTimer += deltaTime;
                    if (_phaseTimer >= 0.25f)
                        EnterResolve(_grab);

                    break;

                case MatchPhase.ResolveRound:
                    _phaseTimer += deltaTime;
                    if (_phaseTimer >= 0.05f)
                        FinishResolveAndAdvance();

                    break;

                case MatchPhase.MatchEnd:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Phase), Phase, null);
            }
        }

        public bool TryRegisterGrab(SoundObjectId id)
        {
            if (Phase != MatchPhase.GrabPhase || _grabConsumed)
                return false;

            _grab = id;
            return true;
        }

        void StartNextRoundFromInit()
        {
            Phase = MatchPhase.RoundPrepare;
            _phaseTimer = 0f;
            _grab = null;
            _grabConsumed = false;
        }

        void PrepareRoundContent()
        {
            if (!_generator.TryGenerate(out var result))
                throw new InvalidOperationException("Card generation failed.");

            CurrentCard = result.Card;
            ActiveSet = result.ActiveSet;
            _grab = null;
            _grabConsumed = false;
        }

        void EnterResolve(SoundObjectId? claimed)
        {
            if (Phase != MatchPhase.GrabPhase && Phase != MatchPhase.SpeakPhase)
                return;

            if (CurrentCard is null || ActiveSet is null)
                throw new InvalidOperationException("Resolve without active card.");

            var card = CurrentCard.Value;
            var set = ActiveSet.Value;
            var correct = _resolver.Resolve(card, set);

            var won = claimed.HasValue && claimed.Value == correct;
            if (won)
                Score += 1;

            var submitted = claimed ?? new SoundObjectId(0);
            RoundResolved?.Invoke(new RoundOutcome(won, submitted, correct));

            Phase = MatchPhase.ResolveRound;
            _phaseTimer = 0f;
        }

        void FinishResolveAndAdvance()
        {
            CurrentRoundIndex++;
            if (CurrentRoundIndex >= _rules.TotalRounds)
            {
                Phase = MatchPhase.MatchEnd;
                return;
            }

            Phase = MatchPhase.RoundPrepare;
            _phaseTimer = 0f;
        }
    }
}
