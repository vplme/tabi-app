﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Tabi.DataSync;
using Tabi.Helpers;
using Tabi.iOS.Helpers;
using UIKit;
using Xamarin.Forms;

namespace Tabi.iOS.PlatformImplementations
{
    public class DataUploadTask : IDataUploadTask
    {
        private readonly TabiConfiguration _configuration;
        private nint _taskId;
        public DataUploadTask(TabiConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task Start()
        {
            _taskId = UIApplication.SharedApplication.BeginBackgroundTask("LongRunningTask", OnExpiration);

            await Task.Run(async () =>
            {
                SyncService syncService = App.Container.Resolve<SyncService>();
                int interval = _configuration.Api.SyncInterval;
                await syncService.AutoUpload(TimeSpan.FromMinutes(interval), Settings.Current.WifiOnly);
            });

            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }

        void OnExpiration()
        {
        }
    }
}

