using System;
using System.Threading.Tasks;
using Tabi.Helpers;
using Tabi.Logging;
using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi
{
    public partial class LogsPage : ContentPage
    {
        LogsViewModel ViewModel => vm ?? (vm = BindingContext as LogsViewModel);
        LogsViewModel vm;

        LogFileAccess logAccess;

        public LogsPage()
        {
            InitializeComponent();
            BindingContext = new LogsViewModel();
            logAccess = new LogFileAccess();

            Task.Run(async () => { await ReadLogFile(); });
        }

        void Refresh(object sender, EventArgs e)
        {
            Task.Run(async () => { await ReadLogFile(); });
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            FileLogWriter.LogWriteEvent += FileLogger_LogWriteEvent;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            FileLogWriter.LogWriteEvent -= FileLogger_LogWriteEvent;
        }

        
        private void FileLogger_LogWriteEvent(object sender, EventArgs e)
        {
//            Task.Run(async () =>
//            {
//                await ReadLogFile();
//            });
        }

        private async Task ReadLogFile()
        {
            string t = await logAccess.ReadFile();
            ViewModel.Text = t;
        }
    }
}