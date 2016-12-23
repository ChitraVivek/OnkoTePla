using System;
using System.Collections.Generic;
using bytePassion.Lib.FrameworkExtensions;
using bytePassion.Lib.TimeLib;


namespace bytePassion.OnkoTePla.Contracts.Patients
{

	public class Patient
	{
		public static readonly Patient Dummy = new Patient("", Date.Dummy, false, new Guid(), "", false);

		public Patient(string name, Date birthday, bool alive, 
					   Guid id, string externalId, bool isHidden)
		{
			Name     = name;
			Alive    = alive;
			Birthday = birthday; 
			Id       = id;
			ExternalId = externalId;
			IsHidden = isHidden;
		}

		public string Name       { get; }
		public bool   Alive      { get; }		
		public Date   Birthday   { get; }
		public Guid   Id         { get; }
		public string ExternalId { get; }
		public bool   IsHidden   { get; }


		public override string ToString    ()           => $"{Name}(*{Birthday})";
	    public override bool   Equals      (object obj) => this.Equals(obj, (patient1, patient2) => patient1.Id == patient2.Id);
	    public override int    GetHashCode ()           => Id.GetHashCode();


	    public static bool operator ==(Patient p1, Patient p2) => EqualsExtension.EqualsForEqualityOperator(p1, p2);
	    public static bool operator !=(Patient p1, Patient p2) => !(p1 == p2);

	    public static IComparer<Patient> GetNameComparer() => new PatientNameSorter();

	    private class PatientNameSorter : IComparer<Patient>
		{
			public int Compare(Patient x, Patient y)
			{
				return String.Compare(x.Name, y.Name, StringComparison.Ordinal);
			}
		}
	}
}
