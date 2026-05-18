using Blitz.Core;
using Blitz.Gameplay.Content;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Blitz.Gameplay.Tests
{
    public sealed class DifficultyProfileTests
    {
        [Test]
        public void ToMatchRules_ClampsRoundsAndGrabWindow()
        {
            var profile = ScriptableObject.CreateInstance<DifficultyProfile>();
            var so = new SerializedObject(profile);
            so.FindProperty("_totalRounds").intValue = 99;
            so.FindProperty("_grabWindowSeconds").floatValue = 0.1f;
            so.ApplyModifiedPropertiesWithoutUndo();

            var rules = profile.ToMatchRules();

            Assert.That(rules.TotalRounds, Is.EqualTo(CardGenerator.MaxDistinctCardsPerMatch));
            Assert.That(rules.GrabWindowSeconds, Is.EqualTo(0.5f));
        }
    }
}
