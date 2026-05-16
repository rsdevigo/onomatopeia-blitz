using System;

namespace Blitz.Core
{
    /// <summary>
    /// Pure rules: positive (cue matches T(L_card)) vs exclusion (cue mismatches T(L_card)).
    /// </summary>
    public sealed class AnswerResolver : IAnswerResolver
    {
        public SoundObjectId Resolve(GeneratedCard card, ActiveOnomatopoeiaSet activeSet)
        {
            var trueUnit = activeSet.TrueOnomatopoeiaForLetter(card.CardLetterId);
            var co = card.CueOnomatopoeiaId;

            if (card.Mode == CardMode.HasTruePair)
            {
                if (co != trueUnit)
                    throw new InvalidOperationException("Positive card requires cue onomatopoeia to equal T(CardLetterId).");

                for (byte j = 0; j < 3; j++)
                {
                    if (activeSet.GetOnomatopoeiaOnSlot(j) == co)
                        return new SoundObjectId(j);
                }

                throw new InvalidOperationException("No table slot carries the cue onomatopoeia.");
            }

            if (co == trueUnit)
                throw new InvalidOperationException("Exclusion card requires cue onomatopoeia to differ from T(CardLetterId).");

            SoundObjectId? unique = null;
            for (byte j = 0; j < 3; j++)
            {
                var pj = activeSet.GetOnomatopoeiaOnSlot(j);
                if (pj == co) continue;

                var letterForPj = activeSet.LetterWhoseTrueOnomatopoeiaIs(pj);
                if (letterForPj == card.CardLetterId) continue;

                if (unique.HasValue)
                    throw new InvalidOperationException("Exclusion resolution is ambiguous — generator invariant violated.");

                unique = new SoundObjectId(j);
            }

            if (!unique.HasValue)
                throw new InvalidOperationException("Exclusion resolution found no candidate — generator invariant violated.");

            return unique.Value;
        }
    }
}
