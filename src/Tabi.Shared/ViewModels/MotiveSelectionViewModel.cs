using System;
using MvvmHelpers;

namespace Tabi.Shared.ViewModels
{
    public class MotiveSelectionViewModel : ObservableObject
    {
        private MotiveOptionViewModel selectedMotiveOption;

        public MotiveOptionViewModel SelectedMotiveOption
        {
            get => selectedMotiveOption;
            set => SetProperty(ref selectedMotiveOption, value);
        }

        private bool shouldSave;

        public bool ShouldSave
        {
            get => shouldSave;
            set => SetProperty(ref shouldSave, value);
        }

        private bool customMotive;

        public bool CustomMotive
        {
            get => customMotive;
            set => SetProperty(ref customMotive, value);
        }
    }
}
