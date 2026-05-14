using Blitz.UI.Presenters;
using Blitz.UI.ViewModels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blitz.UI.Views;

[DisallowMultipleComponent]
[RequireComponent(typeof(UIDocument))]
public sealed class MainMenuView : MonoBehaviour
{
    [SerializeField] VisualTreeAsset? uxml;

    MainMenuViewModel _vm = new();
    MainMenuPresenter? _presenter;

    void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        if (uxml != null)
            doc.visualTreeAsset = uxml;

        var root = doc.rootVisualElement.Q("root") ?? doc.rootVisualElement;
        _presenter = new MainMenuPresenter(root, _vm);
        _presenter.Bind();

        var continueBtn = root.Q<Button>("continue");
        if (continueBtn != null)
            continueBtn.clicked += OnContinue;
    }

    void OnDisable()
    {
        _presenter?.Unbind();
        _presenter = null;

        var doc = GetComponent<UIDocument>();
        if (doc == null) return;

        var root = doc.rootVisualElement.Q("root") ?? doc.rootVisualElement;
        var continueBtn = root.Q<Button>("continue");
        if (continueBtn != null)
            continueBtn.clicked -= OnContinue;
    }

    void OnContinue() => Debug.Log($"[MainMenu] continue as '{_vm.PlayerName}' difficulty index {_vm.DifficultyIndex}");
}
