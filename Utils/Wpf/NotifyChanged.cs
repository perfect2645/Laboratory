using System.ComponentModel;
using System.Linq.Expressions;

namespace Utils.Wpf
{
    public class NotifyChanged : INotifyPropertyChanged
    {
        protected virtual void NotifyUI(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void NotifyUI<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = (propertyExpression.Body as MemberExpression)?.Member.Name;
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }
            NotifyUI(propertyName);
        }

        public virtual event PropertyChangedEventHandler? PropertyChanged;
    }
}
