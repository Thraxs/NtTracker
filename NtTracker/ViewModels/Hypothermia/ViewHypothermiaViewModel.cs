using System.Collections.Generic;
using NtTracker.Models;

namespace NtTracker.ViewModels
{
    public class ViewHypothermiaViewModel : AbstractViewModel
    {
        public int PatientId { get; set; }
        public List<Hypothermia> Hypothermias { get; set; }
    }
}