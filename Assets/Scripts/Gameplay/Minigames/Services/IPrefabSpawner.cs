using UnityEngine;

namespace Blitz.Gameplay.Minigames
{
    public interface IPrefabSpawner
    {
        GameObject? Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform? parent = null);
    }
}
