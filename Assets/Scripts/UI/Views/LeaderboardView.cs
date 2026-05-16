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
            _presenter = new LeaderboardPresenter(root);
            _presenter.Bind();

            var back = root.Q<Button>("back");
            if (back != null)
                back.clicked += OnBack;
        }

        void OnDisable()
        {
            var doc = GetComponent<UIDocument>();
            if (doc == null) return;

            var root = doc.rootVisualElement.Q("root") ?? doc.rootVisualElement;
            var back = root.Q<Button>("back");
            if (back != null)
                back.clicked -= OnBack;

            _presenter = null;
        }

        static void OnBack() => SceneFlow.LoadMainMenu();
    }
}
