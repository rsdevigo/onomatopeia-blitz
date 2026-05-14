using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Blitz.UI.Presenters;

public sealed class LeaderboardPresenter
{
    readonly ListView _list;
    readonly List<string> _rows = new();

    public LeaderboardPresenter(VisualElement root)
    {
        _list = root.Q<ListView>("entries")!;
    }

    public void Bind()
    {
        _rows.Clear();
        for (var i = 1; i <= 10; i++)
            _rows.Add($"{i}. DemoPlayer — {1200 - i * 37} pts");

        _list.makeItem = () => new Label();
        _list.bindItem = (e, i) =>
        {
            if (e is Label l)
                l.text = _rows[i];
        };

        _list.itemsSource = _rows;
        _list.Rebuild();
    }
}
