using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.MedicalPracticeSelector.Helper;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.MedicalPracticeSelector
{
	internal class MedicalPracticeSelectorViewModelSampleData : IMedicalPracticeSelectorViewModel
	{
		public MedicalPracticeSelectorViewModelSampleData()
		{
			var med1 = new MedicalPracticeDisplayData(Guid.Empty, "examplePractice 1");
			var med2 = new MedicalPracticeDisplayData(Guid.Empty, "examplePractice 1");

			SelectedMedicalPractice = med1;
			
			AvailableMedicalPractices = new ObservableCollection<MedicalPracticeDisplayData>
			{
				med1,
				med2
			};

			PracticeIsSelectable = true;
		}

		public MedicalPracticeDisplayData SelectedMedicalPractice { get; set; }
		public ObservableCollection<MedicalPracticeDisplayData> AvailableMedicalPractices { get; }
		public bool PracticeIsSelectable { get; }
		
	    public void Dispose() {	}
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
