namespace bytePassion.OnkoTePla.Communication.NetworkMessages.RequestsAndResponses
{
	public class TryToAddNewPatientResponse : NetworkMessageBase
	{
		public TryToAddNewPatientResponse (bool addingWasSuccessful)
			: base(NetworkMessageType.TryToAddNewPatientResponse)
		{
			AddingWasSuccessful = addingWasSuccessful;
		}

		public bool AddingWasSuccessful { get; }

		public override string AsString ()
		{
			return AddingWasSuccessful.ToString();
		}

		public static TryToAddNewPatientResponse Parse (string s)
		{
			return new TryToAddNewPatientResponse(bool.Parse(s));
		}
	}
}