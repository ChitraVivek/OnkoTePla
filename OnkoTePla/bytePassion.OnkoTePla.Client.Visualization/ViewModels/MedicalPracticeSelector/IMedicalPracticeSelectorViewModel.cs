using System.Collections.ObjectModel;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.MedicalPracticeSelector.Helper;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.MedicalPracticeSelector
{
	internal interface IMedicalPracticeSelectorViewModel : IViewModel
	{
		MedicalPracticeDisplayData SelectedMedicalPractice { get; set; }

		ObservableCollection<MedicalPracticeDisplayData> AvailableMedicalPractices { get; } 

		bool PracticeIsSelectable { get; }
	}
}
