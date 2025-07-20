using System.Windows;
using System.Windows.Controls;

namespace WpfControls.MaterialDesign
{
    /// <summary>
    /// PowerButton.xaml 的交互逻辑
    /// </summary>
    public partial class PowerButton : UserControl
    {
        #region Properties

        public PowerButtonViewModel ViewModel
        {
            get => (PowerButtonViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(PowerButtonViewModel), typeof(PowerButton),
                new PropertyMetadata(null, (d, e) =>
                {
                    var control = (PowerButton)d;
                    control.DataContext = e.NewValue;
                }));

        #endregion Properties
        public PowerButton()
        {
            InitializeComponent();
            if (ViewModel == null)
            {
                ViewModel = new PowerButtonViewModel();
            }
        }
    }
}
