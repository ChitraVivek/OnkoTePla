using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Repository;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Server.DataAndService.Repositories.XmlDataStores
{

	public class XmlPatientDataStore : IPersistenceService<IEnumerable<Patient>>
	{

		private readonly string filename;

		public XmlPatientDataStore(string filename)
		{
			this.filename = filename;
		}

		private static XmlWriterSettings WriterSettings
		{
			get
			{
				return new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "  ",
					NewLineChars = "\r\n",
					NewLineHandling = NewLineHandling.Replace
				};
			}
		}

		private const string XmlRoot = "patients";
		
		private const string Patient               = "patient";
		private const string NameAttribute         = "name";
		private const string BirthdayAttribute     = "birthday";
		private const string LivingStatusAttribute = "livingStatus";
		private const string IdAttribute           = "id";
		private const string ExternalIdAttribute   = "externalId";
		private const string IsHiddenAttribute     = "isHidden";
		
		public void Persist(IEnumerable<Patient> data)
		{
		
			var writer = XmlWriter.Create(filename, WriterSettings);

			writer.WriteStartDocument();
			
				writer.WriteStartElement(XmlRoot);				

					foreach (var patient in data)
					{
						WritePatient(writer, patient);
					}

				writer.WriteEndElement();

			writer.WriteEndDocument();			
			writer.Close();
		}

		private void WritePatient(XmlWriter writer, Patient patient)
		{
			writer.WriteStartElement(Patient);

			writer.WriteAttributeString(NameAttribute,         patient.Name);
			writer.WriteAttributeString(BirthdayAttribute,     patient.Birthday.ToString());
			writer.WriteAttributeString(LivingStatusAttribute, patient.Alive.ToString());
			writer.WriteAttributeString(IdAttribute,           patient.Id.ToString());	
			writer.WriteAttributeString(ExternalIdAttribute,   patient.ExternalId);
			writer.WriteAttributeString(IsHiddenAttribute,     patient.IsHidden.ToString());
				
			writer.WriteEndElement();		
		}

		public IEnumerable<Patient> Load()
		{
			IList<Patient> patients = new List<Patient>();

			if (!File.Exists(filename))
				return patients;

			var reader = XmlReader.Create(filename);

			while (reader.Read())
			{
				if (reader.NodeType != XmlNodeType.Element || reader.Name != XmlRoot) continue;

				while (reader.Read())
				{
					if (reader.NodeType != XmlNodeType.Element || reader.Name != Patient) continue;
					if (!reader.HasAttributes) continue;

					var name         = string.Empty;
					var birthday     = Date.Dummy;
					var livingStatus = false;
					var id           = new Guid();
					var externalId   = string.Empty;
					var isHidden     = false;
						
					while (reader.MoveToNextAttribute())
					{
						switch (reader.Name)
						{
							case NameAttribute:         name         =            reader.Value;  break;
							case BirthdayAttribute:     birthday     = Date.Parse(reader.Value); break;
							case LivingStatusAttribute: livingStatus = bool.Parse(reader.Value); break;
							case IdAttribute:           id           = Guid.Parse(reader.Value); break;
							case ExternalIdAttribute:   externalId   =            reader.Value;  break;
							case IsHiddenAttribute:     isHidden     = bool.Parse(reader.Value); break;
						}
					}

					patients.Add(new Patient(name, birthday, livingStatus, id, externalId, isHidden));
				}
			}
			reader.Close();

			return patients;
		}
	}
}
