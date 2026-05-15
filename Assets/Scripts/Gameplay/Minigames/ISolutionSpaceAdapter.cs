using Blitz.Core;

namespace Blitz.Gameplay.Minigames
{
    /// <summary>
    /// Lets a minigame remap world picks (e.g. letter meshes) to the canonical 0..2 table slots used by <see cref="AnswerResolver"/>.
    /// </summary>
    public interface ISolutionSpaceAdapter
    {
        SoundObjectId WorldPickToCoreSlot(SoundObjectId worldSlot);

        SoundObjectId CoreSlotToWorldPick(SoundObjectId coreSlot);
    }

    public sealed class IdentitySolutionSpaceAdapter : ISolutionSpaceAdapter
    {
        public static IdentitySolutionSpaceAdapter Instance { get; } = new();

        public SoundObjectId WorldPickToCoreSlot(SoundObjectId worldSlot) => worldSlot;

        public SoundObjectId CoreSlotToWorldPick(SoundObjectId coreSlot) => coreSlot;
    }
}
