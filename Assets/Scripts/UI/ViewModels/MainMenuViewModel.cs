using System;
using System.Collections.Generic;

namespace Blitz.UI.ViewModels
{
    public sealed class MainMenuViewModel
    {
        public string PlayerName { get; set; } = "Jogador";

        public int DifficultyIndex { get; set; }

        public IReadOnlyList<string> Difficulties { get; } = new[] { "Fácil", "Médio", "Difícil" };

        public event Action? Changed;

        public void NotifyChanged() => Changed?.Invoke();
    }
}
