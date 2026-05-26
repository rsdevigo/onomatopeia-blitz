using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blitz.Core;
using UnityEngine;

namespace Blitz.App
{
    public sealed class JsonLeaderboardRepository : ILeaderboardRepository
    {
        readonly string _filePath;
        readonly int _maxEntries;
        List<LeaderboardEntry> _entries = new();

        public JsonLeaderboardRepository(int maxEntries = LeaderboardConstants.DefaultTopCount)
        {
            _maxEntries = Math.Max(1, maxEntries);
            _filePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
            LoadFromDisk();
        }

        public IReadOnlyList<LeaderboardEntry> LoadTop(int max)
        {
            var take = Math.Max(0, max);
            return _entries
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.UtcIso)
                .Take(take)
                .ToList();
        }

        public bool TryAdd(LeaderboardEntry entry)
        {
            _entries.Add(entry);
            TrimAndSort();
            SaveToDisk();

            return IsInTopList(entry);
        }

        void TrimAndSort()
        {
            _entries = _entries
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.UtcIso)
                .Take(_maxEntries)
                .ToList();
        }

        bool IsInTopList(LeaderboardEntry entry) =>
            _entries.Any(e =>
                e.Score == entry.Score
                && e.Name == entry.Name
                && e.MinigameId == entry.MinigameId
                && e.DifficultyId == entry.DifficultyId
                && e.UtcIso == entry.UtcIso);

        void LoadFromDisk()
        {
            if (!File.Exists(_filePath))
            {
                _entries = new List<LeaderboardEntry>();
                return;
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _entries = new List<LeaderboardEntry>();
                    return;
                }

                var file = JsonUtility.FromJson<LeaderboardJsonFile>(json);
                _entries = file?.Entries != null
                    ? new List<LeaderboardEntry>(file.Entries)
                    : new List<LeaderboardEntry>();
                TrimAndSort();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[JsonLeaderboardRepository] Could not load '{_filePath}': {ex.Message}");
                _entries = new List<LeaderboardEntry>();
            }
        }

        void SaveToDisk()
        {
            try
            {
                var file = new LeaderboardJsonFile { Entries = _entries.ToArray() };
                var json = JsonUtility.ToJson(file, prettyPrint: true);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[JsonLeaderboardRepository] Could not save '{_filePath}': {ex.Message}");
            }
        }

        [Serializable]
        sealed class LeaderboardJsonFile
        {
            public LeaderboardEntry[] Entries = Array.Empty<LeaderboardEntry>();
        }
    }
}
