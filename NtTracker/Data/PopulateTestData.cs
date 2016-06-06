using System;
using System.Data.Entity;
using NtTracker.Models;

namespace NtTracker.Data
{
    public class PopulateTestData : DropCreateDatabaseAlways<DataContext>
    {
        //NOTE: Any changes made to this test data set may break the 
        //integration tests in the associated Tests assembly.
        protected override void Seed(DataContext context)
        {
            // Admin
            var admin = new UserAccount()
            {
                IsAdmin = true,
                UserName = "admin",
                RegistrationDate = DateTime.Now.AddMinutes(-1),
                FailedLoginAttempts = 0
            };

            context.UserAccounts.Add(admin);
            context.SaveChanges();

            // Dummy patients

            for (var i = 0; i < 50; i++)
            {
                var p = new Patient
                {
                    Nhc = "123",
                    Nuhsa = "AN0123456789",
                    Name = "John",
                    Surnames = "Doe " + i,
                    Sex = PatientSex.Male,
                    BirthDate = DateTime.Now,
                    BirthType = BirthType.Cesarean,
                    Ph = 10,
                    Apgar = 7,
                    Weight = 500,
                    CprType = null,
                    Lethargy = false,
                    Stupor = false,
                    Coma = false,
                    AlteredTone = false,
                    AlteredReflexes = false,
                    AlteredSuction = false,
                    Convulsion = false,
                    PatientStatus = PatientStatus.Normal,
                    Registrant = admin,
                    RegistrantName = "admin"
                };
                context.Patients.Add(p);
            }

            context.SaveChanges();

            // Patient 1

            var patient1 = new Patient
            {
                Nhc = "1",
                Nuhsa = "AN0123456789",
                Name = "John",
                Surnames = "Smith",
                Sex = PatientSex.Male,
                BirthDate = DateTime.Parse("2015-01-25 8:25"),
                BirthType = BirthType.Cesarean,
                Ph = 10,
                Apgar = 7,
                Weight = 500,
                CprType = CprType.Type3,
                Lethargy = false,
                Stupor = false,
                Coma = false,
                AlteredTone = false,
                AlteredReflexes = false,
                AlteredSuction = false,
                Convulsion = false,
                PatientStatus = PatientStatus.Normal,
                Registrant = admin,
                RegistrantName = "admin"
            };

            context.Patients.Add(patient1);
            context.SaveChanges();

            // Patient 2

            var patient2 = new Patient
            {
                Nhc = "2",
                Nuhsa = "AN0123456789",
                Name = "Jake",
                Surnames = "Smith",
                Sex = PatientSex.Male,
                BirthDate = DateTime.Parse("2015-01-25 8:25"),
                BirthType = BirthType.Cesarean,
                Ph = 3.2,
                Apgar = 4,
                Weight = 500,
                CprType = CprType.Type3,
                Lethargy = false,
                Stupor = false,
                Coma = false,
                AlteredTone = false,
                AlteredReflexes = false,
                AlteredSuction = false,
                Convulsion = false,
                PatientStatus = PatientStatus.NbrSurveillance,
                Registrant = admin,
                RegistrantName = "admin"
            };

            context.Patients.Add(patient2);
            context.SaveChanges();

            var analysis1 = new Analysis
            {
                Hemoglobin = 150,
                PlateletCount = 85000,
                Alt = 6.5,
                Ast = 2.5,
                Cpk = 10
            };

            context.Analyses.Add(analysis1);
            context.SaveChanges();

            var cnsExploration1 = new CnsExploration
            {
                Behavior = Behavior.Normal,
            };

            context.CnsExplorations.Add(cnsExploration1);
            context.SaveChanges();

            var nbrSurveillance1 = new NbrSurveillance
            {
                Eeg = Electroencephalogram.Normal,
                AEeg = AElectroencephalogram.BurstSupression,
                TfUltrasound = TransfontanellarUltrasound.Normal,
                Patient = patient2,
                PatientId = patient2.Id,
                CnsExploration = cnsExploration1,
                CnsExplorationId = cnsExploration1.Id,
                Analysis = analysis1,
                AnalysisId = analysis1.Id
            };

            context.NbrSurveillances.Add(nbrSurveillance1);
            context.SaveChanges();

            // Patient 3

            var patient3 = new Patient
            {
                Nhc = "3",
                Nuhsa = "AN0123456789",
                Name = "Jane",
                Surnames = "Smith",
                Sex = PatientSex.Female,
                BirthDate = DateTime.Parse("2015-01-25 8:25"),
                BirthType = BirthType.Cesarean,
                Ph = 3.2,
                Apgar = 4,
                Weight = 500,
                CprType = CprType.Type2,
                Lethargy = false,
                Stupor = false,
                Coma = false,
                AlteredTone = false,
                AlteredReflexes = false,
                AlteredSuction = false,
                Convulsion = false,
                PatientStatus = PatientStatus.Monitoring,
                Registrant = admin,
                RegistrantName = "admin"
            };

            context.Patients.Add(patient3);
            context.SaveChanges();

            var analysis2 = new Analysis
            {
                Hemoglobin = 150,
                PlateletCount = 100000,
                Alt = 10.5,
                Ast = 7.86,
                Cpk = 10
            };

            context.Analyses.Add(analysis2);
            context.SaveChanges();

            var cnsExploration2 = new CnsExploration
            {
                Behavior = Behavior.Normal,
            };

            context.CnsExplorations.Add(cnsExploration2);
            context.SaveChanges();

            var nbrSurveillance2 = new NbrSurveillance
            {
                Eeg = Electroencephalogram.Normal,
                AEeg = AElectroencephalogram.BurstSupression,
                TfUltrasound = TransfontanellarUltrasound.Normal,
                Patient = patient3,
                PatientId = patient3.Id,
                CnsExploration = cnsExploration2,
                CnsExplorationId = cnsExploration2.Id,
                Analysis = analysis2,
                AnalysisId = analysis2.Id
            };

            context.NbrSurveillances.Add(nbrSurveillance2);
            context.SaveChanges();

            var monitoring1 = new Monitoring
            {
                DateTime = DateTime.Now,
                Description = "Monitoring1",
                PatientId = nbrSurveillance2.PatientId
            };

            context.Monitorings.Add(monitoring1);
            context.SaveChanges();

            // Patient 4

            var patient4 = new Patient
            {
                Nhc = "4",
                Nuhsa = "AN0123456788",
                Name = "Jean",
                Surnames = "Smith",
                Sex = PatientSex.Male,
                BirthDate = DateTime.Parse("2015-01-25 8:25"),
                BirthType = BirthType.Cesarean,
                Ph = 3.2,
                Apgar = 4,
                Weight = 500,
                CprType = CprType.Type3,
                Lethargy = false,
                Stupor = false,
                Coma = true,
                AlteredTone = true,
                AlteredReflexes = false,
                AlteredSuction = false,
                Convulsion = false,
                PatientStatus = PatientStatus.Hypothermia,
                Registrant = admin,
                RegistrantName = "admin"
            };

            context.Patients.Add(patient4);
            context.SaveChanges();

            var cnsExploration3 = new CnsExploration
            {
                Id = 3,
                Behavior = Behavior.Normal,
            };

            context.CnsExplorations.Add(cnsExploration3);
            context.SaveChanges();

            var cnsExploration4 = new CnsExploration
            {
                Id = 4,
                Behavior = Behavior.Normal,
            };

            context.CnsExplorations.Add(cnsExploration4);
            context.SaveChanges();

            var analysis3 = new Analysis
            {
                Hemoglobin = 150,
                PlateletCount = 100000,
                Alt = 10.5,
                Ast = 7.86,
                Cpk = 10
            };

            context.Analyses.Add(analysis3);
            context.SaveChanges();

            var hypothermia1 = new Hypothermia
            {
                TimeSlot = TimeSlot.H24,
                CnsUs = CnsUltrasound.ThalamusInjury,
                Eeg = Electroencephalogram.Normal,
                AEeg = AElectroencephalogram.Convulsion,
                Cr = CerebralResonance.CorpusCallosumInjury,
                PatientId = patient4.Id,
                CnsExploration = cnsExploration3,
                CnsExplorationId = cnsExploration3.Id,
                Analysis = analysis3,
                AnalysisId = analysis3.Id
            };

            context.Hypothermias.Add(hypothermia1);
            context.SaveChanges();

            var analysis4 = new Analysis
            {
                Hemoglobin = 150,
                PlateletCount = 100000,
                Alt = 10.5,
                Ast = 7.86,
                Cpk = 10
            };

            context.Analyses.Add(analysis4);
            context.SaveChanges();

            var hypothermia2 = new Hypothermia
            {
                TimeSlot = TimeSlot.H48,
                PatientId = patient4.Id,
                CnsExploration = cnsExploration4,
                CnsExplorationId = cnsExploration4.Id,
                Analysis = analysis4,
                AnalysisId = analysis4.Id
            };

            context.Hypothermias.Add(hypothermia2);
            context.SaveChanges();
        }
    }
}