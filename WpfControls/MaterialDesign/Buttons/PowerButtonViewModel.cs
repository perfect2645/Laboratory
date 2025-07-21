using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Utils.Enumerable;
using Utils.Wpf;
using WpfControls.ContextMenu;

namespace WpfControls.MaterialDesign
{
    public partial class PowerButtonViewModel : NotifyChanged, IContextMenu
    {
        #region Init

        private PowerMode _powerMode;

        public PowerMode PowerMode
        {
            get { return _powerMode; }
            set
            {
                _powerMode = value;
                NotifyUI(() => PowerMode);
            }
        }

        public PowerButtonViewModel()
        {
            InitContextMenu();
        }

        private void InitContextMenu()
        {
            ContextMenus = new ObservableCollection<IContextMenuItem>();
            ContextMenus.Add(new ContextMenuItem
            {
                Header = "Power Off",
                Command = new AsyncRelayCommand<PowerMode>(ExecutePowerChanged, CanExecutePowerChanged),
                CommandParameter = PowerMode.Off
            });
            ContextMenus.Add(new ContextMenuItem
            {
                Header = "Power On",
                Command = new AsyncRelayCommand<PowerMode>(ExecutePowerChanged, CanExecutePowerChanged),
                CommandParameter = PowerMode.On
            });
            ContextMenus.Add(new ContextMenuItem
            {
                Header = "Disabled",
                Command = new AsyncRelayCommand<PowerMode>(ExecutePowerChanged, CanExecutePowerChanged),
                CommandParameter = PowerMode.Disabled
            });

        }

        #endregion Init

        #region IContextMenu

        private ObservableCollection<IContextMenuItem>? _contextMenus;
        public ObservableCollection<IContextMenuItem>? ContextMenus
        {
            get { return _contextMenus; }
            set
            {
                _contextMenus = value;
                NotifyUI(() => ContextMenus);
            }
        }

        public IContextMenuItem? GetContextMenuItem(string key)
        {
            if (!_contextMenus.HasItem())
            {
                return null;
            }

            return _contextMenus!.FirstOrDefault(item => item.Header == key);
        }

        public void AddMenuItem(IContextMenu contextMenu)
        {
        }

        public bool RemoveMenuItem(IContextMenu contextMenu)
        {
            if (!_contextMenus.HasItem())
            {
                return false;
            }
            var itemToRemove = _contextMenus!.FirstOrDefault(item => item.Header == contextMenu?.ContextMenus?.FirstOrDefault()?.Header);
            if (itemToRemove != null)
            {
                _contextMenus!.Remove(itemToRemove);
                return true;
            }
            return false;
        }

        private async Task ExecutePowerChanged(PowerMode powerMode)
        {
            await Task.Delay(1000);
            PowerMode = powerMode;
        }

        private bool CanExecutePowerChanged(PowerMode powerMode)
        {
            return powerMode != PowerMode;
        }

        #endregion IContextMenu
    }

    public enum PowerMode
    {
        Disabled = -1,
        Off = 0,
        On = 1,
    }
}
