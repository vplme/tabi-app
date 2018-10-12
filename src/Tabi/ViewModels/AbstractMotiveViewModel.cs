using System;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.Resx;

namespace Tabi.ViewModels
{
    public abstract class AbstractMotiveViewModel : ObservableObject
    {
        protected readonly Motive _motive;
        protected readonly IMotiveConfiguration _motiveConfiguration;

        public AbstractMotiveViewModel(Motive motive, IMotiveConfiguration motiveConfiguration)
        {
            _motive = motive ?? throw new ArgumentNullException(nameof(motive));
            _motiveConfiguration = motiveConfiguration ?? throw new ArgumentNullException(nameof(motiveConfiguration));
            ResetViewModel();

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Text))
                {
                    OnPropertyChanged(nameof(ConvertedText));
                }
            };
        }

        private MotiveOption FindMatchingMotiveOption(string motiveKeyId)
        {
            MotiveOption foundMotive = null;
            foreach (MotiveOption mo in _motiveConfiguration.Options)
            {
                if (mo.Id == motiveKeyId)
                {
                    foundMotive = mo;
                    break;
                }
            }

            foreach (MotiveOption mo in _motiveConfiguration.OtherOptions)
            {
                if (mo.Id == motiveKeyId)
                {
                    foundMotive = mo;
                    break;
                }
            }

            return foundMotive;
        }

        public string ConvertedText
        {
            get
            {
                MotiveOption mo = FindMatchingMotiveOption(Text);
                string result = text;
                if (mo != null)
                {
                    string translatedText = typeof(AppResources).GetProperty($"{mo.Id}MotiveText")?.GetValue(null) as string;
                    result = translatedText ?? mo.Text;
                }


                return result;
            }
        }

        private string text;

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        /// <summary>
        /// Returns the ViewModel to it's original parameters based on the initial motive.
        /// </summary>
        public void ResetViewModel()
        {
            Text = _motive.Text;
        }


        public abstract Motive SaveViewModelToModel();
    }
}

