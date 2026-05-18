using Blitz.Core;
using UnityEngine;

namespace Blitz.Gameplay.Minigames
{
    /// <summary>
    /// Default table minigame: Blitz grab is handled by <see cref="Input.OfflineGrabInputDriver"/> on the core scene
    /// (enabled via <see cref="MinigameDescriptor.UseBlitzGrabDriver"/>).
    /// </summary>
    public sealed class BlitzOnomatopoeicoMinigame : MonoBehaviour, IMinigame
    {
        [SerializeField] LocalMatchSession? session;

        MinigameServices _services = MinigameServices.Empty;

        public void OnRegister(MinigameServices services) => _services = services;

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

        public void OnUnregister() => _services = MinigameServices.Empty;
    }
}
