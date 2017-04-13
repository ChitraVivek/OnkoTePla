using System.Windows.Input;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateDisplay;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.DateSelector;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.GridContainer;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.MedicalPracticeSelector;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.RoomSelector;
using bytePassion.OnkoTePla.Client.Visualization.ViewModels.UndoRedoView;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels.OverviewPage
{
	internal interface IOverviewPageViewModel : IViewModel
	{
		IDateDisplayViewModel             DateDisplayViewModel             { get; }
		IMedicalPracticeSelectorViewModel MedicalPracticeSelectorViewModel { get; }
		IRoomFilterViewModel              RoomFilterViewModel              { get; }
		IDateSelectorViewModel            DateSelectorViewModel            { get; }
		IGridContainerViewModel           GridContainerViewModel           { get; }			
		IUndoRedoViewModel                UndoRedoViewModel                { get; }

		ICommand ShowAddAppointmentDialog { get; }
		ICommand ShowPrintDialog          { get; }

		bool ChangeConfirmationVisible { get; }
		bool AddAppointmentPossible    { get; }		
	}
}
