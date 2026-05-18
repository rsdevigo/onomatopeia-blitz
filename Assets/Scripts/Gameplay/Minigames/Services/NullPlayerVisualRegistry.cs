namespace Blitz.Gameplay.Minigames
{
    public sealed class NullPlayerVisualRegistry : IPlayerVisualRegistry
    {
        public static NullPlayerVisualRegistry Instance { get; } = new();
    }
}
