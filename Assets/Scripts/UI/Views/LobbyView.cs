using Blitz.Gameplay.Lobby;
using Blitz.UI.Presenters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blitz.UI.Views
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public sealed class LobbyView : MonoBehaviour
    {
        [SerializeField] LobbyServiceHost? lobby;
        [SerializeField] VisualTreeAsset? uxml;
        [SerializeField] VisualTreeAsset? seatRowUxml;

        LobbyPresenter? _presenter;

        void OnEnable()
        {
            lobby ??= FindAnyObjectByType<LobbyServiceHost>();
            if (lobby is null || seatRowUxml is null)
                return;

            var doc = GetComponent<UIDocument>();
            if (uxml != null)
                doc.visualTreeAsset = uxml;

            var root = doc.rootVisualElement.Q("root") ?? doc.rootVisualElement;
            _presenter = new LobbyPresenter(root, lobby, seatRowUxml);
            _presenter.Bind();

            var simulate = root.Q<Button>("simulate");
            var clear = root.Q<Button>("clear");
            if (simulate != null) simulate.clicked += lobby.SimulateLocalFill;
            if (clear != null) clear.clicked += lobby.Clear;
        }

        void OnDisable()
        {
            _presenter?.Unbind();
            _presenter = null;

            var doc = GetComponent<UIDocument>();
            if (lobby == null) return;

            var rootElement = doc != null ? doc.rootVisualElement : null;
            if (rootElement == null) return;

            var root = rootElement.Q("root") ?? rootElement;
            var simulate = root.Q<Button>("simulate");
            var clear = root.Q<Button>("clear");
            if (simulate != null) simulate.clicked -= lobby.SimulateLocalFill;
            if (clear != null) clear.clicked -= lobby.Clear;
        }
    }
}
