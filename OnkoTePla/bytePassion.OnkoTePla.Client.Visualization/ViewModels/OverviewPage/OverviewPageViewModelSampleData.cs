using System.ComponentModel;
using System.Windows.Input;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateDisplay;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateSelector;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.GridContainer;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.MedicalPracticeSelector;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.RoomSelector;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.UndoRedoView;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.OverviewPage
{
	internal class OverviewPageViewModelSampleData : IOverviewPageViewModel
	{
		public OverviewPageViewModelSampleData()
		{
			DateDisplayViewModel             = new DateDisplayViewModelSampleData();
			MedicalPracticeSelectorViewModel = new MedicalPracticeSelectorViewModelSampleData();
			RoomFilterViewModel              = new RoomFilterViewModelSampleData();
			DateSelectorViewModel            = new DateSelectorViewModelSampleData();
			GridContainerViewModel           = new GridContainerViewModelSampleData();			
			UndoRedoViewModel                = new UndoRedoViewModelSampleData();

			ChangeConfirmationVisible = true;
			AddAppointmentPossible = true;			
		}

		public IDateDisplayViewModel             DateDisplayViewModel             { get; }
		public IMedicalPracticeSelectorViewModel MedicalPracticeSelectorViewModel { get; }
		public IRoomFilterViewModel              RoomFilterViewModel              { get; }
		public IDateSelectorViewModel            DateSelectorViewModel            { get; }
		public IGridContainerViewModel           GridContainerViewModel           { get; }	
		public IUndoRedoViewModel                UndoRedoViewModel                { get; }

		public ICommand ShowAddAppointmentDialog => null;
		public ICommand ShowPrintDialog          => null;

		public bool ChangeConfirmationVisible { get; }
		public bool AddAppointmentPossible { get; }		

        public void Dispose() { }
        public event PropertyChangedEventHandler PropertyChanged;	    
	}
}
