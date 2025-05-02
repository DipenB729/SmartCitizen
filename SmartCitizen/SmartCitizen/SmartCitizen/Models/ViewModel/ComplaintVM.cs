using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartCitizen.Models.ViewModel
{
    public class ComplaintVM
    {
        public Complaint? Complaint { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? StatusList { get; set; } // Optional: If you want to provide status options

        [ValidateNever]
        public IEnumerable<Complaint>? ComplaintList { get; set; } // Optional: If you want to display multiple complaints
    }
}
