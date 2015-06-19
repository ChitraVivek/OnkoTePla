﻿using System.Globalization;
using bytePassion.FileRename.Enums;
using bytePassion.Lib.FrameworkExtensions;


namespace bytePassion.FileRename.Converter
{
	public class SearchTypeToBoolConverter : GenericParameterizedValueConverter<SearchType, bool, SearchType>
	{
		protected override bool Convert(SearchType searchType, SearchType chosenSearchType, CultureInfo culture)
		{
			return searchType == chosenSearchType;
		}

		protected override SearchType ConvertBack (bool buttonIsChecked, SearchType chosenSearchType, CultureInfo culture)
		{
			if (buttonIsChecked) 
				return chosenSearchType;			

			// return dummy because null is not possible
			return SearchType.Characters;
		}
	}
}