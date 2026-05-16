using System;
using Blitz.Core;
using Unity.Netcode;
using UnityEngine;

namespace Blitz.Netcode
{
    /// <summary>
    /// Server-spawned session object: replicates phase + score and validates grabs on the server.
    /// Add alongside a NetworkObject on the same GameObject.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NetMatchSession : NetworkBehaviour
    {
        readonly NetworkVariable<byte> _phase =
            new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        readonly NetworkVariable<int> _score =
            new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        readonly AnswerResolver _resolver = new();

        GeneratedCard? _serverCard;
        ActiveOnomatopoeiaSet? _serverSet;

        public byte ReplicatedPhase => _phase.Value;

        public int ReplicatedScore => _score.Value;

        public override void OnNetworkSpawn()
        {
            _phase.OnValueChanged += OnPhaseChanged;
            _score.OnValueChanged += OnScoreChanged;
        }

        public override void OnNetworkDespawn()
        {
            _phase.OnValueChanged -= OnPhaseChanged;
            _score.OnValueChanged -= OnScoreChanged;
        }

        void OnPhaseChanged(byte previous, byte current) =>
            Debug.Log($"[NetMatchSession] phase {previous} -> {current}");

        void OnScoreChanged(int previous, int current) =>
            Debug.Log($"[NetMatchSession] score {previous} -> {current}");

        /// <summary>Host-only convenience to seed a round for RPC tests.</summary>
        public void ServerBeginStubRound(int seed)
        {
            if (!IsServer)
                return;

            var rng = new Random(seed);
            var set = ActiveOnomatopoeiaSet.CreateSyntheticDevSet(rng);
            var gen = new CardGenerator(seed);
            if (!gen.TryGenerateCard(set, out var card))
                return;

            _serverCard = card;
            _serverSet = set;
            _phase.Value = (byte)MatchPhase.GrabPhase;
        }

        [Rpc(SendTo.Server)]
        public void SubmitGrabRpc(byte slotIndex, RpcParams rpcParams = default)
        {
            if (!IsServer)
                return;

            if (slotIndex > 2)
                return;

            if (_serverCard is null || _serverSet is null)
                return;

            var correct = _resolver.Resolve(_serverCard.Value, _serverSet.Value);
            if (correct.Slot == slotIndex)
                _score.Value += 1;

            _phase.Value = (byte)MatchPhase.ResolveRound;
        }

        public void ServerResetScore()
        {
            if (!IsServer)
                return;

            _score.Value = 0;
            _phase.Value = (byte)MatchPhase.MatchInit;
        }
    }
}
