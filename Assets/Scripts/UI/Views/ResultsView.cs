using Blitz.UI.Presenters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blitz.UI.Views;

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
        _presenter.Bind();

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

    static void OnToLeaderboard() => Debug.Log("[Results] -> leaderboard");

    static void OnToMenu() => Debug.Log("[Results] -> main menu");
}
