using Blitz.Core;
using UnityEngine;

namespace Blitz.Gameplay.Table;

public sealed class SoundObjectInstance : MonoBehaviour
{
    [SerializeField] byte slotIndex;

    TableRuntimeRegistry? _registry;

    public byte SlotIndex => slotIndex;

    public void Configure(byte slot, TableRuntimeRegistry registry)
    {
        slotIndex = slot;
        _registry = registry;
    }

    void OnEnable()
    {
        _registry ??= GetComponentInParent<TableRuntimeRegistry>();
        if (_registry is null)
            _registry = FindFirstObjectByType<TableRuntimeRegistry>();

        _registry?.Register(this);
    }

    void OnDisable() => _registry?.Unregister(this);
}
