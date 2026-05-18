using System;
using System.Collections.Generic;
using Blitz.Core;
//using UnityEngine;

namespace Blitz.Gameplay.Content
{
    /// <summary>
    /// Draws three distinct <see cref="OnomatopoeiaDefinition"/> entries with three distinct letters
    /// and builds the runtime <see cref="ActiveOnomatopoeiaSet"/> for one match.
    /// </summary>
    public static class OnomatopoeiaMatchSampler
    {
        const int MaxSampleAttempts = 256;

        public static ActiveOnomatopoeiaSet ResolveActiveSet(OnomatopoeiaCatalog? catalog, int seed) =>
            ResolveActiveSet(catalog, seed, out _);

        public static ActiveOnomatopoeiaSet ResolveActiveSet(
            OnomatopoeiaCatalog? catalog,
            int seed,
            out Dictionary<OnomatopoeiaId, OnomatopoeiaDefinition>? definitionsById)
        {
            var rng = new Random(seed);
            if (catalog != null && TrySample(catalog, rng, out var sampled, out definitionsById))
                return sampled;
            definitionsById = null;
            return ActiveOnomatopoeiaSet.CreateSyntheticDevSet(rng);
        }

        public static bool TrySample(
            OnomatopoeiaCatalog catalog,
            Random rng,
            out ActiveOnomatopoeiaSet set,
            out Dictionary<OnomatopoeiaId, OnomatopoeiaDefinition> definitionsById)
        {
            set = default;
            definitionsById = new Dictionary<OnomatopoeiaId, OnomatopoeiaDefinition>();
            var defs = catalog.Definitions;
            var valid = new List<OnomatopoeiaDefinition>(defs.Count);
            foreach (var d in defs)
            {
                if (d != null)
                    valid.Add(d);
            }

            if (valid.Count < 3)
                return false;

            for (var attempt = 0; attempt < MaxSampleAttempts; attempt++)
            {
                var i0 = rng.Next(valid.Count);
                var i1 = rng.Next(valid.Count);
                var i2 = rng.Next(valid.Count);
                if (i1 == i0) continue;
                if (i2 == i0 || i2 == i1) continue;

                var a = valid[i0];
                var b = valid[i1];
                var c = valid[i2];
                if (a.Letter == b.Letter || a.Letter == c.Letter || b.Letter == c.Letter)
                    continue;

                if (!HasDistinctTablePresentation(a, b, c))
                    continue;

                var slots = new[] { a.Id, b.Id, c.Id };
                Shuffle(slots, rng);
                set = new ActiveOnomatopoeiaSet(
                    a.Letter, a.Id,
                    b.Letter, b.Id,
                    c.Letter, c.Id,
                    slots[0], slots[1], slots[2]);
                definitionsById[a.Id] = a;
                definitionsById[b.Id] = b;
                definitionsById[c.Id] = c;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Each table slot must look and sound different: no shared figure sprite or audio clip.
        /// </summary>
        public static bool HasDistinctTablePresentation(
            OnomatopoeiaDefinition a,
            OnomatopoeiaDefinition b,
            OnomatopoeiaDefinition c) =>
            PairDistinct(a.FigureSprite, b.FigureSprite)
            && PairDistinct(a.FigureSprite, c.FigureSprite)
            && PairDistinct(b.FigureSprite, c.FigureSprite)
            && PairDistinct(a.AudioClip, b.AudioClip)
            && PairDistinct(a.AudioClip, c.AudioClip)
            && PairDistinct(b.AudioClip, c.AudioClip);

        static bool PairDistinct(UnityEngine.Object? x, UnityEngine.Object? y) => !ReferenceEquals(x, y);

        static void Shuffle(OnomatopoeiaId[] list, Random rng)
        {
            for (var i = list.Length - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
