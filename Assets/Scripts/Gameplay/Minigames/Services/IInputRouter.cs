using Blitz.Core;

namespace Blitz.Gameplay.Minigames
{
    public interface IInputRouter
    {
        bool TrySubmitGrab(SoundObjectId id);
    }
}
