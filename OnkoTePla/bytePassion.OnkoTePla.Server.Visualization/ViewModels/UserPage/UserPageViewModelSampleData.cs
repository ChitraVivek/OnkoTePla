using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Server.Visualization.ViewModels.UserPage.Helper;

#pragma warning disable 0067

namespace bytePassion.OnkoTePla.Server.Visualization.ViewModels.UserPage
{
	internal class UserPageViewModelSampleData : IUserPageViewModel
    {
	    public UserPageViewModelSampleData()
	    {
		    Users = new ObservableCollection<User>
		    {			    
				new User("exampleUser1", new List<Guid> {Guid.Parse("9b95563a-039d-44b3-b95f-8ee7fabc41e3")}, "1234", Guid.Parse("f74605e6-3f54-4f08-b127-f52201d03d20"), true),
				new User("exampleUser2", new List<Guid> {Guid.Parse("9b95563a-039d-44b3-b95f-8ee7fabc41e3"),
														 Guid.Parse("d6c3e8c6-6281-4041-97ea-724a3d5379a5")}, "2345", Guid.Parse("1ca9e57c-9fee-42d9-8067-292abbfb29fb"), false),
			};

		    SelectedUser = Users.First();
		    ShowModificationView = true;

		    UserName = SelectedUser.Name;
		    Password = SelectedUser.Password;
		    IsHidden = SelectedUser.IsHidden;

			AccessablePractices = new ObservableCollection<MedPracticeListItemData>
			{
				new MedPracticeListItemData(true,  "pr1", Guid.NewGuid()),
				new MedPracticeListItemData(false, "pr2", Guid.NewGuid()),
				new MedPracticeListItemData(true,  "pr3", Guid.NewGuid())
			};
	    }

	    public ICommand AddUser		   => null;
	    public ICommand SaveChanges	   => null;
		public ICommand DiscardChanges => null;

		public ObservableCollection<User> Users { get; }
		public User SelectedUser { get; set; }
		public bool ShowModificationView { get; }

		public string UserName { get; set; }
		public string Password { get; set; }
		public bool  IsHidden  { get; set; }

		public ObservableCollection<MedPracticeListItemData> AccessablePractices { get; }

		public void Dispose() { }
        public event PropertyChangedEventHandler PropertyChanged;	    
    }
}