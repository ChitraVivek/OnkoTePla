using System;
using System.Collections.Generic;
using bytePassion.Lib.TimeLib;
using bytePassion.Lib.Types.Communication;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Domain;
using bytePassion.OnkoTePla.Contracts.Domain.Events.Base;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;

namespace bytePassion.OnkoTePla.Server.DataAndService.Data
{
	public interface IDataCenter
	{		
		IReadOnlyList<Address> GetAllAvailableAddresses();

		IEnumerable<Patient> GetAllPatients();
		bool AddPatient(Patient newPatient);
			
		IEnumerable<User> GetAllUsers();
		void AddNewUser(User newUser);
		void UpdateUser(User updatedUser);
		User GetUser(Guid id);

		IEnumerable<Label> GetAllLabels();
		void AddNewLabel(Label newLabel);
		void UpdateLabel(Label updatedLabel);
		Label GetLabel(Guid id);
			
		IEnumerable<TherapyPlaceType> GetAllTherapyPlaceTypes();
		IEnumerable<TherapyPlaceType> GetAllTherapyPlaceTypesPlusDummy(); 
		void AddNewTherapyPlaceType(TherapyPlaceType newTherapyPlaceType);
		void UpdateTherapyPlaceType(TherapyPlaceType updatedTherapyPlaceType);
		TherapyPlaceType GetTherapyPlaceType(Guid id);

		IEnumerable<MedicalPractice> GetAllMedicalPractices();
		void AddNewMedicalPractice(MedicalPractice newMedicalPractice);
		void UpdateMedicalPractice(MedicalPractice updatedMedicalPractice);
		void RemoveMedicalPractice(MedicalPractice medicalPracticeToRemove);
		MedicalPractice GetMedicalPractice(Guid id);
		MedicalPractice GetMedicalPractice(Guid id, uint version);
		uint GetMedicalPracticeVersion(Guid id, Date date);
		
		bool AddEvents (IEnumerable<DomainEvent> newEvents);
		EventStream<Guid> GetEventStreamForAPatient (Guid patientId);
		EventStream<AggregateIdentifier> GetEventStreamForADay (AggregateIdentifier id);
	}
}