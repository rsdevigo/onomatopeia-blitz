using System;
using System.Collections.Generic;
using Blitz.Core;
using UnityEngine;

namespace Blitz.Gameplay.Lobby;

public sealed class LobbyServiceHost : MonoBehaviour, ILobbyService
{
    readonly LobbyServiceStub _inner = new();

    public IReadOnlyList<LobbySeat> Seats => _inner.Seats;

    public event Action? LobbyChanged
    {
        add => _inner.LobbyChanged += value;
        remove => _inner.LobbyChanged -= value;
    }

    public void SimulateLocalFill() => _inner.SimulateLocalFill();

    public void SetReady(byte seatIndex, bool ready) => _inner.SetReady(seatIndex, ready);

    public void Clear() => _inner.Clear();
}
