using Blitz.Core;
using Blitz.Gameplay.Minigames;
using Blitz.Gameplay.Table;
using UnityEngine;

namespace Blitz.Gameplay.Input
{
    /// <summary>
    /// Fantasma additive scene: raycast picks call <see cref="FantasmaLadraoMinigame.TrySubmitWorldGrab"/>.
    /// Requires the core <see cref="OfflineGrabInputDriver"/> to be disabled for this minigame.
    /// </summary>
    public sealed class FantasmaWorldGrabInput : MonoBehaviour
    {
        [SerializeField] Camera? targetCamera;
        [SerializeField] TableRuntimeRegistry? registry;
        [SerializeField] FantasmaLadraoMinigame? minigame;
        [SerializeField] LocalMatchSession? session;

        void Awake()
        {
            targetCamera ??= Camera.main;
            registry ??= FindAnyObjectByType<TableRuntimeRegistry>();
            minigame ??= FindAnyObjectByType<FantasmaLadraoMinigame>();
            session ??= FindAnyObjectByType<LocalMatchSession>();
        }

        void Update()
        {
            if (session is null || registry is null || targetCamera is null || minigame is null)
                return;

            if (session.Phase != MatchPhase.GrabPhase)
                return;

            if (!UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                return;

            var pos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            if (registry.TryRaycastGrab(targetCamera, pos, out var id))
                minigame.TrySubmitWorldGrab(id);
        }
    }
}
