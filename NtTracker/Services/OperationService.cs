using System;
using System.Data.Entity;
using System.Linq;
using NtTracker.Data;
using NtTracker.Models;
using PagedList;

namespace NtTracker.Services
{
    public class OperationService : Service, IOperationService
    {
        private readonly IUserAccountService _userAccountService;

        public OperationService(DataContext dataContext, IUserAccountService userAccountService) : base(dataContext)
        {
            _userAccountService = userAccountService;
        }

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
        public IPagedList<Operation> Search(string user, OperationType? operation, int? patient,
            string dateFrom, string dateTo, string operationData, string sorting, int pageNumber, int pageSize)
        {
            var operations = Repository.Operations.AsQueryable()
                .Include(o => o.User)
                .Include(o => o.Patient);

            //Filtering
            if (!string.IsNullOrEmpty(user))
            {
                var userAccount = _userAccountService.FindByUserName(user);
                var userId = userAccount?.Id ?? -1; //User id or -1 if invalid user
                operations = operations.Where(o => o.UserId == userId);
            }
            if (operation != null)
            {
                operations = operations.Where(o => o.Action == operation);
            }
            if (patient != null)
            {
                operations = operations.Where(o => o.PatientId == patient);
            }
            if (!string.IsNullOrEmpty(dateFrom))
            {
                DateTime dateComp;
                if (!DateTime.TryParse(dateFrom, out dateComp))
                {
                    dateComp = DateTime.MinValue;
                }
                operations = operations.Where(o => o.TimeStamp >= dateComp);
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                DateTime dateComp;
                if (!DateTime.TryParse(dateTo, out dateComp))
                {
                    dateComp = DateTime.MaxValue;
                }
                operations = operations.Where(o => o.TimeStamp <= dateComp);
            }
            if (!string.IsNullOrEmpty(operationData))
            {
                operations = operations.Where(o => o.OperationData.Contains(operationData));
            }

            //Sorting
            switch (sorting)
            {
                case "time_a":
                    operations = operations.OrderBy(o => o.TimeStamp);
                    break;
                case "time_d":
                    operations = operations.OrderByDescending(o => o.TimeStamp);
                    break;
                case "user_a":
                    operations = operations.OrderBy(o => o.UserId);
                    break;
                case "user_d":
                    operations = operations.OrderByDescending(o => o.UserId);
                    break;
                case "operation_a":
                    operations = operations.OrderBy(o => o.Action);
                    break;
                case "operation_d":
                    operations = operations.OrderByDescending(o => o.Action);
                    break;
                case "patient_a":
                    operations = operations.OrderBy(o => o.PatientId);
                    break;
                case "patient_d":
                    operations = operations.OrderByDescending(o => o.PatientId);
                    break;
                default:
                    operations = operations.OrderByDescending(o => o.TimeStamp);
                    break;
            }

            return operations.ToPagedList(pageNumber, pageSize);
        }

        public override void Dispose()
        {
            _userAccountService.Dispose();
            base.Dispose();
        }
    }
}