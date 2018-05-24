using System;
using MvvmHelpers;
using Tabi.DataObjects;

namespace Tabi.Shared.ViewModels
{
    public class TrackMotiveViewModel : ObservableObject
    {
        private readonly Motive _motive;

        public TrackMotiveViewModel(Motive motive)
        {
            _motive = motive ?? throw new ArgumentNullException(nameof(motive));

            ResetViewModel();
        }

        private string text;

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public Motive SaveViewModelToModel()
        {
            Motive initialModel = _motive;
            initialModel.TrackId = _motive.TrackId;
            initialModel.Text = text;
            initialModel.Timestamp = DateTimeOffset.Now;

            return initialModel;
        }

        /// <summary>
        /// Returns the ViewModel to it's original parameters based on the initial motive.
        /// </summary>
        public void ResetViewModel()
        {
            Text = _motive.Text;
        }
    }
}
