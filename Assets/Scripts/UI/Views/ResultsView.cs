using Blitz.Core;
using Blitz.Gameplay.Navigation;
using Blitz.UI.Presenters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blitz.UI.Views
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public sealed class ResultsView : MonoBehaviour
    {
        [SerializeField] VisualTreeAsset? uxml;

        ResultsPresenter? _presenter;

        void OnEnable()
        {
            var doc = GetComponent<UIDocument>();
            if (uxml != null)
                doc.visualTreeAsset = uxml;

            var root = doc.rootVisualElement.Q("root") ?? doc.rootVisualElement;
            _presenter = new ResultsPresenter(root);
            var score = PlayerPrefs.GetInt(GameSessionPrefs.PendingResultsScore, 0);
            _presenter.Bind(score);

            var toLb = root.Q<Button>("to-leaderboard");
            var toMenu = root.Q<Button>("to-menu");
            if (toLb != null) toLb.clicked += OnToLeaderboard;
            if (toMenu != null) toMenu.clicked += OnToMenu;
        }

        void OnDisable()
        {
            _presenter?.Unbind();
            _presenter = null;

            var doc = GetComponent<UIDocument>();
            if (doc == null) return;

            var root = doc.rootVisualElement.Q("root") ?? doc.rootVisualElement;
            var toLb = root.Q<Button>("to-leaderboard");
            var toMenu = root.Q<Button>("to-menu");
            if (toLb != null) toLb.clicked -= OnToLeaderboard;
            if (toMenu != null) toMenu.clicked -= OnToMenu;
        }

        static void OnToLeaderboard() => SceneFlow.LoadLeaderboard();

        static void OnToMenu() => SceneFlow.LoadMainMenu();
    }
}
