using FPTBooking.Business.Models;
using FPTBooking.Business.Queries;
using FPTBooking.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FPTBooking.Business.Services
{
    public class DepartmentService : Service
    {
        public DepartmentService(ServiceInjection inj) : base(inj)
        {
        }

        #region Update Department
        public Department UpdateDepartment(Department entity, UpdateDepartmentModel model)
        {
            model.CopyTo(entity);
            return entity;
        }
        #endregion

        #region Delete Department
        public Department DeleteDepartment(Department entity)
        {
            entity = context.Department.Remove(entity).Entity;
            return entity;
        }
        #endregion

        #region Create Department
        protected void PrepareCreate(Department entity)
        {
        }
        public Department CreateDepartment(CreateDepartmentModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.Department.Add(entity).Entity;
        }
        #endregion

        #region Query Department
        public IQueryable<Department> Departments
        {
            get
            {
                return context.Department;
            }
        }

        public IDictionary<string, object> GetDepartmentDynamic(
            Department row, DepartmentQueryProjection projection,
            DepartmentQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case DepartmentQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["archived"] = entity.Archived;
                            obj["code"] = entity.Code;
                            obj["description"] = entity.Description;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case DepartmentQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.Name;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetDepartmentDynamic(
            IEnumerable<Department> rows, DepartmentQueryProjection projection,
            DepartmentQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetDepartmentDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryDepartmentDynamic(
            DepartmentQueryProjection projection,
            IDictionary<string, object> tempData = null,
            DepartmentQueryFilter filter = null,
            DepartmentQuerySort sort = null,
            DepartmentQueryPaging paging = null,
            DepartmentQueryOptions options = null)
        {
            var query = Departments.AsNoTracking();
            if (filter != null)
                query = query.Filter(filter, tempData);
            int? totalCount = null; Task<int> countTask = null;
            var countQuery = query;
            query = query.Project(projection);
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !DepartmentQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = countQuery.CountAsync();
                #endregion
            }
            if (options != null && options.count_total) totalCount = await countTask;
            var queryResult = await query.ToListAsync();
            if (options != null && options.single_only)
            {
                var single = queryResult.FirstOrDefault();
                if (single == null) return null;
                var singleResult = GetDepartmentDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            var results = GetDepartmentDynamic(queryResult, projection, options);
            return new QueryResult<IDictionary<string, object>>()
            {
                Results = results,
                TotalCount = totalCount
            };
        }
        #endregion

        #region Validation
        public ValidationData ValidateUpdateDepartment(ClaimsPrincipal principal,
            Department entity, UpdateDepartmentModel model)
        {
            var validationData = new ValidationData();
            if (string.IsNullOrWhiteSpace(model.Name))
                validationData = validationData.Fail(mess: "Name required", code: AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateDeleteDepartment(ClaimsPrincipal principal,
            Department entity)
        {
            return new ValidationData();
        }

        public ValidationData ValidateGetDepartments(
            DepartmentQueryFilter filter,
            DepartmentQuerySort sort,
            DepartmentQueryProjection projection,
            DepartmentQueryPaging paging,
            DepartmentQueryOptions options)
        {
            var validationData = new ValidationData();
            return validationData;
        }

        public ValidationData ValidateCreateDepartment(ClaimsPrincipal principal,
            CreateDepartmentModel model)
        {
            var validationData = new ValidationData();
            if (string.IsNullOrWhiteSpace(model.Code))
                validationData.Fail(mess: "Code required", code: AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.Name))
                validationData.Fail(mess: "Name required", code: AppResultCode.FailValidation);
            return validationData;
        }

        #endregion
    }
}
