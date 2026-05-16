using System;
using System.Collections.Generic;
using Blitz.Core;
using Blitz.Gameplay.Content;
using Blitz.Gameplay.Table;
using UnityEngine;

namespace Blitz.Gameplay
{
    public sealed class LocalMatchSession : MonoBehaviour, IMatchSession, IOnomatopoeiaMatchContent
    {
        readonly RoundController _round = new();

        [SerializeField] OnomatopoeiaCatalog? _onomatopoeiaCatalog;
        [SerializeField] AudioSource? _cardCueAudio;

        Dictionary<OnomatopoeiaId, OnomatopoeiaDefinition>? _definitionsById;

        void Awake()
        {
            _round.RoundResolved += OnRoundResolved;
            _round.CardPrepared += OnCardPrepared;
        }

        void OnDestroy()
        {
            _round.CardPrepared -= OnCardPrepared;
            _round.RoundResolved -= OnRoundResolved;
        }

        public MatchPhase Phase => _round.Phase;

        public int CurrentRoundIndex => _round.CurrentRoundIndex;

        public int Score => _round.Score;

        public GeneratedCard? CurrentCard => _round.CurrentCard;

        public ActiveOnomatopoeiaSet? ActiveSet => _round.ActiveSet;

        public float GrabTimeRemaining => _round.GrabTimeRemaining;

        public event Action? StateChanged;

        public bool TryGetDefinition(OnomatopoeiaId id, out OnomatopoeiaDefinition? definition)
        {
            definition = null;
            return _definitionsById != null && _definitionsById.TryGetValue(id, out definition);
        }

        public void StartMatch(MatchRules rules, int seed)
        {
            var active = OnomatopoeiaMatchSampler.ResolveActiveSet(_onomatopoeiaCatalog, seed, out var map);
            _definitionsById = map;
            BeginMatchInternal(rules, active, seed);
        }

        public void StartMatch(MatchRules rules, ActiveOnomatopoeiaSet activeSet, int cardGenSeed)
        {
            _definitionsById = null;
            BeginMatchInternal(rules, activeSet, cardGenSeed);
        }

        void BeginMatchInternal(MatchRules rules, ActiveOnomatopoeiaSet activeSet, int cardGenSeed)
        {
            _round.BeginMatch(rules, activeSet, cardGenSeed);
            ApplyTableBindings(activeSet);
            StateChanged?.Invoke();
        }

        void ApplyTableBindings(ActiveOnomatopoeiaSet set)
        {
            var registry = FindAnyObjectByType<TableRuntimeRegistry>();
            registry?.ApplyMatchSlots(set, this);
        }

        void OnCardPrepared(GeneratedCard card)
        {
            if (!TryGetDefinition(card.CueOnomatopoeiaId, out var def) || def?.AudioClip is null)
                return;

            if (_cardCueAudio != null)
                _cardCueAudio.PlayOneShot(def.AudioClip);
            else
                AudioSource.PlayClipAtPoint(def.AudioClip, Vector3.zero);
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
