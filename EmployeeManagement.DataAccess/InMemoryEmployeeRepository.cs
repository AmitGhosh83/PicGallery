using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmployeeManagement.DataAccess
{
    public class InMemoryEmployeeRepository : IEmpRepository
    {
        private readonly List<EmployeeModel> _employeeList;
        public InMemoryEmployeeRepository()
        {
             _employeeList = new List<EmployeeModel>
             {
                 new EmployeeModel { Id= 1, Name= "Joey", Department= Dept.Joy, EmailId= "Joey@gmail.com", },
                 new EmployeeModel { Id= 2, Name= "Linda", Department= Dept.HR, EmailId= "linda@gmail.com", },
                 new EmployeeModel { Id= 3, Name= "Amit", Department= Dept.IT, EmailId= "amit@gmail.com", },
                 new EmployeeModel { Id= 4, Name= "Ghosh", Department= Dept.IT, EmailId= "ghosh@gmail.com", },
             };
        }

        public EmployeeModel Add(EmployeeModel employee)
        {
            employee.Id = _employeeList.Max(x => x.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public EmployeeModel Delete(int id)
        {
            var empModel= _employeeList.FirstOrDefault(x => x.Id == id);
            if(empModel!=null)
            {
                _employeeList.Remove(empModel);
            }
            return empModel;
        }

        public IEnumerable<EmployeeModel> GetAllEmployees()
        {
            return _employeeList.OrderBy(x => x.Id).ToList();
        }

        public EmployeeModel GetEmployee(int id)
        {
            var employee = _employeeList.FirstOrDefault(x => x.Id == id);
            return employee;
        }

        public EmployeeModel Update(EmployeeModel employeeChanges)
        {
            var empModel = _employeeList.FirstOrDefault(x => x.Id == employeeChanges.Id);
            if (empModel != null)
            {
                empModel.Name = employeeChanges.Name;
                empModel.EmailId = employeeChanges.EmailId;
                empModel.Department = employeeChanges.Department;
            }
            return empModel;
        }
    }
}
