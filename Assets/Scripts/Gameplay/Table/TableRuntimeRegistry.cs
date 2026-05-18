using Blitz.Core;
using Blitz.Gameplay;
using Blitz.Gameplay.Content;
using UnityEngine;

namespace Blitz.Gameplay.Table
{
    /// <summary>
    /// Resolves the three table slots to runtime instances (by slot index).
    /// </summary>
    public sealed class TableRuntimeRegistry : MonoBehaviour
    {
        readonly SoundObjectInstance?[] _slots = new SoundObjectInstance[3];

        public void Register(SoundObjectInstance instance)
        {
            var i = instance.SlotIndex;
            if (i > 2) return;
            _slots[i] = instance;
        }

        public void Unregister(SoundObjectInstance instance)
        {
            var i = instance.SlotIndex;
            if (i <= 2 && _slots[i] == instance)
                _slots[i] = null;
        }

        public SoundObjectInstance? GetSlot(byte index) => index <= 2 ? _slots[index] : null;

        /// <summary>
        /// Binds each slot to the onomatopoeia on the table for this match (sprite / label from <see cref="OnomatopoeiaDefinition"/> when available).
        /// </summary>
        public void ApplyMatchSlots(ActiveOnomatopoeiaSet set, IOnomatopoeiaMatchContent? content)
        {
            for (byte i = 0; i < 3; i++)
            {
                var id = set.GetOnomatopoeiaOnSlot(i);
                OnomatopoeiaDefinition? def = null;
                content?.TryGetDefinition(id, out def);
                GetSlot(i)?.ApplyFromDefinition(def);
            }
        }

        public bool TryRaycastGrab(Camera camera, Vector3 screenPoint, out SoundObjectId id)
        {
            id = default;
            if (camera is null)
                return false;

            var ray = camera.ScreenPointToRay(screenPoint);
            var hit2D = Physics2D.GetRayIntersection(ray, 100f);
            if (hit2D.collider != null)
            {
                var sound2D = hit2D.collider.GetComponentInParent<SoundObjectInstance>();
                if (sound2D != null)
                {
                    id = new SoundObjectId(sound2D.SlotIndex);
                    return true;
                }
            }

            if (!Physics.Raycast(ray, out var hit3D, 100f, ~0, QueryTriggerInteraction.Ignore))
                return false;

            var sound3D = hit3D.collider.GetComponentInParent<SoundObjectInstance>();
            if (sound3D is null)
                return false;

            id = new SoundObjectId(sound3D.SlotIndex);
            return true;
        }
    }
}
