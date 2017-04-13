using System.ComponentModel;
using bytePassion.Lib.FrameworkExtensions;

namespace bytePassion.OnkoTePla.Client.Visualization.ViewModels
{

	public abstract class ViewModel : DisposingObject, 
                                      IViewModel                                      
    {
        public abstract event PropertyChangedEventHandler PropertyChanged;
    }
}