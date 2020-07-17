﻿using Microsoft.EntityFrameworkCore;
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

        #endregion

        #region Create Member
        protected void PrepareCreate(Member entity)
        {
            if (entity.MemberTypeCode == null)
                entity.MemberTypeCode = MemberTypeName.GENERAL;
        }

        public Member ConvertToMember(AppUser user, string code, string memberType)
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
                MemberTypeCode = memberType
            };
            //Dev only
            if (Settings.Instance.DevMode)
                entity.DepartmentMember = code == null ? null : new List<DepartmentMember>
                {
                    new DepartmentMember
                    {
                        DepartmentCode = "DOE",
                        IsManager = false,
                        MemberId = entity.UserId
                    }
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
        #endregion

    }
}
