using System.Collections.Generic;
using NtTracker.Models;

namespace NtTracker.ViewModels
{
    public class ListMonitoringViewModel : AbstractViewModel
    {
        public int PatientId { get; set; }
        public List<Monitoring> Monitorings { get; set; }
    }
}