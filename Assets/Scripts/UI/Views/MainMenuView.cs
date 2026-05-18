using Blitz.Core;
using Blitz.Gameplay.Content;
using Blitz.Gameplay.Navigation;
using Blitz.UI.Presenters;
using Blitz.UI.ViewModels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blitz.UI.Views
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public sealed class MainMenuView : MonoBehaviour
    {
        [SerializeField] VisualTreeAsset? uxml;
        [SerializeField] DifficultyCatalog? _difficultyCatalog;

        MainMenuViewModel _vm = new();
        MainMenuPresenter? _presenter;

        void OnEnable()
        {
            _vm.PlayerName = PlayerPrefs.GetString(GameSessionPrefs.PlayerName, _vm.PlayerName);
            BindDifficultiesFromCatalog();
            RestoreDifficultySelection();

            var savedMinigame = PlayerPrefs.GetString(GameSessionPrefs.SelectedMinigameId, MinigameIds.BlitzOnomatopoeico);
            _vm.MinigameIndex = IndexOfMinigame(savedMinigame);

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
            var rootElement = doc != null ? doc.rootVisualElement : null;
            if (rootElement == null)
                return;

            var root = rootElement.Q("root") ?? rootElement;
            var continueBtn = root.Q<Button>("continue");
            if (continueBtn != null)
                continueBtn.clicked -= OnContinue;
        }

        void OnContinue()
        {
            PlayerPrefs.SetString(GameSessionPrefs.PlayerName, _vm.PlayerName);
            PlayerPrefs.SetString(GameSessionPrefs.SelectedDifficultyId, SelectedDifficultyId());
            PlayerPrefs.SetString(GameSessionPrefs.SelectedMinigameId, SelectedMinigameId());
            PlayerPrefs.Save();
            SceneFlow.LoadOfflineGame();
        }

        void BindDifficultiesFromCatalog()
        {
            _vm.DifficultyLabels.Clear();
            _vm.DifficultyIds.Clear();

            if (_difficultyCatalog == null)
            {
                Debug.LogWarning("MainMenuView: DifficultyCatalog not assigned; using fallback labels.");
                _vm.DifficultyLabels.Add("Fácil");
                _vm.DifficultyLabels.Add("Médio");
                _vm.DifficultyLabels.Add("Difícil");
                _vm.DifficultyIds.Add(DifficultyIds.Easy);
                _vm.DifficultyIds.Add(DifficultyIds.Medium);
                _vm.DifficultyIds.Add(DifficultyIds.Hard);
                return;
            }

            foreach (var profile in _difficultyCatalog.Entries)
            {
                if (profile == null)
                    continue;

                _vm.DifficultyLabels.Add(profile.DisplayName);
                _vm.DifficultyIds.Add(profile.DifficultyId);
            }
        }

        void RestoreDifficultySelection()
        {
            if (!PlayerPrefs.HasKey(GameSessionPrefs.SelectedDifficultyId)
                && PlayerPrefs.HasKey(GameSessionPrefs.DifficultyIndex)
                && _difficultyCatalog != null)
            {
                var legacyIndex = PlayerPrefs.GetInt(GameSessionPrefs.DifficultyIndex, 0);
                var entries = _difficultyCatalog.Entries;
                if (legacyIndex >= 0 && legacyIndex < entries.Count && entries[legacyIndex] != null)
                {
                    PlayerPrefs.SetString(
                        GameSessionPrefs.SelectedDifficultyId,
                        entries[legacyIndex].DifficultyId);
                }
            }

            var savedId = PlayerPrefs.GetString(GameSessionPrefs.SelectedDifficultyId, DifficultyIds.Easy);
            _vm.DifficultyIndex = IndexOfDifficulty(savedId);
        }

        string SelectedDifficultyId()
        {
            var ids = _vm.DifficultyIds;
            if (ids.Count == 0)
                return DifficultyIds.Easy;

            var index = Mathf.Clamp(_vm.DifficultyIndex, 0, ids.Count - 1);
            return ids[index];
        }

        string SelectedMinigameId()
        {
            var ids = _vm.MinigameIds;
            var index = Mathf.Clamp(_vm.MinigameIndex, 0, ids.Count - 1);
            return ids[index];
        }

        int IndexOfDifficulty(string id)
        {
            var ids = _vm.DifficultyIds;
            for (var i = 0; i < ids.Count; i++)
            {
                if (ids[i] == id)
                    return i;
            }

            return 0;
        }

        int IndexOfMinigame(string id)
        {
            var ids = _vm.MinigameIds;
            for (var i = 0; i < ids.Count; i++)
            {
                if (ids[i] == id)
                    return i;
            }

            return 0;
        }
    }
}
