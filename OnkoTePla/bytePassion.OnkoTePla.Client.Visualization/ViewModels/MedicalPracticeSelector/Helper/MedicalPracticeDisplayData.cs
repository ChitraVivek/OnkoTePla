using System;
using System.ComponentModel;
using bytePassion.Lib.FrameworkExtensions;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.MedicalPracticeSelector.Helper
{
	internal class MedicalPracticeDisplayData : INotifyPropertyChanged
	{
		private string practiceName;

		public MedicalPracticeDisplayData(Guid medicalPracticeId, string initialName)
		{
			MedicalPracticeId = medicalPracticeId;
			PracticeName = initialName;
		}

		public Guid MedicalPracticeId { get; }

		public string PracticeName
		{
			get { return practiceName; }
			set { PropertyChanged.ChangeAndNotify(this, ref practiceName, value); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
