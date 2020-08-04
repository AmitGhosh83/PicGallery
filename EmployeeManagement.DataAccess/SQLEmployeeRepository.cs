using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmployeeManagement.DataAccess
{
    public class SQLEmployeeRepository : IEmpRepository
    {
        private readonly AppDbContext context;

        public SQLEmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }
        public EmployeeModel Add(EmployeeModel employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public EmployeeModel Delete(int id)
        {
            EmployeeModel empModel= context.Employees.Find(id);
            if(empModel!=null)
            {
                context.Employees.Remove(empModel);
                context.SaveChanges();
            }
            return empModel;
        }

        public IEnumerable<EmployeeModel> GetAllEmployees()
        {
            return context.Employees;
        }

        public EmployeeModel GetEmployee(int id)
        {
            var empModel= context.Employees.Find(id);
            return empModel;
        }

        public EmployeeModel Update(EmployeeModel employeeChanges)
        {
            var empModel= context.Employees.Attach(employeeChanges);
            empModel.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return employeeChanges;
        }
    }
}
