﻿using System.ComponentModel;

namespace bytePassion.OnkoTePla.Server.WpfUi.ViewModels.OptionsPage
{
    internal class OptionsPageViewModel : ViewModel, 
                                          IOptionsPageViewModel
    {
        protected override void CleanUp() { }
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}