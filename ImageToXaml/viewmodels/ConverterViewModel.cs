using CommunityToolkit.Mvvm.Input;
using SharpVectors.Converters;
using SharpVectors.Renderers;
using SharpVectors.Renderers.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utils.Wpf;

namespace ImageToXaml.viewmodels
{
    public class ConverterViewModel : NotifyChanged
    {
        private WpfDrawingSettings _wpfSettings;
        public ICommand ConvertCommand { get; }

        private string? _svgPath;
        public string? SvgPath
        {
            get => _svgPath;
            set
            {
                if (value == _svgPath)
                {
                    return;
                }
                _svgPath = value;
                NotifyUI(() => SvgPath);
                OnSvgPathChanged();
            }
        }

        private string? _outputPath;
        public string? OutputPath
        {
            get => _outputPath;
            set
            {
                _outputPath = value;
                NotifyUI(() => OutputPath);
            }
        }

        public ConverterViewModel()
        {
            _wpfSettings = new WpfDrawingSettings();
            _wpfSettings.IncludeRuntime = false;
            _wpfSettings.TextAsGeometry = false;
            ConvertCommand = new RelayCommand(ConvertImageToXaml, CanExecute);
        }

        private bool CanExecute()
        {
            if (string.IsNullOrEmpty(SvgPath))
            {
                return false;
            }

            if (string.IsNullOrEmpty(OutputPath))
            {
                return false;
            }

            return true;
        }

        private void ConvertImageToXaml()
        {
            try
            {
                var converter = new FileSvgConverter(_wpfSettings);
                converter.Convert(SvgPath, OutputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during conversion: {ex.Message}");
                return;
            }
        }

        private void OnSvgPathChanged()
        {
            if (string.IsNullOrEmpty(OutputPath))
            {
                OutputPath = SvgPath?.Replace(".svg", ".xaml");
            }
        }
    }
}
