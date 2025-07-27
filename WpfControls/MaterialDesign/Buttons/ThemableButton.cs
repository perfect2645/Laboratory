using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControls.MaterialDesign.Buttons
{
    public class ThemableButton : Button
    {
        // 依赖属性：允许自定义图标
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ThemableButton));

        // 依赖属性：允许自定义徽章文本
        public string BadgeText
        {
            get => (string)GetValue(BadgeTextProperty);
            set => SetValue(BadgeTextProperty, value);
        }

        public static readonly DependencyProperty BadgeTextProperty =
            DependencyProperty.Register("BadgeText", typeof(string), typeof(ThemableButton));

        // 依赖属性：允许自定义圆角
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ThemableButton));

        static ThemableButton()
        {
            // 重写默认样式的元数据
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThemableButton),
                new FrameworkPropertyMetadata(typeof(ThemableButton)));
        }

        // 共用功能方法
        public void CommonButtonFunction()
        {
            // 所有按钮都需要的共用逻辑
        }
    }
}
