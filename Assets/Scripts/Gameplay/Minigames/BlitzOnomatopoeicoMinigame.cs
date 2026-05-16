using Blitz.Core;
using UnityEngine;

namespace Blitz.Gameplay.Minigames
{
    /// <summary>
    /// Default table minigame: uses core <see cref="AnswerResolver"/> slots directly.
    /// </summary>
    public sealed class BlitzOnomatopoeicoMinigame : MonoBehaviour, IMinigame
    {
        [SerializeField] LocalMatchSession? session;

        public void OnRegister(MinigameServices services)
        {
        }

        public void OnSceneLoaded() => session ??= FindAnyObjectByType<LocalMatchSession>();

        public void OnMatchBegin(MatchConfig config) => session?.StartMatch(config.Rules, config.Seed);

        public void OnRoundBegin(GeneratedCard card, ActiveOnomatopoeiaSet set)
        {
        }

        public void OnRoundEnd(RoundOutcome outcome)
        {
        }

        public void OnMatchEnd()
        {
        }

        public void OnUnregister()
        {
        }
    }
}
