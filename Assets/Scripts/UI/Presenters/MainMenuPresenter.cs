using System.Collections.Generic;
using Blitz.UI.ViewModels;
using UnityEngine.UIElements;

namespace Blitz.UI.Presenters
{
    public sealed class MainMenuPresenter
    {
        readonly MainMenuViewModel _vm;
        readonly TextField _name;
        readonly DropdownField _difficulty;
        readonly DropdownField? _minigame;

        public MainMenuPresenter(VisualElement root, MainMenuViewModel vm)
        {
            _vm = vm;
            _name = root.Q<TextField>("player-name");
            _difficulty = root.Q<DropdownField>("difficulty");
            _minigame = root.Q<DropdownField>("minigame");
        }

        public void Bind()
        {
            _difficulty.choices = new List<string>(_vm.DifficultyLabels);
            _difficulty.index = _vm.DifficultyIndex;
            _name.SetValueWithoutNotify(_vm.PlayerName);

            if (_minigame != null)
            {
                _minigame.choices = new List<string>(_vm.Minigames);
                _minigame.index = _vm.MinigameIndex;
            }

            _name.RegisterValueChangedCallback(evt =>
            {
                _vm.PlayerName = evt.newValue;
                _vm.NotifyChanged();
            });

            _difficulty.RegisterValueChangedCallback(evt =>
            {
                _vm.DifficultyIndex = _difficulty.index;
                _vm.NotifyChanged();
            });

            _minigame?.RegisterValueChangedCallback(_ =>
            {
                _vm.MinigameIndex = _minigame.index;
                _vm.NotifyChanged();
            });

            _vm.Changed += SyncFromVm;
        }

        public void Unbind()
        {
            _vm.Changed -= SyncFromVm;
        }

        void SyncFromVm()
        {
            _name.SetValueWithoutNotify(_vm.PlayerName);
            _difficulty.index = _vm.DifficultyIndex;
            if (_minigame != null)
                _minigame.index = _vm.MinigameIndex;
        }
    }
}
