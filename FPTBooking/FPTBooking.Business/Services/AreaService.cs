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
    public class AreaService : Service
    {
        public AreaService(ServiceInjection inj) : base(inj)
        {
        }



        #region Update BuildingArea
        public BuildingArea UpdateBuildingArea(BuildingArea entity, UpdateBuildingAreaModel model)
        {
            model.CopyTo(entity);
            return entity;
        }
        #endregion

        #region Delete BuildingArea
        public BuildingArea DeleteBuildingArea(BuildingArea entity)
        {
            entity = context.BuildingArea.Remove(entity).Entity;
            return entity;
        }
        #endregion

        #region Create BuildingArea
        protected void PrepareCreate(BuildingArea entity)
        {
        }
        public BuildingArea CreateBuildingArea(CreateBuildingAreaModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.BuildingArea.Add(entity).Entity;
        }
        #endregion

        #region Query BuildingArea
        public IQueryable<BuildingArea> BuildingAreas
        {
            get
            {
                return context.BuildingArea;
            }
        }

        public IDictionary<string, object> GetBuildingAreaDynamic(
            BuildingArea row, AreaQueryProjection projection,
            AreaQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case AreaQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["archived"] = entity.Archived;
                            obj["code"] = entity.Code;
                            obj["description"] = entity.Description;
                            obj["name"] = entity.Name;
                        }
                        break;
                    case AreaQueryProjection.SELECT:
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

        public List<IDictionary<string, object>> GetBuildingAreaDynamic(
            IEnumerable<BuildingArea> rows, AreaQueryProjection projection,
            AreaQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetBuildingAreaDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryAreaDynamic(
            AreaQueryProjection projection,
            IDictionary<string, object> tempData = null,
            AreaQueryFilter filter = null,
            AreaQuerySort sort = null,
            AreaQueryPaging paging = null,
            AreaQueryOptions options = null)
        {
            var query = BuildingAreas.AsNoTracking();
            if (filter != null)
                query = query.Filter(filter, tempData);
            int? totalCount = null; Task<int> countTask = null;
            var countQuery = query;
            query = query.Project(projection);
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !AreaQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetBuildingAreaDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            var results = GetBuildingAreaDynamic(queryResult, projection, options);
            return new QueryResult<IDictionary<string, object>>()
            {
                Results = results,
                TotalCount = totalCount
            };
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetAreas(
            AreaQueryFilter filter,
            AreaQuerySort sort,
            AreaQueryProjection projection,
            AreaQueryPaging paging,
            AreaQueryOptions options)
        {
            var validationData = new ValidationData();
            return validationData;
        }
        public ValidationData ValidateUpdateBuildingArea(ClaimsPrincipal principal,
            BuildingArea entity, UpdateBuildingAreaModel model)
        {
            var validationData = new ValidationData();
            if (string.IsNullOrWhiteSpace(model.Name))
                validationData = validationData.Fail(mess: "Name required", code: AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateDeleteBuildingArea(ClaimsPrincipal principal,
            BuildingArea entity)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateBuildingArea(ClaimsPrincipal principal,
            CreateBuildingAreaModel model)
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
