using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NtTracker.Data;
using NtTracker.Models;
using NtTracker.ViewModels;
using NtTracker.Resources.NbrSurveillance;

namespace NtTracker.Services
{
    public class NbrSurveillanceService : Service, INbrSurveillanceService
    {
        private readonly ICnsExplorationService _cnsExplorationService;
        private readonly IAnalysisService _analysisService;

        public NbrSurveillanceService(DataContext dataContext, ICnsExplorationService cnsExplorationService, 
            IAnalysisService analysisService) : base(dataContext)
        {
            _cnsExplorationService = cnsExplorationService;
            _analysisService = analysisService;
        }

        /// <summary>
        /// Find the NbrSurveillance with the given id. Include also
        /// the related analysis and exploration.
        /// </summary>
        /// <param name="id">NbrSurveillance id.</param>
        /// <returns>The NbrSurveillance with the given id 
        /// or null if not found.</returns>
        public NbrSurveillance FindById(int id)
        {
            var surveillance = Repository.NbrSurveillances
                .Include(s => s.CnsExploration)
                .Include(s => s.Analysis)
                .SingleOrDefault(s => s.Id == id);

            return surveillance;
        }

        /// <summary>
        /// Find the NbrSurveillances of the patient with the given id.
        /// </summary>
        /// <param name="patientId">Patient id.</param>
        /// <returns>NbrSurveillances for the patient with the given id.</returns>
        public List<NbrSurveillance> FindByPatientId(int patientId)
        {
            var surveillances = Repository.NbrSurveillances
                .Where(s => s.PatientId == patientId)
                .Include(s => s.CnsExploration)
                .Include(s => s.Analysis)
                .OrderBy(s => s.TimeSlot);

            return surveillances.ToList();
        }

        /// <summary>
        /// Checks with the data of the surveillance if there is risk
        /// of encephalopathy.
        /// </summary>
        /// <param name="nbrSurveillance">NbrSurveillance to check.</param>
        /// <returns>bool indicating if there is risk of encephalopathy.</returns>
        public bool AtRisk(NbrSurveillance nbrSurveillance)
        {
            var result = false;

            var analysisScore = nbrSurveillance.Analysis.ComputeScore(); //Analysis score out of 10
            var explorationScore = nbrSurveillance.CnsExploration.Score; //Exploration score out of 10
            var totalScore = analysisScore + explorationScore;

            //If tests are not normal or there is a low score in the analysis and the exploration 
            //the patient is at risk
            if (nbrSurveillance.Eeg != null && nbrSurveillance.Eeg != 0)
                result = true;
            if (nbrSurveillance.AEeg != null && nbrSurveillance.AEeg != 0)
                result = true;
            if (nbrSurveillance.TfUltrasound != null && nbrSurveillance.TfUltrasound != 0)
                result = true;

            if (totalScore <= 10)
                result = true;

            return result;
        }

        /// <summary>
        /// Save a new NbrSurveillance and it's related CnsExploration to the database with the data from 
        /// a viewmodel. If the TimeSlot is repeated, an error will be added to the modelstate of the 
        /// viewmodel and the operation will not be completed.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new surveillance data.</param>
        /// <param name="patientId">Id of the patient for which the surveillance is being created.</param>
        public void Create(EditNbrSurveillanceViewModel viewModel, int patientId)
        {
            //Check that the time slot for the surveillance is not repeated
            var repeated = Repository.NbrSurveillances
                .Where(s => s.PatientId == patientId)
                .Any(s => s.TimeSlot == viewModel.TimeSlot);

            //If repeated show error to the user
            if (repeated)
            {
                viewModel.ModelState.AddModelError("TimeSlot", Strings.ErrorRepeated);
            }
            else
            {
                //Create a new surveillance from the viewmodel
                var surveillance = viewModel.ToNewModel();
                surveillance.PatientId = patientId;

                //Create the associated CnsExploration
                var explorationId = _cnsExplorationService.Create(viewModel.CnsExploration);
                surveillance.CnsExplorationId = explorationId;

                //Create the associated Analysis
                var analysisId = _analysisService.Create(viewModel.Analysis);
                surveillance.AnalysisId = analysisId;

                Repository.NbrSurveillances.Add(surveillance);
                Save();
            }
        }

        /// <summary>
        /// Update the NbrSurveillance with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        /// <returns>bool indicating if the update was successful.</returns>
        public bool Update(EditNbrSurveillanceViewModel viewModel)
        {
            var nbrSurveillance = Repository.NbrSurveillances
                .SingleOrDefault(n => n.Id == viewModel.Id);
            if (nbrSurveillance == null) return false;

            //Update the surveillance with the viewmodel data
            viewModel.ToModel(nbrSurveillance);
            Repository.Entry(nbrSurveillance).State = EntityState.Modified;

            //Update associated CnsExploration
            _cnsExplorationService.Update(viewModel.CnsExploration);

            //Update associated Analysis
            _analysisService.Update(viewModel.Analysis);

            Save();
            return true;
        }

        /// <summary>
        /// Delete the NbrSurveillance with the given id.
        /// </summary>
        /// <param name="id">Id of the NbrSurveillance to delete.</param>
        /// <returns>bool indicating if the deletion was successful.</returns>
        public bool Delete(int id)
        {
            var surveillance = Repository.NbrSurveillances.SingleOrDefault(e => e.Id == id);
            if (surveillance == null) return false;

            //Delete the surveillance
            var explorationId = surveillance.CnsExplorationId;
            var analysisId = surveillance.AnalysisId;
            Repository.NbrSurveillances.Remove(surveillance);
            Save();

            //Delete related cns exploration
            _cnsExplorationService.Delete(explorationId);

            //Delete related analysis
            _analysisService.Delete(analysisId);

            return true;
        }

        public override void Dispose()
        {
            _cnsExplorationService.Dispose();
            _analysisService.Dispose();
            base.Dispose();
        }
    }
}