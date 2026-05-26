namespace Blitz.Core
{
    /// <summary>Runtime registration so UI/Gameplay can use <see cref="ILeaderboardRepository"/> without referencing App.</summary>
    public static class LeaderboardServices
    {
        static ILeaderboardRepository? _repository;

        public static void Register(ILeaderboardRepository repository) =>
            _repository = repository;

        public static bool TryGetRepository(out ILeaderboardRepository repository)
        {
            repository = _repository!;
            return _repository != null;
        }
    }
}
