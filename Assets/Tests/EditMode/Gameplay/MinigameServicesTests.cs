using Blitz.Core;
using Blitz.Gameplay.Minigames;
using NUnit.Framework;

namespace Blitz.Gameplay.Tests
{
    public sealed class MinigameServicesTests
    {
        [Test]
        public void Create_ProvidesNonNullServices()
        {
            var services = MinigameServices.Create(
                new NullAudioDirector(),
                new NoOpInputRouter(),
                new SimplePrefabSpawner(null),
                NullPlayerVisualRegistry.Instance);

            Assert.That(services.Audio, Is.Not.Null);
            Assert.That(services.Input, Is.Not.Null);
            Assert.That(services.Spawner, Is.Not.Null);
            Assert.That(services.Players, Is.Not.Null);
        }

        [Test]
        public void BlitzMinigame_OnRegister_DoesNotThrow()
        {
            var go = new UnityEngine.GameObject("test");
            var minigame = go.AddComponent<BlitzOnomatopoeicoMinigame>();
            Assert.DoesNotThrow(() => minigame.OnRegister(MinigameServices.Empty));
            UnityEngine.Object.DestroyImmediate(go);
        }
    }
}
