using Logging;
using System.Threading.Tasks;

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

        public static async void SafeFireAndForget(this Task task, Action? onCompleted = null,
            Action<Exception>? onError = null)
        {
            try
            {
                await task;
                onCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }

        public static async void SafeFireAndForget<T>(this Task<T> task, Action<T>? onCompleted = null,
            Action<Exception>? onError = null) where T : notnull
        {
            try
            {
                var result = await task;
                onCompleted?.Invoke(result);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }
    }
}
