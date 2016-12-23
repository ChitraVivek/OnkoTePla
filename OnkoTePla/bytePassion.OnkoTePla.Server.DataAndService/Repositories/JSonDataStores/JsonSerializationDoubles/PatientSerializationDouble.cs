using System;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Server.DataAndService.Repositories.JSonDataStores.JsonSerializationDoubles
{

	internal class PatientSerializationDouble
	{									
		public PatientSerializationDouble()
		{				
		}

		public PatientSerializationDouble (Patient p)
		{
			Name       = p.Name;
			Alive      = p.Alive;
			Birthday   = new DateSerializationDouble(p.Birthday);
			Id         = p.Id;
			ExternalId = p.ExternalId;
			IsHidden   = p.IsHidden;
		}

		public string                  Name       { get; set; }
		public bool                    Alive      { get; set; }
		public DateSerializationDouble Birthday   { get; set; }
		public Guid                    Id         { get; set; }
		public string                  ExternalId { get; set; }
		public bool                    IsHidden   { get; set; }

		public Patient GetPatient()
		{
			return new Patient(Name, 
							   Birthday.GetDate(), 
							   Alive, 
							   Id, 
							   ExternalId,
							   IsHidden);
		}		
	}

}
 