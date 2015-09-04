﻿using System;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.OnkoTePla.Client.Core.Domain;


namespace bytePassion.OnkoTePla.Client.WPFVisualization.ViewModels.TherapyPlaceRowView.Helper
{
	public class TherapyPlaceRowIdentifier
	{
		public TherapyPlaceRowIdentifier(AggregateIdentifier placeAndDate, 
									   Guid therapyPlaceId)
		{
			PlaceAndDate      = placeAndDate;
			TherapyPlaceId = therapyPlaceId;
		}

		public AggregateIdentifier PlaceAndDate { get; }
		public Guid TherapyPlaceId { get; }

		public override bool Equals(object obj)
		{
			return this.Equals(obj,
							  (al1, al2) => al1.PlaceAndDate == al2.PlaceAndDate && 
										    al1.TherapyPlaceId == al2.TherapyPlaceId);
		}

		public override int GetHashCode()
		{
			return PlaceAndDate.GetHashCode() ^ 
				   TherapyPlaceId.GetHashCode();
		}

		public override string ToString()
		{
			return $"[{PlaceAndDate}|{TherapyPlaceId}]";
        }

		public static bool operator == (TherapyPlaceRowIdentifier al1, TherapyPlaceRowIdentifier al2) => al1.Equals(al2);
		public static bool operator != (TherapyPlaceRowIdentifier al1, TherapyPlaceRowIdentifier al2) => !(al1 == al2);
	}
}