using System;
using System.Collections.Generic;
using Blitz.Core;
using NUnit.Framework;

namespace Blitz.Core.Tests
{
    public sealed class CardGeneratorTests
    {
        [Test]
        public void MatchDraws_NeverRepeatGameplayIdentity()
        {
            var rng = new Random(42);
            var set = ActiveOnomatopoeiaSet.CreateSyntheticDevSet(rng);
            var gen = new CardGenerator(99);
            gen.ResetDeck(set);

            var seen = new List<GeneratedCard>();
            for (var round = 0; round < CardGenerator.MaxDistinctCardsPerMatch; round++)
            {
                Assert.That(gen.TryGenerateCard(set, out var card), Is.True, $"round {round}");
                foreach (var prior in seen)
                    Assert.That(CardUniqueness.HaveSameGameplayIdentity(prior, card), Is.False, $"round {round}");
                seen.Add(card);
            }

            Assert.That(gen.TryGenerateCard(set, out _), Is.False);
        }

        [Test]
        public void ResetDeck_ProducesAtMostNineDistinctCards()
        {
            var rng = new Random(7);
            var set = ActiveOnomatopoeiaSet.CreateSyntheticDevSet(rng);
            var deck = new List<GeneratedCard>();
            CardGenerator.CollectAllValidCards(set, deck);

            Assert.That(deck.Count, Is.GreaterThan(0).And.LessThanOrEqualTo(CardGenerator.MaxDistinctCardsPerMatch));

            for (var i = 0; i < deck.Count; i++)
            {
                for (var j = i + 1; j < deck.Count; j++)
                    Assert.That(CardUniqueness.HaveSameGameplayIdentity(deck[i], deck[j]), Is.False);
            }
        }
    }
}
