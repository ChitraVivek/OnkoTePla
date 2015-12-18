﻿using bytePassion.Lib.TimeLib;
using bytePassion.OnkoTePla.Contracts.Config;
using bytePassion.OnkoTePla.Contracts.Infrastructure;
using bytePassion.OnkoTePla.Contracts.Patients;
using bytePassion.OnkoTePla.Core.Domain;
using bytePassion.OnkoTePla.Core.Readmodels;
using System;
using System.Collections.Generic;


namespace bytePassion.OnkoTePla.Client.WpfUi.Model
{

    internal interface IDataCenter
    {
        User LoggedInUser { get; }

        AppointmentsOfADayReadModel     GetAppointmentsOfADayReadModel    (AggregateIdentifier identifier);
        AppointmentsOfAPatientReadModel GetAppointmentsOfAPatientReadModel(Guid                patientId);

        MedicalPractice GetMedicalPracticeByDateAndId(Date date, Guid medicalPracticeId);
        MedicalPractice GetMedicalPracticeByIdAndVersion(Guid medicalPracticeId, uint version=0);

        IEnumerable<MedicalPractice> GetAllMedicalPractices();
        IEnumerable<Patient>         GetAllPatients();
    }

}