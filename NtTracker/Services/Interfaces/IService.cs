using System.Threading.Tasks;
using NtTracker.Models;

namespace NtTracker.Services
{
    public interface IService
    {
        /// <summary>
        /// Initialize the database for this service. 
        /// </summary>
        void Initialize();

        /// <summary>
        /// Log an operation to the database and save it asynchronously.
        /// </summary>
        /// <param name="operationType">Type of operation.</param>
        /// <param name="userId">Id of the user that performed the operation.</param>
        /// <param name="patientId">Id of the patient affected by the operation (if any).</param>
        /// <param name="data">Additional data of the operation to store (optional).</param>
        /// <returns>Task for the asynchronous log save.</returns>
        Task Log(OperationType operationType, int userId, int? patientId = null, string data = null);

        /// <summary>
        /// Release all the resources used by the instance of the service.
        /// </summary>
        void Dispose();
    }
}
