using Blitz.Core;
using Blitz.Gameplay.Content;
using UnityEngine;

namespace Blitz.Gameplay.Table
{
    public sealed class SoundObjectInstance : MonoBehaviour
    {
        [SerializeField] byte slotIndex;
        [SerializeField] SpriteRenderer? figureRenderer;

        TableRuntimeRegistry? _registry;

        public byte SlotIndex => slotIndex;

        public void Configure(byte slot, TableRuntimeRegistry registry)
        {
            slotIndex = slot;
            _registry = registry;
        }

        /// <summary>Updates world visuals for this slot from the match's <see cref="OnomatopoeiaDefinition"/>.</summary>
        public void ApplyFromDefinition(OnomatopoeiaDefinition? definition)
        {
            if (figureRenderer is null)
                return;

            figureRenderer.sprite = definition?.FigureSprite;
            figureRenderer.enabled = figureRenderer.sprite != null;
        }

        void OnEnable()
        {
            _registry ??= GetComponentInParent<TableRuntimeRegistry>();
            if (_registry is null)
                _registry = FindAnyObjectByType<TableRuntimeRegistry>();

            _registry?.Register(this);
        }

        void OnDisable() => _registry?.Unregister(this);
    }
}
