using System;
using System.ComponentModel;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.UserPage.Helper
{
	internal class MedPracticeListItemData : INotifyPropertyChanged
	{
		public MedPracticeListItemData(bool isSelected, string name, Guid id)
		{
			IsSelected = isSelected;
			Name = name;
			Id = id;
		}

		public bool   IsSelected { get; set; }
		public string Name       { get; }
		public Guid   Id         { get; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
