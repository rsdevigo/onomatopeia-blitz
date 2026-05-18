using Blitz.Gameplay.Input;
using UnityEngine;

namespace Blitz.Gameplay.Minigames
{
    /// <summary>Builds <see cref="MinigameServices"/> from core-scene references (lives with gameplay core).</summary>
    public sealed class MinigameServicesHost : MonoBehaviour
    {
        [SerializeField] LocalMatchSession? _session;
        [SerializeField] OfflineGrabInputDriver? _grabDriver;
        [SerializeField] Transform? _spawnRoot;
        [SerializeField] bool _blitzGrabDriverEnabled = true;

        public MinigameServices Build(bool useBlitzGrabDriver)
        {
            _session ??= FindAnyObjectByType<LocalMatchSession>();
            _grabDriver ??= FindAnyObjectByType<OfflineGrabInputDriver>();

            if (_grabDriver != null)
                _grabDriver.enabled = useBlitzGrabDriver && _blitzGrabDriverEnabled;

            var audioSource = _session != null ? _session.GetComponent<AudioSource>() : null;
            IInputRouter input = _session != null
                ? new MatchSessionInputRouter(_session)
                : new NoOpInputRouter();

            return MinigameServices.Create(
                new SessionAudioDirector(audioSource),
                input,
                new SimplePrefabSpawner(_spawnRoot),
                NullPlayerVisualRegistry.Instance);
        }
    }
}
