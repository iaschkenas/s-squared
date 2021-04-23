using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModels;

namespace s_squared.Controllers
{
    public class HomeController : Controller
    {
        static readonly Repository.Personnel _repo = new Repository.Personnel();

        public async Task<ActionResult> Index()
        {
            var vm = new Home();

            try
            {
                // Get a list of all managers for the dropdown
                var managers = await _repo.GetManagers();
                vm.Managers = managers.Select(m => new SelectListItem { Value = m.Key, Text = m.Value });

                // get the employees for the first manager in the list
                if (managers.Count() > 0)
                {
                    vm.Employees = await _repo.GetEmployeesByManager(managers.First().Key);
                }
            }
            catch
            {
                // TODO: return error to the user
            }

            return View(vm);
        }

        /// <summary>
        /// Gets the employees for the grid
        /// </summary>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> GetEmployees(string managerId)
        {
            var vm = new Home();

            try
            {
                vm.Employees = await _repo.GetEmployeesByManager(managerId);
            }
            catch
            {
                // TODO: return error to the user
            }

            return PartialView("_Employees", vm);
        }

        public async Task<ActionResult> AddEmployee()
        {
            var vm = new AddEmployee();

            // get all managers for the dropdown
            var managers = await _repo.GetManagers();
            vm.Managers = managers.Select(m => new SelectListItem { Value = m.Key, Text = m.Value });

            // get all roles for listbox
            var roles = await _repo.GetRoles();
            vm.Roles = roles.Select(m => new SelectListItem { Value = m.Key, Text = m.Value });

            return View("AddEmployee", vm);
        }

        [HttpPost]
        public async Task<ActionResult> AddEmployee(AddEmployee vm)
        {
            if (ModelState.IsValid)
            {
                var employee = new Domain.Employee
                {
                    EmployeeId = vm.EmployeeID,
                    LastName = vm.LastName,
                    FirstName = vm.FirstName,
                    ManagerID = vm.SelectedManagerId
                };

                try
                {
                    // adds the employee
                    var employeeId = await _repo.InsertEmployee(employee);

                    // if roles selected then add roles for the employee
                    if (vm.SelectedRoles.Count() > 0 && employeeId > 0)
                    {
                        await _repo.InsertEmployeeRoles(employeeId, vm.SelectedRoles);
                    }
                }
                catch
                {
                    // TODO: return error to the user
                }

                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    var managers = await _repo.GetManagers();
                    vm.Managers = managers.Select(m => new SelectListItem { Value = m.Key, Text = m.Value });

                    var roles = await _repo.GetRoles();
                    vm.Roles = roles.Select(m => new SelectListItem { Value = m.Key, Text = m.Value });
                }
                catch
                {
                    // TODO: return error to the user
                }

                return View("AddEmployee", vm);
            }
        }

    }
}