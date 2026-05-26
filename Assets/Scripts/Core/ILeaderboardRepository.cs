using System.Collections.Generic;

namespace Blitz.Core
{
    public interface ILeaderboardRepository
    {
        /// <summary>Returns up to <paramref name="max"/> entries sorted by score descending.</summary>
        IReadOnlyList<LeaderboardEntry> LoadTop(int max);

        /// <summary>Inserts entry, keeps top N by score, persists. Returns whether the entry is in the saved top list.</summary>
        bool TryAdd(LeaderboardEntry entry);
    }
}
