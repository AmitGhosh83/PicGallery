using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.DataAccess
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder builder)
        {
            builder.Entity<EmployeeModel>().HasData(
            new EmployeeModel
            {
                Id = 1,
                Name = "Joey",
                Department = Dept.Joy,
                EmailId = "joey@gmail.com"
            },
            new EmployeeModel
            { 
                Id=2,
                Name="Linda",
                Department= Dept.HR,
                EmailId="linda@gmail.com"
            },
            new EmployeeModel
            {
                Id = 3,
                Name = "Amit",
                Department = Dept.IT,
                EmailId = "amit@gmail.com"
            });
        }
    }
}
