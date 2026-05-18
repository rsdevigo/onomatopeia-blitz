using Blitz.Core;

namespace Blitz.Gameplay.Minigames
{
    /// <summary>Used when the generic Blitz grab driver is disabled (e.g. Fantasma world picks).</summary>
    public sealed class NoOpInputRouter : IInputRouter
    {
        public bool TrySubmitGrab(SoundObjectId id) => false;
    }
}
