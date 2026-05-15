namespace Blitz.Core
{
    public readonly struct MatchRules
    {
        public readonly int TotalRounds;
        public readonly float GrabWindowSeconds;
        public const int TableOptionCount = 3;

        public MatchRules(int totalRounds, float grabWindowSeconds)
        {
            TotalRounds = totalRounds;
            GrabWindowSeconds = grabWindowSeconds;
        }
    }

    public enum MatchPhase : byte
    {
        MatchInit = 0,
        RoundPrepare = 1,
        RoundPresent = 2,
        GrabPhase = 3,
        SpeakPhase = 4,
        ResolveRound = 5,
        MatchEnd = 6
    }

    public readonly struct RoundOutcome
    {
        public readonly bool WonCard;
        public readonly SoundObjectId SubmittedSlot;
        public readonly SoundObjectId CorrectSlot;

        public RoundOutcome(bool wonCard, SoundObjectId submittedSlot, SoundObjectId correctSlot)
        {
            WonCard = wonCard;
            SubmittedSlot = submittedSlot;
            CorrectSlot = correctSlot;
        }
    }
}
