using Blitz.Gameplay.Content;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Blitz.Gameplay.Tests
{
    public sealed class OnomatopoeiaMatchSamplerTests
    {
        [Test]
        public void HasDistinctTablePresentation_RejectsSharedSprite()
        {
            var shared = CreateSprite(Color.red);
            var other = CreateSprite(Color.blue);
            var a = CreateDefinition(0, 0, shared, null);
            var b = CreateDefinition(1, 1, shared, null);
            var c = CreateDefinition(2, 2, other, null);

            Assert.That(OnomatopoeiaMatchSampler.HasDistinctTablePresentation(a, b, c), Is.False);
        }

        [Test]
        public void HasDistinctTablePresentation_AcceptsUniqueSprites()
        {
            var a = CreateDefinition(0, 0, CreateSprite(Color.red), null);
            var b = CreateDefinition(1, 1, CreateSprite(Color.green), null);
            var c = CreateDefinition(2, 2, CreateSprite(Color.blue), null);

            Assert.That(OnomatopoeiaMatchSampler.HasDistinctTablePresentation(a, b, c), Is.True);
        }

        static Sprite CreateSprite(Color color)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one);
        }

        static OnomatopoeiaDefinition CreateDefinition(byte id, byte letter, Sprite? sprite, AudioClip? clip)
        {
            var def = ScriptableObject.CreateInstance<OnomatopoeiaDefinition>();
            var so = new SerializedObject(def);
            so.FindProperty("_id").intValue = id;
            so.FindProperty("_letterValue").intValue = letter;
            so.FindProperty("_figureSprite").objectReferenceValue = sprite;
            so.FindProperty("_audioClip").objectReferenceValue = clip;
            so.ApplyModifiedPropertiesWithoutUndo();
            return def;
        }
    }
}
