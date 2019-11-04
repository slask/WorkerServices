﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
#region
using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging.EventLog;
#endregion
namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extension methods for setting up WindowsServiceLifetime.
    /// </summary>
    public static class WindowsServiceLifetimeHostBuilderExtensions
    {
        /// <summary>
        /// Sets the host lifetime to WindowsServiceLifetime, sets the Content Root,
        /// and enables logging to the event log with the application name as the default source name.
        /// </summary>
        /// <remarks>
        /// This is context aware and will only activate if it detects the process is running
        /// as a Windows Service.
        /// </remarks>
        /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to operate on.</param>
        /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder UseWindowsService(this IHostBuilder hostBuilder)
        {
            if (WindowsServiceHelpers.IsWindowsService())
            {
                // Host.CreateDefaultBuilder uses CurrentDirectory for VS scenarios, but CurrentDirectory for services is c:\Windows\System32.
                hostBuilder.UseContentRoot(AppContext.BaseDirectory);
                hostBuilder.ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddEventLog();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostLifetime, WindowsServiceLifetime>();
                    #region
                    services.Configure<EventLogSettings>(settings =>
                    {
                        if (string.IsNullOrEmpty(settings.SourceName))
                        {
                            settings.SourceName = hostContext.HostingEnvironment.ApplicationName;
                        }
                    });
                    #endregion
                });
            }

            return hostBuilder;
        }
    }
}