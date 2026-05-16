using Blitz.Core;
using Blitz.Gameplay;
using Blitz.Gameplay.Navigation;
using UnityEngine;

namespace Blitz.App
{
    /// <summary>
    /// Starts an offline match; when the match ends, opens the results scene with the final score.
    /// </summary>
    public sealed class OfflineQuickStart : MonoBehaviour
    {
        [SerializeField] LocalMatchSession? session;
        [SerializeField] int totalRounds = 5;
        [SerializeField] int seed = 123;
        [SerializeField] float grabWindowSeconds = 3f;

        bool _matchEndHandled;

        void Start()
        {
            session ??= FindAnyObjectByType<LocalMatchSession>();
            if (session is null)
                return;

            session.StateChanged += OnSessionStateChanged;
            session.StartMatch(new MatchRules(totalRounds, grabWindowSeconds), seed);
        }

        void OnDestroy()
        {
            if (session != null)
                session.StateChanged -= OnSessionStateChanged;
        }

        void OnSessionStateChanged()
        {
            if (_matchEndHandled || session is null)
                return;

            if (session.Phase != MatchPhase.MatchEnd)
                return;

            _matchEndHandled = true;
            SceneFlow.LoadResults(session.Score);
        }
    }
}
