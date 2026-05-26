using Blitz.Core;
using UnityEngine;

namespace Blitz.App
{
    /// <summary>Registers <see cref="JsonLeaderboardRepository"/> for UI and gameplay (place on menu or bootstrap scene).</summary>
    public sealed class LeaderboardBootstrap : MonoBehaviour
    {
        [SerializeField] bool _dontDestroyOnLoad = true;
        [SerializeField] int _maxEntries = LeaderboardConstants.DefaultTopCount;

        JsonLeaderboardRepository? _repository;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void EnsureRegistered()
        {
            if (LeaderboardServices.TryGetRepository(out _))
                return;

            var go = new GameObject(nameof(LeaderboardBootstrap));
            go.AddComponent<LeaderboardBootstrap>();
        }

        void Awake()
        {
            if (LeaderboardServices.TryGetRepository(out _))
            {
                Destroy(gameObject);
                return;
            }

            _repository = new JsonLeaderboardRepository(_maxEntries);
            LeaderboardServices.Register(_repository);

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
    }
}
