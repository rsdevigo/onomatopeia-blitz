using Blitz.Core;
using UnityEngine;

namespace Blitz.Gameplay.Minigames
{
    /// <summary>
    /// Second minigame: letter-forward props with an adapter hook for future remapping.
    /// </summary>
    public sealed class FantasmaLadraoMinigame : MonoBehaviour, IMinigame
    {
        [SerializeField] LocalMatchSession? session;

        ISolutionSpaceAdapter _adapter = IdentitySolutionSpaceAdapter.Instance;

        public void OnRegister(MinigameServices services)
        {
            _adapter = IdentitySolutionSpaceAdapter.Instance;
        }

        public void OnSceneLoaded() => session ??= FindAnyObjectByType<LocalMatchSession>();

        public void OnMatchBegin(MatchConfig config) => session?.StartMatch(config.Rules, config.Seed);

        public void OnRoundBegin(GeneratedCard card, ActiveLetterSoundSet set)
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

        public bool TrySubmitWorldGrab(SoundObjectId worldSlot) =>
            session is not null && session.TrySubmitGrab(_adapter.WorldPickToCoreSlot(worldSlot));

        public ISolutionSpaceAdapter Adapter => _adapter;
    }
}
