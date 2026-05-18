using UnityEngine;

namespace Blitz.Gameplay.Minigames
{
    public sealed class SimplePrefabSpawner : IPrefabSpawner
    {
        readonly Transform? _defaultParent;

        public SimplePrefabSpawner(Transform? defaultParent) => _defaultParent = defaultParent;

        public GameObject? Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform? parent = null)
        {
            if (prefab is null)
                return null;

            return Object.Instantiate(prefab, position, rotation, parent ?? _defaultParent);
        }
    }
}
