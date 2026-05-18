using System.Collections.Generic;
using UnityEngine;

namespace Blitz.Gameplay.Content
{
    [CreateAssetMenu(menuName = "Blitz/Difficulty Catalog", fileName = "DifficultyCatalog")]
    public sealed class DifficultyCatalog : ScriptableObject
    {
        [SerializeField] List<DifficultyProfile> _entries = new();

        public IReadOnlyList<DifficultyProfile> Entries => _entries;

        public bool TryGet(string difficultyId, out DifficultyProfile? profile)
        {
            foreach (var entry in _entries)
            {
                if (entry != null && entry.DifficultyId == difficultyId)
                {
                    profile = entry;
                    return true;
                }
            }

            profile = null;
            return false;
        }

        public int IndexOf(string difficultyId)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                if (_entries[i] != null && _entries[i].DifficultyId == difficultyId)
                    return i;
            }

            return -1;
        }
    }
}
