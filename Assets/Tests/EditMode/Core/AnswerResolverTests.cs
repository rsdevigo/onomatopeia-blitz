using Blitz.Core;
using NUnit.Framework;

namespace Blitz.Core.Tests
{
    public sealed class AnswerResolverTests
    {
        static readonly AnswerResolver Resolver = new();

        [Test]
        public void Positive_ResolvesUniqueSlotMatchingCue()
        {
            var set = new ActiveLetterSoundSet(
                new LetterId(0), new PhonemeId(10),
                new LetterId(1), new PhonemeId(11),
                new LetterId(2), new PhonemeId(12),
                new PhonemeId(12), new PhonemeId(10), new PhonemeId(11));

            var card = new GeneratedCard(
                new LetterId(1),
                new CardPresentationPair(1, new PhonemeId(11)),
                CardMode.HasTruePair);

            var answer = Resolver.Resolve(card, set);
            Assert.That(answer.Slot, Is.EqualTo(2));
        }

        [Test]
        public void Exclusion_ResolvesUniqueObjectByRule()
        {
            var set = new ActiveLetterSoundSet(
                new LetterId(0), new PhonemeId(10),
                new LetterId(1), new PhonemeId(11),
                new LetterId(2), new PhonemeId(12),
                new PhonemeId(12), new PhonemeId(10), new PhonemeId(11));

            var card = new GeneratedCard(
                new LetterId(1),
                new CardPresentationPair(9, new PhonemeId(12)),
                CardMode.ExclusionMismatch);

            var answer = Resolver.Resolve(card, set);
            Assert.That(answer.Slot, Is.EqualTo(1));
        }

        [Test]
        public void Generator_PositiveAndExclusion_AreUniqueAcrossManySeeds()
        {
            for (var seed = 0; seed < 500; seed++)
            {
                var gen = new CardGenerator(seed);
                Assert.That(gen.TryGenerate(out var result), Is.True, $"seed {seed}");

                if (result.Card.Mode == CardMode.HasTruePair)
                    Assert.That(CardUniqueness.PositiveHasUniqueSolution(result.Card.CuePhonemeId, result.ActiveSet), Is.True);

                if (result.Card.Mode == CardMode.ExclusionMismatch)
                {
                    Assert.That(
                        CardUniqueness.ExclusionHasUniqueSolution(result.Card.CardLetterId, result.Card.CuePhonemeId, result.ActiveSet),
                        Is.True,
                        $"seed {seed}");
                }

                Assert.DoesNotThrow(() => Resolver.Resolve(result.Card, result.ActiveSet));
                var resolved = Resolver.Resolve(result.Card, result.ActiveSet);

                if (result.Card.Mode == CardMode.HasTruePair)
                    Assert.That(result.ActiveSet.GetPhonemeOnSlot(resolved.Slot), Is.EqualTo(result.Card.CuePhonemeId));
            }
        }
    }
}
