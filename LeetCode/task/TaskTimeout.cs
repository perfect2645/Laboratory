using System.Diagnostics;
using Utils.Events;
using Utils.Tasking;

namespace LeetCode.task
{
    public class TaskTimeout
    {
        public TaskTimeout() { }

        public async Task HandleTaskTimeoutAsync()
        {
            using var cts = new CancellationTokenSource();
            var task = HeavyJob(cts.Token);

            var sw = Stopwatch.StartNew();

            await task.TimeoutAsync(TimeSpan.FromSeconds(3), cts);

            LogEvents.Publish($"Task completed. {task.Status}, spends:{sw.ElapsedMilliseconds}");

        }

        private async Task HeavyJob(CancellationToken token)
        {
            try
            {
                // 模拟一个耗时的任务
                await Task.Delay(10000, token);
                LogEvents.Publish($"HeavyJob Completed.");
            }
            catch(OperationCanceledException ex)
            {
                LogEvents.Publish($"HeavyJob was cancelled: {ex.Message}");
            }
            catch (Exception ex)
            {
                LogEvents.Publish($"HeavyJob error occurred: {ex.Message}");
            }
        }
    }
}
