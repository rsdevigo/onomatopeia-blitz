using System.Collections.Generic;
using UnityEngine;

namespace Blitz.Gameplay.Content
{
    [CreateAssetMenu(menuName = "Blitz/Onomatopoeia Catalog", fileName = "OnomatopoeiaCatalog")]
    public sealed class OnomatopoeiaCatalog : ScriptableObject
    {
        [SerializeField] List<OnomatopoeiaDefinition> _definitions = new();

        public IReadOnlyList<OnomatopoeiaDefinition> Definitions => _definitions;
    }
}
