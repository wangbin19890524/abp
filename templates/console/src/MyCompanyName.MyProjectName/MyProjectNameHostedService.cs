using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace MyCompanyName.MyProjectName
{
    public class MyProjectNameHostedService : IHostedService
    {
        private readonly IAbpApplicationWithExternalServiceProvider _application;
        private readonly IServiceProvider _serviceProvider;
        private readonly HelloWorldService _helloWorldService;

        public MyProjectNameHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            IServiceProvider serviceProvider,
            HelloWorldService helloWorldService)
        {
            _application = application;
            _serviceProvider = serviceProvider;
            _helloWorldService = helloWorldService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _application.Initialize(_serviceProvider);
            await Task.Delay(100, cancellationToken);

            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine($"START_TEST:\t {i}");
                await _helloWorldService.SayHello(_application);
                Console.WriteLine("");
            }
            Console.WriteLine($"END_TEST_____________________________");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _application.Shutdown();

            return Task.CompletedTask;
        }
    }
}
