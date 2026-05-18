using Blitz.Core;

namespace Blitz.Gameplay.Minigames
{
    public sealed class MatchSessionInputRouter : IInputRouter
    {
        readonly IMatchSession _session;

        public MatchSessionInputRouter(IMatchSession session) => _session = session;

        public bool TrySubmitGrab(SoundObjectId id) => _session.TrySubmitGrab(id);
    }
}
