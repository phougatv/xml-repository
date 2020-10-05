namespace ConsoleApp
{
    using Microsoft.Extensions.Hosting;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class HostedService : IHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource _tokenSource;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside  of this token's cancellation
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we are creating
            _executingTask = ExecuteAsync(cancellationToken);

            // If the task is completed then return it, otherwise it is running.
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // StopAsync called with StartAsync
            if (_executingTask is null)
                return;

            // Signal cancellation to the executing method
            _tokenSource.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Derived classes should override this and execute a long running method until cancellation is requested.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
