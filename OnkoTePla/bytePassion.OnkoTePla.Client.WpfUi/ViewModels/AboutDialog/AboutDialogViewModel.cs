﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bytePassion.Lib.WpfLib.Commands;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.WpfUi.ViewModels.AboutDialog
{
	internal class AboutDialogViewModel : ViewModel, IAboutDialogViewModel
    {
        public AboutDialogViewModel(string versionNumber)
        {
            VersionNumber = versionNumber;
            CloseDialog = new Command(CloseWindow);
        }

        private static void CloseWindow()
        {
            var windows = Application.Current.Windows
                                             .OfType<Views.AboutDialog>()
                                             .ToList();

            if (windows.Count == 1)
                windows[0].Close();
            else
                throw new Exception("inner error");
        }

        public ICommand CloseDialog { get; }
        public string VersionNumber { get; }

        protected override void CleanUp() { }
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}
