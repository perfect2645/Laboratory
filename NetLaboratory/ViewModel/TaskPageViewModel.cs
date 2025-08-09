using CommunityToolkit.Mvvm.Input;
using LeetCode.task;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utils.Wpf;

namespace NetLaboratory.ViewModel
{
    public class TaskPageViewModel : NotifyChanged
    {
        #region Properties

        public ICommand CounterCommand { get; }
        public ICommand SemaphoreCommand { get; }
        public ICommand AsyncAwaitCommand { get; }

        #endregion Properties

        public TaskPageViewModel()
        {
            CounterCommand = new RelayCommand(ExecuteCounter);
            SemaphoreCommand = new RelayCommand(ExecuteSemaphore);
            AsyncAwaitCommand = new RelayCommand(ExecuteAsyncAwait);
        }



        #region Actions

        private void ExecuteCounter()
        {
            var counter = new Counter();
            counter.StaticShow();
        }

        private void ExecuteSemaphore()
        {
            new SemaphoreTest().Run();
        }

        private void ExecuteAsyncAwait()
        {
            //new AsyncTest().Bootstrap();
            var asyncTest = new AsyncTest();
            //asyncTest.TestWait();
            //asyncTest.TestConfigAwait();
            asyncTest.TestUIThreadDeadlockAsync();
        }
        #endregion Actions
    }
}
