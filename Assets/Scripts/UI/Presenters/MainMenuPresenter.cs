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

        public MainMenuPresenter(VisualElement root, MainMenuViewModel vm)
        {
            _vm = vm;
            _name = root.Q<TextField>("player-name");
            _difficulty = root.Q<DropdownField>("difficulty");
        }

        public void Bind()
        {
            _difficulty.choices = new List<string>(_vm.Difficulties);
            _difficulty.index = _vm.DifficultyIndex;
            _name.SetValueWithoutNotify(_vm.PlayerName);

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
        }
    }
}
