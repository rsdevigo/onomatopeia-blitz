using Blitz.Gameplay;
using Blitz.UI.Presenters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blitz.UI.Views
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] LocalMatchSession? session;
        [SerializeField] VisualTreeAsset? uxml;

        HudPresenter? _presenter;

        void OnEnable()
        {
            session ??= FindAnyObjectByType<LocalMatchSession>();
            if (session is null)
                return;

            var doc = GetComponent<UIDocument>();
            ConfigureHudDocument(doc);

            if (uxml != null)
                doc.visualTreeAsset = uxml;

            var root = doc.rootVisualElement.Q("root") ?? doc.rootVisualElement;
            ConfigureHudRoot(root);

            _presenter = new HudPresenter(root, session, session as IOnomatopoeiaMatchContent);
            _presenter.Bind(root);
        }

        static void ConfigureHudDocument(UIDocument doc)
        {
            if (doc.panelSettings is null)
                return;

            doc.panelSettings.renderMode = PanelRenderMode.ScreenSpaceOverlay;
            doc.panelSettings.sortingOrder = 100;
        }

        static void ConfigureHudRoot(VisualElement root)
        {
            root.pickingMode = PickingMode.Ignore;
            root.style.flexGrow = 0;
            root.style.alignSelf = Align.FlexStart;
            root.style.maxHeight = Length.Percent(45);
            root.style.backgroundColor = new StyleColor(new Color(0.07f, 0.08f, 0.1f, 0.85f));
        }

        void OnDisable()
        {
            _presenter?.Unbind();
            _presenter = null;
        }
    }
}
