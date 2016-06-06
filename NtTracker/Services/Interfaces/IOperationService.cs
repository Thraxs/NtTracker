using NtTracker.Models;
using PagedList;

namespace NtTracker.Services
{
    public interface IOperationService : IService
    {
        /// <summary>
        /// Performs a search of the system operations with the given parameters.
        /// All the parameters can be null so that they are not used.
        /// </summary>
        /// <param name="user">User that performed the operation.</param>
        /// <param name="operation">Operation type.</param>
        /// <param name="patient">Operations that affect this patient.</param>
        /// <param name="dateFrom">Operations after this time.</param>
        /// <param name="dateTo">Operations before this time.</param>
        /// <param name="operationData">Operations with this data.</param>
        /// <param name="sorting">Sorting type.</param>
        /// <param name="pageNumber">Page number for listing pagination.</param>
        /// <param name="pageSize">Page size for listing pagination.</param>
        /// <returns>A paged list with the result of the search sorted as indicated.</returns>
        IPagedList<Operation> Search(string user, OperationType? operation, int? patient,
            string dateFrom, string dateTo, string operationData, string sorting, int pageNumber, int pageSize);
    }
}
