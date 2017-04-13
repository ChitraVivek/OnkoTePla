using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using bytePassion.Lib.WpfLib.Commands;
using MahApps.Metro;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.OptionsPage
{
	public class AccentColorMenuData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }


        public ICommand ChangeAccentCommand { get; }

        public AccentColorMenuData()
        {
            ChangeAccentCommand = new Command(() =>
            {
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                var accent = ThemeManager.GetAccent(Name);
                ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
            }, () => true);
        }
    }

    public class OptionsPageViewModel : ViewModel, 
                                        IOptionsPageViewModel
    {
        public List<AccentColorMenuData> AccentColors { get; set; }
        public ICommand ChangeAccent { get; }

        public OptionsPageViewModel()
        {
            AccentColors = ThemeManager.Accents
                .Select(
                    a =>
                        new AccentColorMenuData() {Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush})
                .ToList();
        }

        protected override void CleanUp() { }
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}