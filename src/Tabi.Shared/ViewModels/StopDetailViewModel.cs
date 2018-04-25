using System;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.DataStorage;

namespace Tabi.ViewModels
{
    public class StopDetailViewModel : ObservableObject
    {
        private readonly IRepoManager _repoManager;

        public StopDetailViewModel(IRepoManager repoManager)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
        }

        private Stop stop;

        public Stop Stop
        {
            get => stop;
            set => SetProperty(ref stop, value);
        }

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                SetProperty(ref title, value);
            }
        }

        public void UpdateStop()
        {
            _repoManager.StopRepository.Update(Stop);
        }

    }
}
