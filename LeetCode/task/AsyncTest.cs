using System.Diagnostics;

namespace LeetCode.task
{
    public class AsyncTest
    {
        public AsyncTest()
        {
            //TestAsyncFor();
            RandomTest();
        }

        private void TestAsyncFor()
        {
            for (int i = 0; i < 300; i ++)
            {
                var task = Task.Run(() =>
                {
                    Debug.WriteLine($"Task {i} started.");
                    Debug.WriteLine($"Task {i} completed.");
                });
            }
        }

        private void RandomTest()
        {
            Debug.WriteLine(new Random().Next(1, 100));
            Debug.WriteLine(new Random().Next(1, 100));
            Debug.WriteLine(new Random().Next(1, 100));
            Debug.WriteLine(new Random().Next(1, 100));
            Debug.WriteLine(new Random().Next(1, 100));
        }
    }
}
