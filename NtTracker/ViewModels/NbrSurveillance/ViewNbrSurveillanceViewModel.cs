using System.Collections.Generic;
using NtTracker.Models;

namespace NtTracker.ViewModels
{
    public class ViewNbrSurveillanceViewModel : AbstractViewModel
    {
        public int PatientId { get; set; }
        public PatientStatus PatientStatus { get; set; }
        public List<NbrSurveillance> NbrSurveillances { get; set; }
    }
}