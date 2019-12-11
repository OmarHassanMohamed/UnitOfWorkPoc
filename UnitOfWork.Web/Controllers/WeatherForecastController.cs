using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UnitOfWorkPoc;

namespace UnitOfWork.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private UnitOfWork<ProjectDbContext> unitOfWork = new UnitOfWork<ProjectDbContext>();
        private GenericRepository<Employee> repository;

        public WeatherForecastController()
        {
           repository = new GenericRepository<Employee>(unitOfWork);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = repository.GetAll();
            //Using Specific Repository
            //var model = employeeRepository.GetEmployeesByDepartment(1);
            return Ok(model);
        }

        [HttpPost]
        public ActionResult AddEmployee(Employee model)
        {
            try
            {
                unitOfWork.CreateTransaction();
                if (ModelState.IsValid)
                {
                    repository.Insert(model);
                    unitOfWork.Save();
                    //Do Some Other Task with the Database
                    //If everything is working then commit the transaction else rollback the transaction
                    unitOfWork.Commit();
                    return RedirectToAction("Index", "Employee");
                }
            }
            catch (Exception)
            {
                //Log the exception and rollback the transaction
                unitOfWork.RollBack();
            }
            return Ok();
        }

        [HttpPost]
        public ActionResult EditEmployee(Employee model)
        {
            if (ModelState.IsValid)
            {
                repository.Update(model);
                unitOfWork.Save();
                return Ok();
            }
            else
            {
                return Ok(model);
            }
        }
        [HttpPost]
        public ActionResult Delete(int EmployeeID)
        {
            Employee model = repository.GetById(EmployeeID);
            repository.Delete(model);
            unitOfWork.Save();
            return Ok();
        }

    }
}
