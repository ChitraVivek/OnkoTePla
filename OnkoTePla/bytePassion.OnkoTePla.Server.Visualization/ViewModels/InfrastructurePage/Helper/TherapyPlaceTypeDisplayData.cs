using System;
using System.ComponentModel;
using System.Windows.Media;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper
{
	public class TherapyPlaceTypeDisplayData : INotifyPropertyChanged
	{
		public TherapyPlaceTypeDisplayData(string name, ImageSource icon, Guid id)
		{
			Name = name;
			Icon = icon;
			Id = id;
		}

		public string      Name { get; }
		public ImageSource Icon { get; }
		public Guid		   Id   { get; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}