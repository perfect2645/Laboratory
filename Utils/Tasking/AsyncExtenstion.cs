using Logging;

namespace Utils.Tasking
{
    public static class AsyncExtenstion
    {

        public static async Task<TResult> TimeoutAsync<TResult>(this Task<TResult> task, TimeSpan timeout, CancellationTokenSource cts)
        {
            try
            {
                return await task.WaitAsync(timeout);
            }
            catch (TimeoutException)
            {
                Log4Logger.Logger.Warn($"The operation has timed out after {timeout.TotalMilliseconds} milliseconds.");
                cts.Cancel();
                return await task;
            }
        }

        public static async Task TimeoutAsync(this Task task, TimeSpan timeout, CancellationTokenSource cts)
        {
            try
            {
                await task.WaitAsync(timeout);
            }
            catch (TimeoutException)
            {
                Log4Logger.Logger.Warn($"The operation has timed out after {timeout.TotalMilliseconds} milliseconds.");
                cts.Cancel();
            }
        }
    }
}
