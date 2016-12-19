using System.Linq;
using System.Windows;
using bytePassion.OnkoTePla.Resources.UserNotificationService;

namespace bytePassion.OnkoTePla.Resources
{
	public static class NameChecker
	{
		public static bool CheckName (string name)
		{
			return GlobalConstants.ForbiddenCharacters.All(forbiddenCharacter => !name.Contains(forbiddenCharacter));
		}

		public static string GetNameWithoutForbittenCharacters(string s)
		{
			var forbittenCharacter = GlobalConstants.ForbiddenCharacters.Select(c => c.ToString());

			var result = s;

			foreach (var forbiddenChar in forbittenCharacter)
			{
				result = result.Replace(forbiddenChar, string.Empty);
			}

			return result;
		}

		public static async void ShowCharacterError (string name)
		{
			var dialog = new UserDialogBox("Error",
										   $"Der Name \"{name}\" enthält unzulässige zeichen!\n"+
											"Die Operation wurde abgebrochen\n\n" +
											"Unzulässige Zeichen sind: | ; , : . # ( [ { } ] )",
										   MessageBoxButton.OK);

			await dialog.ShowMahAppsDialog();
		}
	}
}
