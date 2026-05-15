using System.Collections.Generic;

namespace Blitz.Core
{
    /// <summary>
    /// Validates generator invariants for exclusion cards over a 3-slot table.
    /// </summary>
    public static class CardUniqueness
    {
        public static bool ExclusionHasUniqueSolution(LetterId cardLetter, PhonemeId cuePhoneme, ActiveLetterSoundSet set)
        {
            var trueSound = set.TruePhonemeForLetter(cardLetter);
            if (cuePhoneme == trueSound) return false;

            var matches = new List<byte>(3);
            for (byte j = 0; j < 3; j++)
            {
                var pj = set.GetPhonemeOnSlot(j);
                if (pj == cuePhoneme) continue;

                var letterForPj = set.LetterWhoseTrueSoundIs(pj);
                if (letterForPj == cardLetter) continue;

                matches.Add(j);
            }

            return matches.Count == 1;
        }

        public static bool PositiveHasUniqueSolution(PhonemeId cuePhoneme, ActiveLetterSoundSet set)
        {
            byte count = 0;
            for (byte j = 0; j < 3; j++)
            {
                if (set.GetPhonemeOnSlot(j) == cuePhoneme)
                    count++;
            }

            return count == 1;
        }
    }
}
