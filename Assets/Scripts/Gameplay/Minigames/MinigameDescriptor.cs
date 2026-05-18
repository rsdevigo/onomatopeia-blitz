using UnityEngine;

namespace Blitz.Gameplay.Minigames
{
    [CreateAssetMenu(menuName = "Blitz/Minigame Descriptor", fileName = "MinigameDescriptor")]
    public sealed class MinigameDescriptor : ScriptableObject
    {
        [SerializeField] string _minigameId = "";
        [SerializeField] string _additiveSceneName = "";
        [SerializeField] string _displayName = "";
        [SerializeField] bool _useBlitzGrabDriver = true;

        public string MinigameId => _minigameId;
        public string AdditiveSceneName => _additiveSceneName;
        public string DisplayName => _displayName;
        public bool UseBlitzGrabDriver => _useBlitzGrabDriver;
    }
}
