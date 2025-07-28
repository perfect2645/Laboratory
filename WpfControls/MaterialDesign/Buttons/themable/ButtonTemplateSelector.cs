using System.Windows;
using System.Windows.Controls;

namespace WpfControls.MaterialDesign.Buttons
{
    public class ButtonTemplateSelector : DataTemplateSelector
    {
        // 各种按钮类型对应的模板
        public DataTemplate? PrimaryTemplate { get; set; }
        public DataTemplate? SecondaryTemplate { get; set; }
        public DataTemplate? DangerTemplate { get; set; }
        public DataTemplate? SuccessTemplate { get; set; }
        public DataTemplate? InfoTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // 直接使用ButtonViewModel中的ButtonType属性选择模板
            if (item is ThemableButtonViewModel buttonVM)
            {
                return buttonVM.ButtonType switch
                {
                    ButtonType.Primary => PrimaryTemplate!,
                    ButtonType.Secondary => SecondaryTemplate!,
                    ButtonType.Danger => DangerTemplate!,
                    ButtonType.Success => SuccessTemplate!,
                    ButtonType.Info => InfoTemplate!,
                    _ => base.SelectTemplate(item, container)
                };
            }

            return base.SelectTemplate(item, container);
        }
    }
}
