﻿using System.ComponentModel;


namespace bytePassion.OnkoTePla.Server.WpfUi.ViewModels.AboutPage
{
    internal class AboutPageViewModel : ViewModel, IAboutPageViewModel
    {
        protected override void CleanUp() { }
        public override event PropertyChangedEventHandler PropertyChanged;
    }
}