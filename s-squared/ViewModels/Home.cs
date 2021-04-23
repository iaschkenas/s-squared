using Domain;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ViewModels
{
    public class Home
    {
        public Home()
        {
            Managers = new List<SelectListItem>();
            Employees = new List<Employee>();
        }

        public IEnumerable<SelectListItem> Managers { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public string SelectedManagerId { get; set; }
    }
}