using Microsoft.EntityFrameworkCore;
using FPTBooking.Business.Models;
using FPTBooking.Business.Queries;
using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;
using System.Drawing.Printing;
using FPTBooking.Data;
using Microsoft.AspNetCore.Identity;

namespace FPTBooking.Business.Services
{
    public class MemberService : Service
    {
        public MemberService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query Member
        public IQueryable<Member> Members
        {
            get
            {
                return context.Member;
            }
        }

        public IQueryable<DepartmentMember> DepartmentMembers
        {
            get
            {
                return context.DepartmentMember;
            }
        }

        public IQueryable<AreaMember> AreaMembers
        {
            get
            {
                return context.AreaMember;
            }
        }

        public IQueryable<Member> QueryManagersOfMember(string memberId)
        {
            var depCodes = DepartmentMembers.OfMember(memberId)
                .IsNotManager().Select(o => o.DepartmentCode).ToList();
            return Members.IsManagerOfAny(depCodes);
        }

        public IQueryable<Member> QueryManagersOfDepartment(string depCode)
        {
            return DepartmentMembers.OfDep(depCode)
                .IsManager().Select(o => o.Member);
        }

        public IQueryable<Member> QueryManagersOfArea(string areaCode)
        {
            return AreaMembers.OfArea(areaCode)
                .IsManager().Select(o => o.Member);
        }

        public IDictionary<string, object> GetMemberDynamic(
            Member row, MemberQueryProjection projection,
            MemberQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case MemberQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["code"] = entity.Code;
                            obj["email"] = entity.Email;
                            obj["first_name"] = entity.FirstName;
                            obj["full_name"] = entity.FullName;
                            obj["last_name"] = entity.LastName;
                            obj["middle_name"] = entity.MiddleName;
                            obj["phone"] = entity.Phone;
                            obj["user_id"] = entity.UserId;
                        }
                        break;
                    case MemberQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["code"] = entity.Code;
                            obj["name"] = entity.FullName;
                        }
                        break;
                    case MemberQueryProjection.DEPARTMENT:
                        {
                            var entities = row.DepartmentMember
                                .Select(o => new
                                {
                                    id = o.Id,
                                    name = o.Department.Name,
                                    code = o.DepartmentCode,
                                    is_manager = o.IsManager
                                }).ToList();
                            obj["departments"] = entities;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetMemberDynamic(
            IEnumerable<Member> rows, MemberQueryProjection projection,
            MemberQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetMemberDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryMembersDynamic(
            MemberQueryProjection projection,
            IDictionary<string, object> tempData = null,
            MemberQueryFilter filter = null,
            MemberQuerySort sort = null,
            MemberQueryPaging paging = null,
            MemberQueryOptions options = null)
        {
            var query = Members.AsNoTracking();
            if (filter != null)
                query = query.Filter(filter, tempData);
            int? totalCount = null; Task<int> countTask = null;
            var countQuery = query;
            query = query.Project(projection);
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !MemberQueryOptions.IsLoadAllAllowed))
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
                var singleResult = GetMemberDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            var results = GetMemberDynamic(queryResult, projection, options);
            return new QueryResult<IDictionary<string, object>>()
            {
                Results = results,
                TotalCount = totalCount
            };
        }
        #endregion

        #region Create Member
        protected void PrepareCreate(Member entity)
        {
        }

        public Member UpdateMember(Member entity, AppUser user)
        {
            entity.FirstName = user.FullName;
            entity.LastName = user.FullName;
            entity.UserId = user.Id;
            entity.Email = user.Email;
            entity.FullName = user.FullName;
            entity.Phone = user.PhoneNumber;
            entity.Code = user.MemberCode;
            return entity;
        }

        public Member UpdateMember(Member entity, UpdateMemberModel model)
        {
            model.CopyTo(entity);
            DeleteAllDepartmentMemberOf(entity);
            var depMembers = model.UpdateDepartmentMembers.Select(o =>
            {
                var dm = o.ToDest();
                dm.MemberId = entity.UserId;
                return dm;
            }).ToList();
            context.DepartmentMember.AddRange(depMembers);
            return entity;
        }

        public void DeleteAllDepartmentMemberOf(Member entity)
        {
            context.DepartmentMember.RemoveRange(entity.DepartmentMember);
        }
        public void DeleteAllAreaMemberOf(Member entity)
        {
            context.AreaMember.RemoveRange(entity.AreaMember);
        }
        public void DeleteAllAppEventsOf(Member entity)
        {
            context.AppEvent.RemoveRange(entity.AppEvent);
        }


        public Member ConvertToMember(AppUser user, string code)
        {
            var entity = new Member()
            {
                FirstName = user.FullName,
                LastName = user.FullName,
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.PhoneNumber,
                Code = code,
            };
            return entity;
        }

        public Member CreateMember(Member entity)
        {
            PrepareCreate(entity);
            return context.Member.Add(entity).Entity;
        }

        public Member CreateMember(CreateMemberModel model, AppUser userEntity, (bool, bool, string) emailInfo)
        {
            var entity = ConvertToMember(userEntity, emailInfo.Item3);
            PrepareCreate(entity);
            entity.DepartmentMember.Add(new DepartmentMember
            {
                DepartmentCode = model.DepartmentCode,
                IsManager = model.IsManager,
                MemberId = entity.UserId
            });
            return context.Member.Add(entity).Entity;
        }
        #endregion

        #region Update Member
        public void PrepareUpdate(Member entity)
        {
        }

        public Member UpdateMember(Member entity)
        {
            PrepareUpdate(entity);
            return context.Member.Update(entity).Entity;
        }
        #endregion

        #region Delete Member
        public Member DeleteMemberTransaction(Member entity)
        {
            entity = context.Member.Remove(entity).Entity;
            DeleteAllDepartmentMemberOf(entity);
            DeleteAllAreaMemberOf(entity);
            DeleteAllAppEventsOf(entity);
            return entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateDeleteMember(ClaimsPrincipal principal,
            Member entity)
        {
            return new ValidationData();
        }

        public ValidationData ValidateGetMembers(
            MemberQueryFilter filter,
            MemberQuerySort sort,
            MemberQueryProjection projection,
            MemberQueryPaging paging,
            MemberQueryOptions options)
        {
            var validationData = new ValidationData();
            return validationData;
        }

        public ValidationData ValidateUpdateMember(ClaimsPrincipal principal,
            Member entity, UpdateMemberModel model)
        {
            var validationData = new ValidationData();
            if (string.IsNullOrWhiteSpace(entity.Email))
                validationData = validationData.Fail(mess: "Email required", code: AppResultCode.FailValidation);
            return validationData;
        }
        #endregion

    }
}
