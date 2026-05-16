using System.Collections.Generic;

namespace Blitz.Core
{
    /// <summary>
    /// Validates generator invariants for exclusion cards over a 3-slot table.
    /// </summary>
    public static class CardUniqueness
    {
        public static bool ExclusionHasUniqueSolution(LetterId cardLetter, OnomatopoeiaId cue, ActiveOnomatopoeiaSet set)
        {
            var trueUnit = set.TrueOnomatopoeiaForLetter(cardLetter);
            if (cue == trueUnit) return false;

            var matches = new List<byte>(3);
            for (byte j = 0; j < 3; j++)
            {
                var pj = set.GetOnomatopoeiaOnSlot(j);
                if (pj == cue) continue;

                var letterForPj = set.LetterWhoseTrueOnomatopoeiaIs(pj);
                if (letterForPj == cardLetter) continue;

                matches.Add(j);
            }

            return matches.Count == 1;
        }

        public static bool PositiveHasUniqueSolution(OnomatopoeiaId cue, ActiveOnomatopoeiaSet set)
        {
            byte count = 0;
            for (byte j = 0; j < 3; j++)
            {
                if (set.GetOnomatopoeiaOnSlot(j) == cue)
                    count++;
            }

            return count == 1;
        }
    }
}
