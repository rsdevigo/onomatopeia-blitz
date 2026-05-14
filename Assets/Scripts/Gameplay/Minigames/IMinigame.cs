using Blitz.Core;

namespace Blitz.Gameplay.Minigames;

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
    public static MinigameServices Empty { get; } = new MinigameServices();
}

public interface IMinigame
{
    void OnRegister(MinigameServices services);

    void OnSceneLoaded();

    void OnMatchBegin(MatchConfig config);

    void OnRoundBegin(GeneratedCard card, ActiveLetterSoundSet set);

    void OnRoundEnd(RoundOutcome outcome);

    void OnMatchEnd();

    void OnUnregister();
}
