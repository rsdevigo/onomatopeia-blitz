using System;
using System.Collections.Generic;

namespace Blitz.Core;

public enum LobbySeatState : byte
{
    Empty = 0,
    OccupiedLocal = 1,
    OccupiedRemote = 2
}

public readonly struct LobbySeat : IEquatable<LobbySeat>
{
    public readonly byte Index;
    public readonly string DisplayName;
    public readonly bool IsReady;
    public readonly LobbySeatState State;

    public LobbySeat(byte index, string displayName, bool isReady, LobbySeatState state)
    {
        Index = index;
        DisplayName = displayName;
        IsReady = isReady;
        State = state;
    }

    public bool Equals(LobbySeat other) =>
        Index == other.Index && DisplayName == other.DisplayName && IsReady == other.IsReady && State == other.State;

    public override bool Equals(object? obj) => obj is LobbySeat other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Index, DisplayName, IsReady, State);
}

public interface ILobbyService
{
    IReadOnlyList<LobbySeat> Seats { get; }

    event Action? LobbyChanged;

    void SimulateLocalFill();

    void SetReady(byte seatIndex, bool ready);

    void Clear();
}
