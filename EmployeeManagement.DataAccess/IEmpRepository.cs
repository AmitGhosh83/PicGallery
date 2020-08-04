using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.DataAccess
{

    public interface IEmpRepository
    {
        EmployeeModel GetEmployee(int id);
        IEnumerable<EmployeeModel> GetAllEmployees();
        EmployeeModel Add(EmployeeModel employee);
        EmployeeModel Update(EmployeeModel employeeChanges);
        EmployeeModel Delete(int id);
    }
}
