using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blitz.Core;

/// <summary>
/// Fake latency + deterministic local seat fill for UI shell development.
/// </summary>
public sealed class LobbyServiceStub : ILobbyService
{
    readonly object _lock = new();
    LobbySeat[] _seats = CreateEmptySeats();

    public IReadOnlyList<LobbySeat> Seats
    {
        get
        {
            lock (_lock)
            {
                return Array.AsReadOnly((LobbySeat[])_seats.Clone());
            }
        }
    }

    public event Action? LobbyChanged;

    public async void SimulateLocalFill()
    {
        await Task.Delay(1000).ConfigureAwait(true);
        lock (_lock)
        {
            for (byte i = 0; i < 8; i++)
            {
                var name = i == 0 ? "Host (you)" : $"Guest {i}";
                var state = i == 0 ? LobbySeatState.OccupiedLocal : LobbySeatState.OccupiedRemote;
                _seats[i] = new LobbySeat(i, name, i == 0, state);
            }
        }

        LobbyChanged?.Invoke();
    }

    public void SetReady(byte seatIndex, bool ready)
    {
        if (seatIndex > 7) return;

        lock (_lock)
        {
            var s = _seats[seatIndex];
            _seats[seatIndex] = new LobbySeat(s.Index, s.DisplayName, ready, s.State);
        }

        LobbyChanged?.Invoke();
    }

    public void Clear()
    {
        lock (_lock)
        {
            _seats = CreateEmptySeats();
        }

        LobbyChanged?.Invoke();
    }

    static LobbySeat[] CreateEmptySeats()
    {
        var arr = new LobbySeat[8];
        for (byte i = 0; i < 8; i++)
            arr[i] = new LobbySeat(i, string.Empty, false, LobbySeatState.Empty);
        return arr;
    }
}
