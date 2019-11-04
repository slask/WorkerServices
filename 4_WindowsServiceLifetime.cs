﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
#region
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endregion

namespace Microsoft.Extensions.Hosting.WindowsServices
{
    public class WindowsServiceLifetime : ServiceBase, IHostLifetime
    {
        #region
        private readonly TaskCompletionSource<object> _delayStart = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly ManualResetEventSlim _delayStop = new ManualResetEventSlim();
        private readonly HostOptions _hostOptions;

        public WindowsServiceLifetime(IHostEnvironment environment, IHostApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory, IOptions<HostOptions> optionsAccessor)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            Logger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }
            _hostOptions = optionsAccessor.Value;
        }

        private IHostApplicationLifetime ApplicationLifetime { get; }
        private IHostEnvironment Environment { get; }
        private ILogger Logger { get; }
        #endregion

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            #region logging statuses and registerign cancelation token
            cancellationToken.Register(() => _delayStart.TrySetCanceled());
            ApplicationLifetime.ApplicationStarted.Register(() =>
            {
                Logger.LogInformation("Application started. Hosting environment: {envName}; Content root path: {contentRoot}",
                    Environment.EnvironmentName, Environment.ContentRootPath);
            });
            ApplicationLifetime.ApplicationStopping.Register(() =>
            {
                Logger.LogInformation("Application is shutting down...");
            });
            ApplicationLifetime.ApplicationStopped.Register(() =>
            {
                _delayStop.Set();
            });
            #endregion

            new Thread(Run).Start(); // Otherwise this would block and prevent IHost.StartAsync from finishing.
            return _delayStart.Task;
        }

        private void Run()
        {
            try
            {
                Run(this); // This blocks until the service is stopped.
                _delayStart.TrySetException(new InvalidOperationException("Stopped without starting"));
            }
            catch (Exception ex)
            {
                _delayStart.TrySetException(ex);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Avoid deadlock where host waits for StopAsync before firing ApplicationStopped,
            // and Stop waits for ApplicationStopped.
            Task.Run(Stop);
            return Task.CompletedTask;
        }

        // Called by base.Run when the service is ready to start.
        protected override void OnStart(string[] args)
        {
            _delayStart.TrySetResult(null);
            base.OnStart(args);
        }

        // Called by base.Stop. This may be called multiple times by service Stop, ApplicationStopping, and StopAsync.
        // That's OK because StopApplication uses a CancellationTokenSource and prevents any recursion.
        protected override void OnStop()
        {
            ApplicationLifetime.StopApplication();
            // Wait for the host to shutdown before marking service as stopped.
            _delayStop.Wait(_hostOptions.ShutdownTimeout);
            base.OnStop();
        }

        #region
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _delayStop.Set();
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}