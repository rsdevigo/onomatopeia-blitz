using System;
using System.Collections.Generic;

namespace Blitz.Core;

public readonly struct CardGenerationResult
{
    public readonly GeneratedCard Card;
    public readonly ActiveLetterSoundSet ActiveSet;

    public CardGenerationResult(GeneratedCard card, ActiveLetterSoundSet activeSet)
    {
        Card = card;
        ActiveSet = activeSet;
    }
}

/// <summary>
/// Builds single-slot cards for the three-object table with positive vs exclusion modes.
/// </summary>
public sealed class CardGenerator
{
    readonly Random _rng;

    public CardGenerator(int seed) => _rng = new Random(seed);

    public CardGenerator(Random rng) => _rng = rng;

    public bool TryGenerate(out CardGenerationResult result, int maxAttempts = 256)
    {
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            if (TryGenerateOnce(out result))
                return true;
        }

        result = default;
        return false;
    }

    bool TryGenerateOnce(out CardGenerationResult result)
    {
        var l0 = new LetterId(0);
        var l1 = new LetterId(1);
        var l2 = new LetterId(2);

        var t0 = new PhonemeId(10);
        var t1 = new PhonemeId(11);
        var t2 = new PhonemeId(12);

        var phonemes = new[] { t0, t1, t2 };
        Shuffle(phonemes);

        var activeSet = new ActiveLetterSoundSet(
            l0, t0,
            l1, t1,
            l2, t2,
            phonemes[0], phonemes[1], phonemes[2]);

        var letters = new[] { l0, l1, l2 };
        var cardLetter = letters[_rng.Next(3)];
        var usePositive = _rng.Next(2) == 0;

        if (usePositive)
        {
            var cp = activeSet.TruePhonemeForLetter(cardLetter);
            if (!CardUniqueness.PositiveHasUniqueSolution(cp, activeSet))
            {
                result = default;
                return false;
            }

            var figure = (ushort)(100 + cardLetter.Value);
            var card = new GeneratedCard(cardLetter, new CardPresentationPair(figure, cp), CardMode.HasTruePair);
            result = new CardGenerationResult(card, activeSet);
            return true;
        }

        var taught = activeSet.TruePhonemeForLetter(cardLetter);
        var cueCandidates = new List<PhonemeId>(2);
        if (t0 != taught) cueCandidates.Add(t0);
        if (t1 != taught) cueCandidates.Add(t1);
        if (t2 != taught) cueCandidates.Add(t2);

        var cue = cueCandidates[_rng.Next(cueCandidates.Count)];
        if (!CardUniqueness.ExclusionHasUniqueSolution(cardLetter, cue, activeSet))
        {
            result = default;
            return false;
        }

        var fig = (ushort)(200 + cardLetter.Value);
        var exclusionCard = new GeneratedCard(cardLetter, new CardPresentationPair(fig, cue), CardMode.ExclusionMismatch);
        result = new CardGenerationResult(exclusionCard, activeSet);
        return true;
    }

    void Shuffle(IList<PhonemeId> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = _rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
