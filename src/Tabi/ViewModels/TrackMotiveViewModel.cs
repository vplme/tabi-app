using System;
using Tabi.DataObjects;

namespace Tabi.ViewModels
{
    public class TrackMotiveViewModel : AbstractMotiveViewModel
    {
        public TrackMotiveViewModel(Motive motive, IMotiveConfiguration motiveConfiguration) : base(motive, motiveConfiguration)
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
