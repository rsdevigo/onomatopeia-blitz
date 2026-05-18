using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blitz.Gameplay.Minigames
{
    public interface IMinigameLoader
    {
        Task<Scene> LoadAsync(MinigameDescriptor descriptor);
        Task UnloadAsync();
    }

    public sealed class MinigameLoader : IMinigameLoader
    {
        Scene _loadedScene;

        public async Task<Scene> LoadAsync(MinigameDescriptor descriptor)
        {
            if (descriptor is null || string.IsNullOrEmpty(descriptor.AdditiveSceneName))
            {
                Debug.LogError("MinigameLoader: invalid descriptor or scene name.");
                return default;
            }

            var op = SceneManager.LoadSceneAsync(descriptor.AdditiveSceneName, LoadSceneMode.Additive);
            if (op is null)
            {
                Debug.LogError($"MinigameLoader: failed to load '{descriptor.AdditiveSceneName}'. Is it in Build Settings?");
                return default;
            }

            while (!op.isDone)
                await Task.Yield();

            _loadedScene = SceneManager.GetSceneByName(descriptor.AdditiveSceneName);
            return _loadedScene;
        }

        public async Task UnloadAsync()
        {
            if (!_loadedScene.IsValid() || !_loadedScene.isLoaded)
                return;

            var op = SceneManager.UnloadSceneAsync(_loadedScene);
            if (op is null)
                return;

            while (!op.isDone)
                await Task.Yield();

            _loadedScene = default;
        }

        public void UnloadImmediate()
        {
            if (!_loadedScene.IsValid() || !_loadedScene.isLoaded)
                return;

            SceneManager.UnloadSceneAsync(_loadedScene);
            _loadedScene = default;
        }
    }
}
