using System;
using System.ComponentModel;
using bytePassion.Lib.FrameworkExtensions;

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.InfrastructurePage.Helper
{
	public class MedPracticeDisplayData : INotifyPropertyChanged
	{
		private string name;

		public MedPracticeDisplayData (string name, Guid id)
		{
			this.name = name;			
			Id = id;
		}

		public string Name
		{
			get { return name; }
			set { PropertyChanged.ChangeAndNotify(this, ref name, value); }
		}
		public Guid	Id { get; }		

		public event PropertyChangedEventHandler PropertyChanged;
	}
}