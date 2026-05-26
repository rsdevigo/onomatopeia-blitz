using System.Collections.Generic;
using Blitz.Core;
using UnityEngine.UIElements;

namespace Blitz.UI.Presenters
{
    public sealed class LeaderboardPresenter
    {
        readonly ListView _list;
        readonly List<string> _rows = new();

        public LeaderboardPresenter(VisualElement root, ILeaderboardRepository? repository)
        {
            _list = root.Q<ListView>("entries")!;
            Bind(repository);
        }

        public void Bind(ILeaderboardRepository? repository)
        {
            _rows.Clear();

            if (repository != null)
            {
                var entries = repository.LoadTop(LeaderboardConstants.DefaultTopCount);
                for (var i = 0; i < entries.Count; i++)
                {
                    var e = entries[i];
                    _rows.Add(FormatRow(i + 1, e));
                }
            }

            if (_rows.Count == 0)
                _rows.Add("Nenhuma pontuação registada ainda.");

            _list.makeItem = () => new Label();
            _list.bindItem = (e, i) =>
            {
                if (e is Label l)
                    l.text = _rows[i];
            };

            _list.itemsSource = _rows;
            _list.Rebuild();
        }

        static string FormatRow(int rank, LeaderboardEntry entry)
        {
            var minigame = string.IsNullOrEmpty(entry.MinigameId) ? "" : $" · {entry.MinigameId}";
            var difficulty = string.IsNullOrEmpty(entry.DifficultyId) ? "" : $" · {entry.DifficultyId}";
            return $"{rank}. {entry.Name} — {entry.Score} pts{minigame}{difficulty}";
        }
    }
}
