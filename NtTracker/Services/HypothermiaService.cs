using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NtTracker.Data;
using NtTracker.Models;
using NtTracker.Resources.Hypothermia;
using NtTracker.ViewModels;

namespace NtTracker.Services
{
    public class HypothermiaService : Service, IHypothermiaService
    {
        private readonly ICnsExplorationService _cnsExplorationService;
        private readonly IAnalysisService _analysisService;

        public HypothermiaService(DataContext dataContext, ICnsExplorationService cnsExplorationService, 
            IAnalysisService analysisService) : base(dataContext)
        {
            _cnsExplorationService = cnsExplorationService;
            _analysisService = analysisService;
        }

        /// <summary>
        /// Find the Hypothermia with the given id. Include also
        /// the related analysis and exploration.
        /// </summary>
        /// <param name="id">Hypothermia id.</param>
        /// <returns>The Hypothermia with the given id 
        /// or null if not found.</returns>
        public Hypothermia FindById(int id)
        {
            var hypothermia = Repository.Hypothermias
                .Include(h => h.CnsExploration)
                .Include(h => h.Analysis)
                .SingleOrDefault(h => h.Id == id);

            return hypothermia;
        }

        /// <summary>
        /// Find the hypothermias of the patient with the given id.
        /// </summary>
        /// <param name="patientId">Id of the patient.</param>
        /// <returns>List of hypothermias for the patient with the given id.</returns>
        public List<Hypothermia> FindByPatientId(int patientId)
        {
            var hypothermias = Repository.Hypothermias
                .Where(h => h.PatientId == patientId)
                .Include(h => h.CnsExploration)
                .Include(h => h.Analysis)
                .OrderBy(h=> h.TimeSlot);

            return hypothermias.ToList();
        }

        /// <summary>
        /// Save a new Hypothermia and it's related CnsExploration to the database with the data from 
        /// a viewmodel. If the TimeSlot is repeated, an error will be added to the modelstate of the 
        /// viewmodel and the operation will not be completed.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the new hypothermia data.</param>
        /// <param name="patientId">Id of the patient for which the hypothermia is being created.</param>
        public void Create(EditHypothermiaViewModel viewModel, int patientId)
        {
            //Check that the time slot for the hypothermia is not repeated
            var repeated = Repository.Hypothermias
                .Where(e => e.PatientId == patientId)
                .Any(e => e.TimeSlot == viewModel.TimeSlot);

            //If repeated show error to the user
            if (repeated)
            {
                viewModel.ModelState.AddModelError("TimeSlot", Strings.ErrorRepeated);
            }
            else
            {
                //Create a new Hypothermia from the viewmodel
                var hypothermia = viewModel.ToNewModel();
                hypothermia.PatientId = patientId;

                //Create the associated CnsExploration
                var explorationId = _cnsExplorationService.Create(viewModel.CnsExploration);
                hypothermia.CnsExplorationId = explorationId;

                //Create the associated Analysis
                var analysisId = _analysisService.Create(viewModel.Analysis);
                hypothermia.AnalysisId = analysisId;

                Repository.Hypothermias.Add(hypothermia);
                Save();
            }
        }

        /// <summary>
        /// Update the Hypothermia with the data from the given viewmodel.
        /// </summary>
        /// <param name="viewModel">Viewmodel with the modified data.</param>
        /// <returns>bool indicating if the update was successful.</returns>
        public bool Update(EditHypothermiaViewModel viewModel)
        {
            var hypothermia = Repository.Hypothermias.SingleOrDefault(e => e.Id == viewModel.Id);
            if (hypothermia == null) return false;

            //Update the hypothermia with the viewmodel data
            viewModel.ToModel(hypothermia);
            Repository.Entry(hypothermia).State = EntityState.Modified;

            //Update associated CnsExploration
            _cnsExplorationService.Update(viewModel.CnsExploration);

            //Update associated Analysis
            _analysisService.Update(viewModel.Analysis);

            Save();
            return true;
        }

        /// <summary>
        /// Delete the Hypothermia with the given id.
        /// </summary>
        /// <param name="id">Id of the Hypothermia to delete.</param>
        /// <returns>bool indicating if the deletion was successful.</returns>
        public bool Delete(int id)
        {
            var hypothermia = Repository.Hypothermias.SingleOrDefault(e => e.Id == id);
            if (hypothermia == null) return false;

            //Delete the hypothermia
            var explorationId = hypothermia.CnsExplorationId;
            var analysisId = hypothermia.AnalysisId;
            Repository.Hypothermias.Remove(hypothermia);
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