using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KudVenvat1.Models;
using KudVenvat1.ViewModels;
using EmployeeManagement.DataAccess;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace KudVenvat1.Controllers
{

    public class HomeController : Controller
    {
        private readonly IEmpRepository _empRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IEmpRepository empRepository, IHostingEnvironment hostingEnvironment)
        {

            _empRepository = empRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = _empRepository.GetAllEmployees()
                        .Select(t =>
                        new Employee
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Email = t.EmailId,
                            Department = (KudVenvat1.Models.Dept)t.Department,
                            PhotoPath=t.PhotoPath
                            
                        }) ;

            return View(model);
        }

        [AllowAnonymous]
        public ViewResult Details( int? id)
        {
            //throw new Exception("Error occured");
            HomeDetailsViewModel viewModel = new HomeDetailsViewModel();
            var result = _empRepository.GetEmployee(id ?? 1);
            if(result==null)
            {
                Response.StatusCode = 404;
                return View("NotFound", id.Value);
            }
            var model = new Employee
            {
                Id = result.Id,
                Name = result.Name,
                Email = result.EmailId,
                Department = (KudVenvat1.Models.Dept)result.Department,
                PhotoPath = result.PhotoPath  // Added this here
            };

            viewModel.Employee = model;
            viewModel.PageTitle = "Employee Details";

            return View(viewModel);
        }

        [HttpGet]

        public IActionResult Create()
        {

            return View();
        }
 
        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel emp)
        {
            if (ModelState.IsValid)
            {
                string uniqueFilename = ProcessPhoto(emp);
                //CreateViewModel --> Website.Model.Employee
                var newEmployee = new Employee
                {
                    Name = emp.Name,
                    Email = emp.Email,
                    Department = emp.Department,
                    PhotoPath = uniqueFilename
                };
                //Website.Model.Employee--> DataAccess.Repository.EmployeeModel
                var employeeModel = new EmployeeModel
                {
                    Name = newEmployee.Name,
                    EmailId = newEmployee.Email,
                    Department = (EmployeeManagement.DataAccess.Dept)newEmployee.Department,
                    PhotoPath = newEmployee.PhotoPath  // Added this here
                };
                EmployeeModel newEmpployee = _empRepository.Add(employeeModel);

                // DataAccess.Repository.EmployeeModel--> Website.Model.Employee
                Employee employee = new Employee
                {
                    Id = newEmpployee.Id,
                    Name = newEmpployee.Name,
                    Email = newEmpployee.EmailId,
                    Department = (KudVenvat1.Models.Dept)newEmpployee.Department,
                    PhotoPath= newEmpployee.PhotoPath // Added this here
                };

                return RedirectToAction("Details", new { id = newEmpployee.Id });
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = _empRepository.GetAllEmployees()
                            .Where( x=>x.Id==id)
                            .Select(t => new EmployeeEditViewModel
                            {
                                Id = id,
                                Name = t.Name,
                                Email = t.EmailId,
                                Department= (KudVenvat1.Models.Dept)t.Department,
                                ExistingPhotoPath= t.PhotoPath
                            }).First();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel emp)
        {

            if (ModelState.IsValid)
            {
                var input = _empRepository.GetEmployee(emp.Id);
                input.Name = emp.Name;
                input.EmailId = emp.Email;
                input.Department = (EmployeeManagement.DataAccess.Dept)emp.Department;

                //If the user has uploaded a new picture, update the details
                if(emp.Photo!=null)
                {
                    //Delete the old picture if the user uploads a new picture
                    if (emp.ExistingPhotoPath != null)
                    {
                        string filePath= Path.Combine(hostingEnvironment.WebRootPath, "Images", emp.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    string uniqueFilename = ProcessPhoto(emp);
                    input.PhotoPath = uniqueFilename;
                }

                var updatedEmp = _empRepository.Update(input);


                //DataAccess.Repository.EmployeeModel --> EmployeeEditViewModel
                var model = new EmployeeEditViewModel
                {
                    Id= updatedEmp.Id,
                    Name= updatedEmp.Name,
                    Email= updatedEmp.EmailId,
                    Department= (KudVenvat1.Models.Dept)updatedEmp.Department,
                    ExistingPhotoPath = updatedEmp.PhotoPath
                };
                return RedirectToAction("Details", new { id = model.Id });
            }
            else
            {
                return View();
            }
        }


        private string ProcessPhoto(EmployeeCreateViewModel emp)
        {
            string uniqueFilename = null;
            if (emp.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Images");
                uniqueFilename = Guid.NewGuid().ToString() + "_" + emp.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFilename);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    emp.Photo.CopyTo(fs);
                }
            }

            return uniqueFilename;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
