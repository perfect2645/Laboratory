using LeetCode.enumerable;
using LeetCode.numbers;
using System.Windows;
using System.Windows.Controls;

namespace NetLaboratory.View
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Page
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void PrimeCounter_Click(object sender, RoutedEventArgs e)
        {
            new PrimeCounter();
        }

        private void DuplicateRemoval_Click(object sender, RoutedEventArgs e)
        {
            new DuplicateRemoval();
        }

        private void CenterIndex_Click(object sender, RoutedEventArgs e)
        {
            new CenterIndex();
        }

        private void TargetSum_Click(object sender, RoutedEventArgs e)
        {
            new TargetSum();
        }

        private void AsyncTest_Click(object sender, RoutedEventArgs e)
        {
            new LeetCode.task.AsyncTest();
        }

        private void FibonacciSequence_Click(object sender, RoutedEventArgs e)
        {
            new LeetCode.numbers.FibonacciSequence();
        }
    }
}
