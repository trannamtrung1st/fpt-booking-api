using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FPTBooking.Business.Services
{
    public class DepartmentService : Service
    {
        public DepartmentService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query
        public IQueryable<Department> Departments
        {
            get
            {
                return context.Department;
            }
        }
        #endregion
    }
}
