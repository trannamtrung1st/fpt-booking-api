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
    public class MemberServices : Service
    {
        [Inject]
        private readonly UserManager<AppUser> _userManager;
        public MemberServices(ServiceInjection inj) : base(inj)
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
        #endregion

        #region Create Member
        protected void PrepareCreate(Member entity)
        {
            if (entity.MemberTypeCode == null)
                entity.MemberTypeCode = MemberTypeValues.GENERAL;
        }

        public Member ConvertToMember(AppUser user)
        {
            var entity = new Member()
            {
                FirstName = user.FullName,
                LastName = user.FullName,
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.PhoneNumber,
            };
            return entity;
        }

        public Member CreateMember(Member entity)
        {
            PrepareCreate(entity);
            return context.Member.Add(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetMembers(
            ClaimsPrincipal principal,
            MemberQueryFilter filter,
            MemberQuerySort sort,
            MemberQueryProjection projection,
            MemberQueryPaging paging,
            MemberQueryOptions options)
        {
            return new ValidationData();
        }
        #endregion

    }
}
