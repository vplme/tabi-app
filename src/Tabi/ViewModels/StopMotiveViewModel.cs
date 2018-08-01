using System;
using Tabi.DataObjects;

namespace Tabi.ViewModels
{
    public class StopMotiveViewModel : AbstractMotiveViewModel
    {
        public StopMotiveViewModel(Motive motive, MotiveConfiguration motiveConfiguration) : base(motive, motiveConfiguration)
        {
        }

        public override Motive SaveViewModelToModel()
        {
            Motive initialModel = _motive;
            initialModel.StopVisitId = _motive.StopVisitId;
            initialModel.Text = Text;
            initialModel.Timestamp = DateTimeOffset.Now;

            return initialModel;
        }
    }
}
