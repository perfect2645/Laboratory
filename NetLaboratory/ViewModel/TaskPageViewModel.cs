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
        private Counter counter;


        #endregion Properties

        public TaskPageViewModel()
        {
            counter = new Counter();
            CounterCommand = new RelayCommand(ExecuteCounter);
        }

        #region Actions

        private void ExecuteCounter()
        {
            //counter.Show();
            counter.StaticShow();
        }
        #endregion Actions
    }
}
