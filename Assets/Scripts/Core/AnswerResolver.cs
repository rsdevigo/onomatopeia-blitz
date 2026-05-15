using System;

namespace Blitz.Core
{
    /// <summary>
    /// Pure rules: positive (cue matches T(L_card)) vs exclusion (cue mismatches T(L_card)).
    /// </summary>
    public sealed class AnswerResolver : IAnswerResolver
    {
        public SoundObjectId Resolve(GeneratedCard card, ActiveLetterSoundSet activeSet)
        {
            var trueSound = activeSet.TruePhonemeForLetter(card.CardLetterId);
            var cp = card.CuePhonemeId;

            if (card.Mode == CardMode.HasTruePair)
            {
                if (cp != trueSound)
                    throw new InvalidOperationException("Positive card requires cue phoneme to equal T(CardLetterId).");

                for (byte j = 0; j < 3; j++)
                {
                    if (activeSet.GetPhonemeOnSlot(j) == cp)
                        return new SoundObjectId(j);
                }

                throw new InvalidOperationException("No table slot carries the cue phoneme.");
            }

            if (cp == trueSound)
                throw new InvalidOperationException("Exclusion card requires cue phoneme to differ from T(CardLetterId).");

            SoundObjectId? unique = null;
            for (byte j = 0; j < 3; j++)
            {
                var pj = activeSet.GetPhonemeOnSlot(j);
                if (pj == cp) continue;

                var letterForPj = activeSet.LetterWhoseTrueSoundIs(pj);
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
