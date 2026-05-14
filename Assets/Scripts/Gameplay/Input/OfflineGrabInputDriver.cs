using Blitz.Core;
using Blitz.Gameplay.Table;
using UnityEngine;

namespace Blitz.Gameplay.Input;

/// <summary>
/// Offline mouse/touch grab: left button during grab phase raycasts into <see cref="Table.TableRuntimeRegistry"/>.
/// </summary>
public sealed class OfflineGrabInputDriver : MonoBehaviour
{
    [SerializeField] Camera? targetCamera;
    [SerializeField] TableRuntimeRegistry? registry;
    [SerializeField] LocalMatchSession? session;

    void Awake()
    {
        targetCamera ??= Camera.main;
        registry ??= FindFirstObjectByType<TableRuntimeRegistry>();
        session ??= FindFirstObjectByType<LocalMatchSession>();
    }

    void Update()
    {
        if (session is null || registry is null || targetCamera is null)
            return;

        if (session.Phase != MatchPhase.GrabPhase)
            return;

        if (!UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            return;

        var pos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        if (registry.TryRaycastGrab(targetCamera, pos, out var id))
            session.TrySubmitGrab(id);
    }
}
