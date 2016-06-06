using System;
using System.Threading.Tasks;
using NtTracker.Data;
using NtTracker.Models;

namespace NtTracker.Services
{
    public class Service : IService
    {
        protected readonly DataContext Repository;

        /// <summary>
        /// Generic service constructor to inject the data context.
        /// </summary>
        /// <param name="dataContext">Data context to inject in the service.</param>
        public Service(DataContext dataContext)
        {
            Repository = dataContext;
        }

        /// <summary>
        /// Initialize the database for this service. 
        /// </summary>
        public void Initialize()
        {
            Repository.Database.Initialize(false);
        }

        /// <summary>
        /// Log an operation to the database.
        /// </summary>
        /// <param name="operationType">Type of operation.</param>
        /// <param name="userId">Id of the user that performed the operation.</param>
        /// <param name="patientId">Id of the patient affected by the operation (if any).</param>
        /// <param name="data">Additional data of the operation to store (optional).</param>
        /// <returns>Task for the asynchronous log save.</returns>
        public Task Log(OperationType operationType, int userId, int? patientId = null, string data = null)
        {
            //Create a data attribute with info related to user and patient in case
            //the related entities are deleted.
            var operationData = "UserID: " + userId;
            if (patientId != null)
                operationData += ", PatientID: " + patientId;
            if (data != null)
                operationData += ", " + data;

            var operation = new Operation
            {
                TimeStamp = DateTime.Now,
                UserId = userId,
                Action = operationType,
                PatientId = patientId,
                OperationData = operationData
            };

            Repository.Operations.Add(operation);
            return Repository.SaveChangesAsync();
        }

        /// <summary>
        /// Save changes made to the database.
        /// </summary>
        protected void Save()
        {
            Repository.SaveChanges();
        }

        /// <summary>
        /// Dispose the database context.
        /// </summary>
        public virtual void Dispose()
        {
            Repository.Dispose();
        }
    }
}