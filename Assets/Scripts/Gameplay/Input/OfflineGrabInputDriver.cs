using Blitz.Core;
using Blitz.Gameplay.Table;
using UnityEngine;

namespace Blitz.Gameplay.Input
{
    /// <summary>
    /// Blitz offline grab: left button during grab phase raycasts into <see cref="TableRuntimeRegistry"/>
    /// (lives in the additive minigame scene). Disabled for Fantasma — see <see cref="FantasmaWorldGrabInput"/>.
    /// </summary>
    public sealed class OfflineGrabInputDriver : MonoBehaviour
    {
        [SerializeField] Camera? targetCamera;
        [SerializeField] TableRuntimeRegistry? registry;
        [SerializeField] LocalMatchSession? session;

        void Awake()
        {
            targetCamera ??= Camera.main;
            session ??= FindAnyObjectByType<LocalMatchSession>();
        }

        void Update()
        {
            registry ??= FindAnyObjectByType<TableRuntimeRegistry>();

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
}
