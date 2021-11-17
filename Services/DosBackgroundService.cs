using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyWebApp.Services
{
    public class DosBackgroundService : BackgroundService
    {
        public DosBackgroundService() { }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            await ExecuteAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;   
        }
    }
}
