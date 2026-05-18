using System.Collections.Generic;
using UnityEngine;

namespace Blitz.Gameplay.Minigames
{
    [CreateAssetMenu(menuName = "Blitz/Minigame Catalog", fileName = "MinigameCatalog")]
    public sealed class MinigameCatalog : ScriptableObject
    {
        [SerializeField] List<MinigameDescriptor> _entries = new();

        public IReadOnlyList<MinigameDescriptor> Entries => _entries;

        public bool TryGet(string minigameId, out MinigameDescriptor? descriptor)
        {
            foreach (var entry in _entries)
            {
                if (entry != null && entry.MinigameId == minigameId)
                {
                    descriptor = entry;
                    return true;
                }
            }

            descriptor = null;
            return false;
        }
    }
}
