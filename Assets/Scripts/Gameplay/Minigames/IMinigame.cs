using Blitz.Core;

namespace Blitz.Gameplay.Minigames
{
    public readonly struct MatchConfig
    {
        public readonly MatchRules Rules;
        public readonly int Seed;

        public MatchConfig(MatchRules rules, int seed)
        {
            Rules = rules;
            Seed = seed;
        }
    }

    public sealed class MinigameServices
    {
        public IAudioDirector Audio { get; }
        public IInputRouter Input { get; }
        public IPrefabSpawner Spawner { get; }
        public IPlayerVisualRegistry Players { get; }

        MinigameServices(
            IAudioDirector audio,
            IInputRouter input,
            IPrefabSpawner spawner,
            IPlayerVisualRegistry players)
        {
            Audio = audio;
            Input = input;
            Spawner = spawner;
            Players = players;
        }

        public static MinigameServices Empty { get; } = Create(
            new NullAudioDirector(),
            new NoOpInputRouter(),
            new SimplePrefabSpawner(null),
            NullPlayerVisualRegistry.Instance);

        public static MinigameServices Create(
            IAudioDirector audio,
            IInputRouter input,
            IPrefabSpawner spawner,
            IPlayerVisualRegistry players) =>
            new(audio, input, spawner, players);
    }

    public interface IMinigame
    {
        void OnRegister(MinigameServices services);

        void OnSceneLoaded();

        void OnMatchBegin(MatchConfig config);

        void OnRoundBegin(GeneratedCard card, ActiveOnomatopoeiaSet set);

        void OnRoundEnd(RoundOutcome outcome);

        void OnMatchEnd();

        void OnUnregister();
    }
}
