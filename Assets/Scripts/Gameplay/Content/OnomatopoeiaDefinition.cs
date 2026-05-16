using Blitz.Core;
using UnityEngine;

namespace Blitz.Gameplay.Content
{
    [CreateAssetMenu(menuName = "Blitz/Onomatopoeia Definition", fileName = "Onomatopoeia")]
    public sealed class OnomatopoeiaDefinition : ScriptableObject
    {
        [SerializeField] byte _id;
        [SerializeField] byte _letterValue;
        [SerializeField] ushort _figureVisualId;
        [SerializeField] Sprite? _figureSprite;
        [SerializeField] string _letterDisplay = "";
        [SerializeField] string _writtenLabel = "";
        [SerializeField] AudioClip? _audioClip;

        public OnomatopoeiaId Id => new(_id);

        public LetterId Letter => new(_letterValue);

        public ushort FigureVisualId => _figureVisualId;

        public Sprite? FigureSprite => _figureSprite;

        /// <summary>HUD / card anchor; falls back to <see cref="Letter"/> byte if empty.</summary>
        public string LetterDisplay => string.IsNullOrWhiteSpace(_letterDisplay) ? Letter.Value.ToString() : _letterDisplay;

        public string WrittenLabel => _writtenLabel;

        public AudioClip? AudioClip => _audioClip;
    }
}
