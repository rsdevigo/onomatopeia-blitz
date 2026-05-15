using Blitz.Core;
using Blitz.Gameplay;
using UnityEngine;

namespace Blitz.App
{
    /// <summary>
    /// Minimal composition hook: starts an offline match for quick iteration.
    /// </summary>
    public sealed class OfflineQuickStart : MonoBehaviour
    {
        [SerializeField] LocalMatchSession? session;
        [SerializeField] int totalRounds = 5;
        [SerializeField] int seed = 123;
        [SerializeField] float grabWindowSeconds = 3f;

        void Start()
        {
            session ??= FindAnyObjectByType<LocalMatchSession>();
            if (session is null)
                return;

            session.StartMatch(new MatchRules(totalRounds, grabWindowSeconds), seed);
        }
    }
}
