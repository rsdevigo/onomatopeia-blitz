using System;
using Blitz.Core;
using Blitz.Gameplay.Content;
using Blitz.Gameplay.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blitz.Gameplay.Minigames
{
    /// <summary>Owns offline match flow: additive minigame scene + <see cref="IMinigame"/> lifecycle.</summary>
    public sealed class OfflineMinigameOrchestrator : MonoBehaviour
    {
        [SerializeField] MinigameCatalog? _catalog;
        [SerializeField] DifficultyCatalog? _difficultyCatalog;
        [SerializeField] MinigameServicesHost? _servicesHost;
        [SerializeField] LocalMatchSession? _session;
        [SerializeField] int _defaultSeed = 123;

        readonly MinigameLoader _loader = new();

        IMinigame? _minigame;
        MinigameDescriptor? _descriptor;
        MinigameServices? _services;
        bool _matchEndHandled;

        async void Start()
        {
            _session ??= FindAnyObjectByType<LocalMatchSession>();
            _servicesHost ??= FindAnyObjectByType<MinigameServicesHost>();

            if (_catalog is null || _session is null || _servicesHost is null)
            {
                Debug.LogError("OfflineMinigameOrchestrator: missing catalog, session, or services host.");
                return;
            }

            var minigameId = PlayerPrefs.GetString(
                GameSessionPrefs.SelectedMinigameId,
                MinigameIds.BlitzOnomatopoeico);

            if (!_catalog.TryGet(minigameId, out _descriptor) || _descriptor is null)
            {
                Debug.LogError($"OfflineMinigameOrchestrator: unknown minigame id '{minigameId}'.");
                return;
            }

            _services = _servicesHost.Build(_descriptor.UseBlitzGrabDriver);

            var scene = await _loader.LoadAsync(_descriptor);
            if (!scene.IsValid())
                return;

            _minigame = FindMinigameInScene(scene);
            if (_minigame is null)
            {
                Debug.LogError($"OfflineMinigameOrchestrator: no IMinigame in '{_descriptor.AdditiveSceneName}'.");
                return;
            }

            _session.CardPrepared += OnCardPrepared;
            _session.RoundResolved += OnRoundResolved;
            _session.StateChanged += OnSessionStateChanged;

            _minigame.OnRegister(_services);
            _minigame.OnSceneLoaded();

            var profile = ResolveDifficultyProfile();
            var rules = profile != null
                ? profile.ToMatchRules()
                : new MatchRules(5, 3f);
            var seed = _defaultSeed + (profile?.MatchSeedOffset ?? 0);
            _minigame.OnMatchBegin(new MatchConfig(rules, seed));
        }

        void OnDestroy()
        {
            if (_session != null)
            {
                _session.CardPrepared -= OnCardPrepared;
                _session.RoundResolved -= OnRoundResolved;
                _session.StateChanged -= OnSessionStateChanged;
            }

            if (_minigame != null)
            {
                _minigame.OnUnregister();
                _minigame = null;
            }

            _loader.UnloadImmediate();
        }

        static IMinigame? FindMinigameInScene(Scene scene)
        {
            if (!scene.IsValid())
                return null;

            foreach (var root in scene.GetRootGameObjects())
            {
                var found = root.GetComponentInChildren<IMinigame>(true);
                if (found != null)
                    return found;
            }

            return null;
        }

        DifficultyProfile? ResolveDifficultyProfile()
        {
            var id = PlayerPrefs.GetString(GameSessionPrefs.SelectedDifficultyId, DifficultyIds.Easy);

            if (_difficultyCatalog != null && _difficultyCatalog.TryGet(id, out var profile) && profile != null)
                return profile;

            if (_difficultyCatalog != null && _difficultyCatalog.Entries.Count > 0 && _difficultyCatalog.Entries[0] != null)
            {
                Debug.LogWarning($"OfflineMinigameOrchestrator: unknown difficulty '{id}', using first catalog entry.");
                return _difficultyCatalog.Entries[0];
            }

            Debug.LogWarning("OfflineMinigameOrchestrator: no DifficultyCatalog; using built-in easy defaults.");
            return null;
        }

        void OnCardPrepared(GeneratedCard card)
        {
            if (_minigame is null || _session?.ActiveSet is not { } set)
                return;

            _minigame.OnRoundBegin(card, set);
        }

        void OnRoundResolved(RoundOutcome outcome) => _minigame?.OnRoundEnd(outcome);

        void OnSessionStateChanged()
        {
            if (_matchEndHandled || _session is null)
                return;

            if (_session.Phase != MatchPhase.MatchEnd)
                return;

            _matchEndHandled = true;
            _minigame?.OnMatchEnd();
            SceneFlow.LoadResults(_session.Score);
        }
    }
}
