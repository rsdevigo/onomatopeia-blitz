using System;
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
            var set = new ActiveOnomatopoeiaSet(
                new LetterId(0), new OnomatopoeiaId(10),
                new LetterId(1), new OnomatopoeiaId(11),
                new LetterId(2), new OnomatopoeiaId(12),
                new OnomatopoeiaId(12), new OnomatopoeiaId(10), new OnomatopoeiaId(11));

            var card = new GeneratedCard(
                new LetterId(1),
                new CardPresentationPair(1, new OnomatopoeiaId(11)),
                CardMode.HasTruePair);

            var answer = Resolver.Resolve(card, set);
            Assert.That(answer.Slot, Is.EqualTo(2));
        }

        [Test]
        public void Exclusion_ResolvesUniqueObjectByRule()
        {
            var set = new ActiveOnomatopoeiaSet(
                new LetterId(0), new OnomatopoeiaId(10),
                new LetterId(1), new OnomatopoeiaId(11),
                new LetterId(2), new OnomatopoeiaId(12),
                new OnomatopoeiaId(12), new OnomatopoeiaId(10), new OnomatopoeiaId(11));

            var card = new GeneratedCard(
                new LetterId(1),
                new CardPresentationPair(9, new OnomatopoeiaId(12)),
                CardMode.ExclusionMismatch);

            var answer = Resolver.Resolve(card, set);
            Assert.That(answer.Slot, Is.EqualTo(1));
        }

        [Test]
        public void Generator_PositiveAndExclusion_AreUniqueAcrossManySeeds()
        {
            for (var seed = 0; seed < 500; seed++)
            {
                var rng = new Random(seed);
                var set = ActiveOnomatopoeiaSet.CreateSyntheticDevSet(rng);
                var gen = new CardGenerator(seed);
                Assert.That(gen.TryGenerateCard(set, out var card), Is.True, $"seed {seed}");

                if (card.Mode == CardMode.HasTruePair)
                    Assert.That(CardUniqueness.PositiveHasUniqueSolution(card.CueOnomatopoeiaId, set), Is.True);

                if (card.Mode == CardMode.ExclusionMismatch)
                {
                    Assert.That(
                        CardUniqueness.ExclusionHasUniqueSolution(card.CardLetterId, card.CueOnomatopoeiaId, set),
                        Is.True,
                        $"seed {seed}");
                }

                var resolved = Resolver.Resolve(card, set);

                if (card.Mode == CardMode.HasTruePair)
                    Assert.That(set.GetOnomatopoeiaOnSlot(resolved.Slot), Is.EqualTo(card.CueOnomatopoeiaId));
            }
        }
    }
}
