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
    }
}
