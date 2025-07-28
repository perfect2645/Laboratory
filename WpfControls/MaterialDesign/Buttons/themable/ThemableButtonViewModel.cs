using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Utils.Wpf;

namespace WpfControls.MaterialDesign.Buttons
{
    // 自定义按钮控件，包含类型和参数属性
    public class ThemableButtonViewModel : NotifyChanged
    {
        public ICommand? ClickCommand { get; }

        private ButtonType _buttonType;
        public ButtonType ButtonType
        {
            get => _buttonType;
            set
            {
                _buttonType = value;
                NotifyUI(() => ButtonType);
            }
        }

        private string? _content;
        public string? Content
        {
            get => _content;
            set
            {
                _content = value;
                NotifyUI(() => Content);
            }
        }

        private object? _actionParameter;
        public object? ActionParameter
        {
            get => _actionParameter;
            set
            {
                _actionParameter = value;
                NotifyUI(() => ActionParameter);
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyUI(() => IsEnabled);
            }
        }

        public ThemableButtonViewModel(string content, ButtonType buttonType,
                              object actionParameter = null,
                              Action<ThemableButtonViewModel> clickAction = null)
        {
            Content = content;
            ButtonType = buttonType;
            ActionParameter = actionParameter;

            // 初始化点击命令
            ClickCommand = new RelayCommand(OnButtonClick, CanButtonClick);
        }

        private bool CanButtonClick()
        {
            return true;
        }

        private void OnButtonClick()
        {
            
        }
    }
}
