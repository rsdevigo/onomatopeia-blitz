using Blitz.Core;
using UnityEngine.UIElements;

namespace Blitz.UI.Presenters;

public sealed class LobbyPresenter
{
    readonly ILobbyService _lobby;
    readonly VisualElement _seatsRoot;
    readonly VisualTreeAsset _rowTemplate;
    readonly Label[] _names = new Label[8];
    readonly Toggle[] _readies = new Toggle[8];

    public LobbyPresenter(VisualElement root, ILobbyService lobby, VisualTreeAsset rowTemplate)
    {
        _lobby = lobby;
        _seatsRoot = root.Q("seats")!;
        _rowTemplate = rowTemplate;
    }

    public void Bind()
    {
        _seatsRoot.Clear();
        for (byte i = 0; i < 8; i++)
        {
            var rowInstance = _rowTemplate.Instantiate();
            var ve = rowInstance.Q<VisualElement>("row") ?? rowInstance;
            ve.Q<Label>("seat-index").text = i.ToString();
            _names[i] = ve.Q<Label>("seat-name");
            _readies[i] = ve.Q<Toggle>("ready");
            var seatIndex = i;
            _readies[i].RegisterValueChangedCallback(evt => _lobby.SetReady(seatIndex, evt.newValue));
            _seatsRoot.Add(ve);
        }

        _lobby.LobbyChanged += Refresh;
        Refresh();
    }

    public void Unbind()
    {
        _lobby.LobbyChanged -= Refresh;
    }

    void Refresh()
    {
        var seats = _lobby.Seats;
        for (var i = 0; i < seats.Count && i < 8; i++)
        {
            var s = seats[i];
            _names[i].text = string.IsNullOrEmpty(s.DisplayName) ? "(vazio)" : s.DisplayName;
            _readies[i].SetValueWithoutNotify(s.IsReady);
        }
    }
}
