using Blitz.Core;
using Blitz.Gameplay.Content;

namespace Blitz.Gameplay
{
    /// <summary>
    /// Resolves <see cref="OnomatopoeiaDefinition"/> assets for the three units in the current match (by <see cref="OnomatopoeiaId"/>).
    /// </summary>
    public interface IOnomatopoeiaMatchContent
    {
        bool TryGetDefinition(OnomatopoeiaId id, out OnomatopoeiaDefinition? definition);
    }
}
