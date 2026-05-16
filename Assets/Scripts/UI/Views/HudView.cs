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
            if (uxml != null)
                doc.visualTreeAsset = uxml;

            var root = doc.rootVisualElement.Q("root") ?? doc.rootVisualElement;
            _presenter = new HudPresenter(root, session, session as IOnomatopoeiaMatchContent);
            _presenter.Bind(root);
        }

        void OnDisable()
        {
            _presenter?.Unbind();
            _presenter = null;
        }
    }
}
