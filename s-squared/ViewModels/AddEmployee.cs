using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ViewModels
{
    public class AddEmployee
    {
        public AddEmployee()
        {
            Managers = new List<SelectListItem>();
            Roles = new List<SelectListItem>();
        }

        [Display(Name = "Employee ID")]
        [Required(ErrorMessage = "Required")]
        public string EmployeeID { get; set; }
        [Display(Name = "First Name")]
        [Required(ErrorMessage="Required")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Required")]
        public string LastName { get; set; }
        public IEnumerable<SelectListItem> Managers { get; set; }
        [Required(ErrorMessage = "Required")]
        public int SelectedManagerId { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
        public IEnumerable<int> SelectedRoles { get; set; }
    }
}