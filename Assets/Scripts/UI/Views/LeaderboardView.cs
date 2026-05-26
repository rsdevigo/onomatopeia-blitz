using Blitz.Core;
using Blitz.Gameplay.Navigation;
using Blitz.UI.Presenters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blitz.UI.Views
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public sealed class LeaderboardView : MonoBehaviour
    {
        [SerializeField] VisualTreeAsset? uxml;

        LeaderboardPresenter? _presenter;

        void OnEnable()
        {
            var doc = GetComponent<UIDocument>();
            if (uxml != null)
                doc.visualTreeAsset = uxml;

            var root = doc.rootVisualElement.Q("root") ?? doc.rootVisualElement;
            LeaderboardServices.TryGetRepository(out var repository);
            if (repository is null)
                Debug.LogWarning("[LeaderboardView] No ILeaderboardRepository registered. Add LeaderboardBootstrap to the menu scene.");

            _presenter = new LeaderboardPresenter(root, repository);

            var back = root.Q<Button>("back");
            if (back != null)
                back.clicked += OnBack;
        }

        void OnDisable()
        {
            var doc = GetComponent<UIDocument>();
            var rootElement = doc != null ? doc.rootVisualElement : null;
            if (rootElement == null) return;

            var root = rootElement.Q("root") ?? rootElement;
            var back = root.Q<Button>("back");
            if (back != null)
                back.clicked -= OnBack;

            _presenter = null;
        }

        static void OnBack() => SceneFlow.LoadMainMenu();
    }
}
