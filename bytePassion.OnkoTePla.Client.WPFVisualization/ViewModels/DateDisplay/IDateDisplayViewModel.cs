﻿using bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.Base;

namespace bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.DateDisplay
{
	public interface IDateDisplayViewModel : IViewModel
	{
		string Date { get; }
	}
}