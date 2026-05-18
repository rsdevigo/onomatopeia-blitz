using System;
using System.Collections.Generic;

namespace Blitz.UI.ViewModels
{
    public sealed class MainMenuViewModel
    {
        public string PlayerName { get; set; } = "Jogador";

        public int DifficultyIndex { get; set; }

        public List<string> DifficultyLabels { get; } = new();

        public List<string> DifficultyIds { get; } = new();

        public int MinigameIndex { get; set; }

        public IReadOnlyList<string> Minigames { get; } = new[] { "Blitz Onomatopoeico", "Fantasma Ladrão" };

        public IReadOnlyList<string> MinigameIds { get; } = new[] { Core.MinigameIds.BlitzOnomatopoeico, Core.MinigameIds.FantasmaLadrao };

        public event Action? Changed;

        public void NotifyChanged() => Changed?.Invoke();
    }
}
