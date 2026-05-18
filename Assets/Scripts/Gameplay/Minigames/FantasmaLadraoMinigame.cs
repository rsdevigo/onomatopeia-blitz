using Blitz.Core;
using UnityEngine;

namespace Blitz.Gameplay.Minigames
{
    /// <summary>
    /// Fantasma variant: world picks go through <see cref="TrySubmitWorldGrab"/> → <see cref="IInputRouter"/>.
    /// The core <see cref="Input.OfflineGrabInputDriver"/> stays disabled; use <see cref="FantasmaWorldGrabInput"/> in this scene.
    /// </summary>
    public sealed class FantasmaLadraoMinigame : MonoBehaviour, IMinigame
    {
        [SerializeField] LocalMatchSession? session;

        MinigameServices _services = MinigameServices.Empty;
        ISolutionSpaceAdapter _adapter = IdentitySolutionSpaceAdapter.Instance;

        public void OnRegister(MinigameServices services)
        {
            _services = services;
            _adapter = IdentitySolutionSpaceAdapter.Instance;
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

        public void OnUnregister() => _services = MinigameServices.Empty;

        public bool TrySubmitWorldGrab(SoundObjectId worldSlot)
        {
            var coreSlot = _adapter.WorldPickToCoreSlot(worldSlot);
            return _services.Input.TrySubmitGrab(coreSlot);
        }

        public ISolutionSpaceAdapter Adapter => _adapter;
    }
}
