using System.IO;
using ClosedXML.Excel;
using NtTracker.Extensions;
using NtTracker.Models;
using NtTracker.Resources.Patient;
using NtTracker.Resources.Shared;
using PagedList;

namespace NtTracker.Documents
{
    public static class SpreadsheetManager
    {
        /// <summary>
        /// Create an xlsx format spreadsheet with the data of the given patients
        /// including all their related entities.
        /// </summary>
        /// <param name="patients">Patients to export.</param>
        /// <returns>MemoryStream of the resulting document.</returns>
        public static MemoryStream ExportPatientData(IPagedList<Patient> patients)
        {
            //Create file and sheets
            var workBook = new XLWorkbook(XLEventTracking.Disabled);
            var patientSheet = workBook.Worksheets.Add(SharedStrings.PatientData);
            var nbrSurveillanceSheet = workBook.Worksheets.Add(SharedStrings.NBRSurveillance);
            var cnsExplorationSheet = workBook.Worksheets.Add(Resources.NbrSurveillance.Strings.CnsExplorationsShort);
            var hypothermiaSheet = workBook.Worksheets.Add(SharedStrings.Hypothermia);
            var monitoringSheet = workBook.Worksheets.Add(SharedStrings.Monitoring);
            var analysisSheet = workBook.Worksheets.Add(SharedStrings.Analysis);

            //Format sheets
            var headerRow = patientSheet.Row(1);
            headerRow.Cell(1).Value = SharedStrings.Patient;
            headerRow.Cell(2).Value = Strings.Nhc;
            headerRow.Cell(3).Value = Strings.Nuhsa;
            headerRow.Cell(4).Value = Strings.RegistrantName;
            headerRow.Cell(5).Value = Strings.Name;
            headerRow.Cell(6).Value = Strings.Surnames;
            headerRow.Cell(7).Value = Strings.PatientSex;
            headerRow.Cell(8).Value = Strings.BirthDate;
            headerRow.Cell(9).Value = Strings.BirthType;
            headerRow.Cell(10).Value = Strings.Ph;
            headerRow.Cell(11).Value = Strings.Apgar;
            headerRow.Cell(12).Value = Strings.Weight;
            headerRow.Cell(13).Value = Strings.CprType;
            headerRow.Cell(14).Value = Strings.Lethargy;
            headerRow.Cell(15).Value = Strings.Stupor;
            headerRow.Cell(16).Value = Strings.Coma;
            headerRow.Cell(17).Value = Strings.AlteredTone;
            headerRow.Cell(18).Value = Strings.AlteredReflexes;
            headerRow.Cell(19).Value = Strings.AlteredSuction;
            headerRow.Cell(20).Value = SharedStrings.Convulsion;
            headerRow.Cell(21).Value = Strings.PatientStatus;
            FormatSheet(patientSheet);
            var patientIndex = 2;

            headerRow = nbrSurveillanceSheet.Row(1);
            headerRow.Cell(1).Value = SharedStrings.Patient;
            headerRow.Cell(2).Value = SharedStrings.TimeSlot;
            headerRow.Cell(3).Value = SharedStrings.EEG;
            headerRow.Cell(4).Value = SharedStrings.AEEG;
            headerRow.Cell(5).Value = Resources.NbrSurveillance.Strings.TfUltrasound;
            FormatSheet(nbrSurveillanceSheet);
            var nbrSurveillanceIndex = 2;

            headerRow = cnsExplorationSheet.Row(1);
            headerRow.Cell(1).Value = SharedStrings.Patient;
            headerRow.Cell(2).Value = SharedStrings.TimeSlot;
            headerRow.Cell(3).Value = SharedStrings.Behavior;
            headerRow.Cell(4).Value = SharedStrings.CranialNerves;
            headerRow.Cell(5).Value = SharedStrings.Tone;
            headerRow.Cell(6).Value = SharedStrings.Position;
            headerRow.Cell(7).Value = SharedStrings.Reflexes;
            FormatSheet(cnsExplorationSheet);
            var cnsExplorationIndex = 2;

            headerRow = hypothermiaSheet.Row(1);
            headerRow.Cell(1).Value = SharedStrings.Patient;
            headerRow.Cell(2).Value = SharedStrings.TimeSlot;
            headerRow.Cell(3).Value = Resources.Hypothermia.Strings.CnsUltrasound;
            headerRow.Cell(4).Value = SharedStrings.AEEG;
            headerRow.Cell(5).Value = SharedStrings.EEG;
            headerRow.Cell(6).Value = SharedStrings.Convulsion;
            headerRow.Cell(7).Value = Resources.Hypothermia.Strings.CerebralResonance;
            FormatSheet(hypothermiaSheet);
            var hypothermiaIndex = 2;

            headerRow = monitoringSheet.Row(1);
            headerRow.Cell(1).Value = SharedStrings.Patient;
            headerRow.Cell(2).Value = SharedStrings.TimeSlot;
            headerRow.Cell(3).Value = Resources.Monitoring.Strings.Description;
            headerRow.Cell(4).Value = Resources.Monitoring.Strings.Comments;
            headerRow.Cell(5).Value = Resources.Monitoring.Strings.MuscularTone;
            headerRow.Cell(6).Value = Resources.Monitoring.Strings.Spasticity;
            headerRow.Cell(7).Value = Resources.Monitoring.Strings.Dystonia;
            headerRow.Cell(8).Value = Resources.Monitoring.Strings.Dyskinesia;
            headerRow.Cell(9).Value = Resources.Monitoring.Strings.Ataxia;
            headerRow.Cell(10).Value = Resources.Monitoring.Strings.Hyperreflexia;
            headerRow.Cell(11).Value = Resources.Monitoring.Strings.DevelopmentReflexes;
            headerRow.Cell(12).Value = Resources.Monitoring.Strings.CognitiveDeficit;
            headerRow.Cell(13).Value = Resources.Monitoring.Strings.AuditoryDeficit;
            headerRow.Cell(14).Value = Resources.Monitoring.Strings.VisualDeficit;
            headerRow.Cell(15).Value = Resources.Monitoring.Strings.VocalDeficit;
            headerRow.Cell(16).Value = Resources.Monitoring.Strings.TactileResponse;
            headerRow.Cell(17).Value = Resources.Monitoring.Strings.SoundResponse;
            headerRow.Cell(18).Value = Resources.Monitoring.Strings.CaretakerResponse;
            headerRow.Cell(19).Value = Resources.Monitoring.Strings.StrangersResponse;
            headerRow.Cell(20).Value = Resources.Monitoring.Strings.Smiles;
            FormatSheet(monitoringSheet);
            var monitoringIndex = 2;

            headerRow = analysisSheet.Row(1);
            headerRow.Cell(1).Value = SharedStrings.Patient;
            headerRow.Cell(2).Value = SharedStrings.TimeSlot;
            headerRow.Cell(3).Value = Resources.Analysis.Strings.Hemoglobin;
            headerRow.Cell(4).Value = Resources.Analysis.Strings.Hematocrit;
            headerRow.Cell(5).Value = Resources.Analysis.Strings.PlateletCount;
            headerRow.Cell(6).Value = Resources.Analysis.Strings.ALT;
            headerRow.Cell(7).Value = Resources.Analysis.Strings.AST;
            headerRow.Cell(8).Value = Resources.Analysis.Strings.CPK;
            headerRow.Cell(9).Value = Resources.Analysis.Strings.Proteins;
            headerRow.Cell(10).Value = Resources.Analysis.Strings.Sodium;
            headerRow.Cell(11).Value = Resources.Analysis.Strings.Potassium;
            headerRow.Cell(12).Value = Resources.Analysis.Strings.Chloride;
            FormatSheet(analysisSheet);
            var analysisIndex = 2;

            //Basic patient data
            foreach (var patient in patients)
            {
                var row = patientSheet.Row(patientIndex);

                row.Cell(1).Value = patient.Id;
                row.Cell(2).Value = patient.Nhc;
                row.Cell(3).Value = patient.Nuhsa;
                row.Cell(4).Value = patient.RegistrantName;
                row.Cell(5).Value = patient.Name;
                row.Cell(6).Value = patient.Surnames;
                row.Cell(7).Value = patient.Sex.ToLocalizedString();
                row.Cell(8).Value = patient.BirthDate;
                row.Cell(9).Value = patient.BirthType.ToLocalizedString();
                row.Cell(10).Value = patient.Ph;
                row.Cell(11).Value = patient.Apgar;
                row.Cell(12).Value = patient.Weight + "g";
                row.Cell(13).Value = patient.CprType.ToLocalizedString();
                row.Cell(14).Value = patient.Lethargy.ToLocalizedString();
                row.Cell(15).Value = patient.Stupor.ToLocalizedString();
                row.Cell(16).Value = patient.Coma.ToLocalizedString();
                row.Cell(17).Value = patient.AlteredTone.ToLocalizedString();
                row.Cell(18).Value = patient.AlteredReflexes.ToLocalizedString();
                row.Cell(19).Value = patient.AlteredSuction.ToLocalizedString();
                row.Cell(20).Value = patient.Convulsion.ToLocalizedString();
                row.Cell(21).Value = patient.PatientStatus.ToLocalizedString();

                patientIndex++;
            }

            //NBR Surveillance data
            foreach (var patient in patients)
            {
                foreach (var surveillance in patient.NbrSurveillances)
                {
                    var row = nbrSurveillanceSheet.Row(nbrSurveillanceIndex);

                    row.Cell(1).Value = surveillance.PatientId;
                    row.Cell(2).Value = surveillance.TimeSlot.ToLocalizedString();
                    row.Cell(3).Value = surveillance.Eeg.ToLocalizedString();
                    row.Cell(4).Value = surveillance.AEeg.ToLocalizedString();
                    row.Cell(5).Value = surveillance.TfUltrasound.ToLocalizedString();
                    nbrSurveillanceIndex++;

                    //Related CnsExploration
                    if (surveillance.CnsExploration != null)
                    {
                        var explorationRow = cnsExplorationSheet.Row(cnsExplorationIndex);
                        InsertCnsExploration(surveillance.CnsExploration, surveillance.TimeSlot, patient.Id, explorationRow);
                        cnsExplorationIndex++;
                    }

                    //Related Analysis
                    if (surveillance.Analysis != null)
                    {
                        var analysisRow = analysisSheet.Row(analysisIndex);
                        InsertAnalysis(surveillance.Analysis, surveillance.TimeSlot, patient.Id, analysisRow);
                        analysisIndex++;
                    }
                }
            }

            //Hypothermia data
            foreach (var patient in patients)
            {
                foreach (var hypothermia in patient.Hypothermias)
                {
                    var row = hypothermiaSheet.Row(hypothermiaIndex);

                    row.Cell(1).Value = hypothermia.PatientId;
                    row.Cell(2).Value = hypothermia.TimeSlot.ToLocalizedString();
                    row.Cell(3).Value = hypothermia.CnsUs.ToLocalizedString();
                    row.Cell(4).Value = hypothermia.AEeg.ToLocalizedString();
                    row.Cell(5).Value = hypothermia.Eeg.ToLocalizedString();
                    row.Cell(6).Value = hypothermia.Convulsion.ToLocalizedString();
                    row.Cell(7).Value = hypothermia.Cr.ToLocalizedString();

                    hypothermiaIndex++;

                    //Related CnsExploration
                    if (hypothermia.CnsExploration != null)
                    {
                        var explorationRow = cnsExplorationSheet.Row(cnsExplorationIndex);
                        InsertCnsExploration(hypothermia.CnsExploration, hypothermia.TimeSlot, patient.Id, explorationRow);
                        cnsExplorationIndex++;
                    }

                    //Related Analysis
                    if (hypothermia.Analysis != null)
                    {
                        var analysisRow = analysisSheet.Row(analysisIndex);
                        InsertAnalysis(hypothermia.Analysis, hypothermia.TimeSlot ,patient.Id, analysisRow);
                        analysisIndex++;
                    }
                }
            }

            //Monitoring data
            foreach (var patient in patients)
            {
                foreach (var monitoring in patient.Monitorings)
                {
                    var row = monitoringSheet.Row(monitoringIndex);

                    row.Cell(1).Value = monitoring.PatientId;
                    row.Cell(2).Value = monitoring.DateTime;
                    row.Cell(3).Value = monitoring.Description;
                    row.Cell(4).Value = monitoring.Comments;
                    row.Cell(5).Value = monitoring.MuscularTone.ToLocalizedString();
                    row.Cell(6).Value = monitoring.Spasticity.ToLocalizedString();
                    row.Cell(7).Value = monitoring.Dystonia.ToLocalizedString();
                    row.Cell(8).Value = monitoring.Dyskinesia.ToLocalizedString();
                    row.Cell(9).Value = monitoring.Ataxia.ToLocalizedString();
                    row.Cell(10).Value = monitoring.Hyperreflexia.ToLocalizedString();
                    row.Cell(11).Value = monitoring.DevelopmentReflexes.ToLocalizedString();
                    row.Cell(12).Value = monitoring.CognitiveDeficit.ToLocalizedString();
                    row.Cell(13).Value = monitoring.AuditoryDeficit.ToLocalizedString();
                    row.Cell(14).Value = monitoring.VisualDeficit.ToLocalizedString();
                    row.Cell(15).Value = monitoring.VocalDeficit.ToLocalizedString();
                    row.Cell(16).Value = monitoring.TactileResponse.ToLocalizedString();
                    row.Cell(17).Value = monitoring.SoundResponse.ToLocalizedString();
                    row.Cell(18).Value = monitoring.CaretakerResponse.ToLocalizedString();
                    row.Cell(19).Value = monitoring.StrangersResponse.ToLocalizedString();
                    row.Cell(20).Value = monitoring.Smiles.ToLocalizedString();

                    monitoringIndex++;
                }
            }

            //Adjust sheets contents
            patientSheet.Columns(1, 21).AdjustToContents();
            nbrSurveillanceSheet.Columns(1, 5).AdjustToContents();
            cnsExplorationSheet.Columns(1, 7).AdjustToContents();
            analysisSheet.Columns(1, 12).AdjustToContents();
            hypothermiaSheet.Columns(1, 7).AdjustToContents();
            monitoringSheet.Columns(1, 20).AdjustToContents();

            //Save document to stream
            var memoryStream = new MemoryStream();
            workBook.SaveAs(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        /// <summary>
        /// Insert the CnsExploration with the given data to the given row.
        /// </summary>
        /// <param name="cnsExploration">CnsExploration to insert.</param>
        /// <param name="time">Time of the exploration.</param>
        /// <param name="patient">Id of the patient.</param>
        /// <param name="row">Row to add the exploration to.</param>
        private static void InsertCnsExploration(CnsExploration cnsExploration, TimeSlot time, int patient, IXLRow row)
        {
            row.Cell(1).Value = patient;
            row.Cell(2).Value = time.ToLocalizedString();
            row.Cell(3).Value = cnsExploration.Behavior.ToLocalizedString();
            row.Cell(4).Value = cnsExploration.CranialNerves.ToLocalizedString();
            row.Cell(5).Value = cnsExploration.Tone.ToLocalizedString();
            row.Cell(6).Value = cnsExploration.Position.ToLocalizedString();
            row.Cell(7).Value = cnsExploration.Reflexes.ToLocalizedString();
        }

        /// <summary>
        /// Insert the Analysis with the given data to the given row.
        /// </summary>
        /// <param name="analysis">Analysis to insert.</param>
        /// /// <param name="time">Time of the analysis.</param>
        /// <param name="patient">Id of the patient.</param>
        /// <param name="row">Row to add the analysis to.</param>
        private static void InsertAnalysis(Analysis analysis, TimeSlot time, int patient, IXLRow row)
        {
            row.Cell(1).Value = patient;
            row.Cell(2).Value = time.ToLocalizedString();
            row.Cell(3).Value = analysis.Hemoglobin;
            row.Cell(4).Value = analysis.Hematocrit;
            row.Cell(5).Value = analysis.PlateletCount;
            row.Cell(6).Value = analysis.Alt;
            row.Cell(7).Value = analysis.Ast;
            row.Cell(8).Value = analysis.Cpk;
            row.Cell(9).Value = analysis.Proteins;
            row.Cell(10).Value = analysis.Sodium;
            row.Cell(11).Value = analysis.Potassium;
            row.Cell(12).Value = analysis.Chloride;
        }

        /// <summary>
        /// Format the heading column and row for the given worksheet.
        /// </summary>
        /// <param name="workSheet">Sheet to format.</param>
        private static void FormatSheet(IXLWorksheet workSheet)
        {
            var headerRow = workSheet.Row(1);

            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromArgb(235, 241, 222);
            workSheet.Column(1).Style.Fill.BackgroundColor = XLColor.FromArgb(235, 241, 222);
            workSheet.SheetView.FreezeRows(1);
            workSheet.SheetView.FreezeColumns(1);
        }
    }
}