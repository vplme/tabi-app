using System;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.Shared.Resx;

namespace Tabi.Shared.ViewModels
{
    public class TrackMotiveViewModel : AbstractMotiveViewModel
    {
        public TrackMotiveViewModel(Motive motive, MotiveConfiguration motiveConfiguration) : base(motive, motiveConfiguration)
        {
        }

        public override Motive SaveViewModelToModel()
        {
            Motive initialModel = _motive;
            initialModel.TrackId = _motive.TrackId;
            initialModel.Text = Text;
            initialModel.Timestamp = DateTimeOffset.Now;

            return initialModel;
        }
    }
}
