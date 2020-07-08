using FPTBooking.Business.Helpers;
using FPTBooking.Business.Models;
using FPTBooking.Business.Queries;
using FPTBooking.Data;
using FPTBooking.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;
using TNT.Core.Helpers.General;

namespace FPTBooking.Business.Services
{
    public class BookingBusinessService : Service
    {
        [Inject]
        protected readonly MemberService _memberService;
        [Inject]
        protected readonly RoomBusinessService _roomService;

        public BookingBusinessService(ServiceInjection inj) : base(inj)
        {
        }

        #region Create Booking
        protected void PrepareCreate(Booking entity, Member bookMember, Room bookedRoom)
        {
            entity.Code = "B" + DateTime.UtcNow.ToString("ddMMyyyyhhmmss") + Global.Random.Next(0, 9);
            entity.Archived = false;
            entity.SentDate = DateTime.UtcNow;
            var managerDeps = _memberService.DepartmentMembers.OfMember(bookMember.UserId)
                .IsManager().Select(o => o.DepartmentCode).ToList();
            var isDepManager = managerDeps.Contains(bookedRoom.DepartmentCode);
            var isAreaManager = _memberService.AreaManagers.OfMember(bookMember.UserId)
                .Select(o => o.AreaCode)
                .Contains(bookedRoom.BuildingAreaCode);
            entity.Status = isDepManager ? BookingStatusValues.VALID :
                (isAreaManager ? BookingStatusValues.APPROVED : BookingStatusValues.PROCESSING);
        }

        public Booking CreateBooking(Member bookMember, Room bookedRoom,
            CreateBookingModel model, List<string> usingMemberIds)
        {
            var entity = model.ToDest();
            entity.BookMemberId = bookMember.UserId;
            entity.UsingMemberIds = string.Join("\n", usingMemberIds);
            PrepareCreate(entity, bookMember, bookedRoom);
            return context.Booking.Add(entity).Entity;
        }

        public BookingHistory CreateHistoryForCreateBooking(Booking booking, Member createMember)
        {
            var entity = new BookingHistory
            {
                BookingId = booking.Id,
                DisplayContent = $"{createMember.Email} created this booking",
                HappenedTime = DateTime.UtcNow,
                FromStatus = null,
                Id = Guid.NewGuid().ToString(),
                MemberId = createMember.UserId,
                ToStatus = booking.Status,
                Type = BookingHistoryTypes.CREATE,
            };
            return context.BookingHistory.Add(entity).Entity;
        }
        #endregion

        #region Update Booking
        public Booking CancelBooking(CancelBookingModel model, Booking entity)
        {
            model.CopyTo(entity);
            entity.Status = BookingStatusValues.ABORTED;
            return entity;
        }

        public Booking FeedbackBooking(FeedbackBookingModel model, Booking entity)
        {
            model.CopyTo(entity);
            entity.Status = BookingStatusValues.FINISHED;
            return entity;
        }

        public Booking ApproveBooking(ApproveBookingModel model, Booking entity)
        {
            model.CopyTo(entity);
            if (entity.Status == BookingStatusValues.PROCESSING)
            {
                entity.Status = BookingStatusValues.VALID;
                entity.DepartmentAccepted = true;
            }
            else entity.Status = BookingStatusValues.APPROVED;
            return entity;
        }
        #endregion

        #region Query Booking
        public IQueryable<Booking> Bookings
        {
            get
            {
                return context.Booking;
            }
        }


        public Booking GetBookingDetail(int id, BookingQueryProjection projection)
        {
            var entity = Bookings.Id(id).Project(projection).FirstOrDefault();
            return entity;
        }

        public IDictionary<string, object> GetBookingDynamic(
            Booking row, BookingQueryProjection projection,
            BookingQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case BookingQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            var bookedDate = entity.BookedDate
                               .ToTimeZone(options.time_zone, options.culture, Settings.Instance.SupportedLangs[0]);
                            var timeStr = bookedDate.ToString(options.date_format, options.culture, Settings.Instance.SupportedLangs[0]);
                            obj["booked_date"] = new
                            {
                                display = timeStr,
                                iso = $"{bookedDate.ToUniversalTime():s}Z"
                            };
                            obj["from_time"] = entity.FromTime;
                            obj["to_time"] = entity.ToTime;
                            obj["type"] = BookingTypeValues.BOOKING;
                            obj["status"] = entity.Status;
                            obj["archived"] = entity.Archived;
                            obj["book_member_id"] = entity.BookMemberId;
                            obj["note"] = entity.Note;
                            obj["num_of_people"] = entity.NumOfPeople;
                            obj["manager_message"] = entity.ManagerMessage;
                            obj["feedback"] = entity.Feedback;
                        }
                        break;
                    case BookingQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["id"] = entity.Code;
                            obj["code"] = entity.Code;
                        }
                        break;
                    case BookingQueryProjection.ROOM:
                        {
                            var entity = row.Room;
                            obj["room"] = new
                            {
                                code = entity.Code,
                                name = entity.Name
                            };
                        }
                        break;
                    case BookingQueryProjection.SERVICES:
                        {
                            var entities = row.AttachedService
                                .Select(o => o.BookingService).Select(o => new
                                {
                                    code = o.Code,
                                    name = o.Name
                                }).ToList();
                            obj["attached_services"] = entities;
                        }
                        break;
                    case BookingQueryProjection.MEMBER:
                        {
                            var entity = row.BookMember;
                            obj["book_member"] = new
                            {
                                user_id = entity.UserId,
                                email = entity.Email
                            };
                        }
                        break;
                    case BookingQueryProjection.USING_EMAILS:
                        {
                            var ids = row.UsingMemberIds.Split('\n');
                            var usingEmails = _memberService.Members.Ids(ids)
                                .Select(o => o.Email).ToList();
                            obj["using_emails"] = usingEmails;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetBookingDynamic(
            IEnumerable<Booking> rows, BookingQueryProjection projection,
            BookingQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetBookingDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public IQueryable<Booking> QueryBookingsManagedByManager(Member member)
        {
            var managerDeps = member.DepartmentMember.AsQueryable()
                .IsManager().Select(o => o.DepartmentCode).ToList();
            var areaManagers = member.AreaManager.Select(o => o.AreaCode).ToList();
            var query = Bookings.ManagedByDepsOrAreasExceptStatuses(managerDeps, null,
                areaManagers, null);
            return query;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryBookingDynamic(
            ClaimsPrincipal principal,
            Member member,
            BookingPrincipalRelationship relationship,
            BookingQueryProjection projection,
            IDictionary<string, object> tempData = null,
            BookingQueryFilter filter = null,
            BookingQuerySort sort = null,
            BookingQueryPaging paging = null,
            BookingQueryOptions options = null)
        {
            var query = Bookings;
            //---- multi-tenants ------
            if (principal != null)
            {
                var memberId = principal.Identity.Name;
                switch (relationship)
                {
                    case BookingPrincipalRelationship.Owner:
                        query = query.OfBookMember(memberId);
                        break;
                    case BookingPrincipalRelationship.Manager:
                        query = QueryBookingsManagedByManager(member);
                        break;
                }
            }
            //-------------------------
            if (filter != null)
                query = query.Filter(filter, tempData);
            int? totalCount = null; Task<int> countTask = null;
            query = query.Project(projection);
            if (options != null && !options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !BookingQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
                #region Count query
                if (options.count_total)
                    countTask = query.CountAsync();
                #endregion
            }
            var queryResult = await query.ToListAsync();
            if (options != null && options.single_only)
            {
                var single = queryResult.FirstOrDefault();
                if (single == null) return null;
                var singleResult = GetBookingDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    SingleResult = singleResult
                };
            }
            if (options != null && options.count_total) totalCount = await countTask;
            var results = GetBookingDynamic(queryResult, projection, options);
            return new QueryResult<IDictionary<string, object>>()
            {
                Results = results,
                TotalCount = totalCount
            };
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetBookings(
            ClaimsPrincipal principal,
            BookingPrincipalRelationship relationship,
            BookingQueryFilter filter,
            BookingQuerySort sort,
            BookingQueryProjection projection,
            BookingQueryPaging paging,
            BookingQueryOptions options)
        {
            var validationData = new ValidationData();
            return validationData;
        }

        public ValidationData ValidateGetBookingDetail(
            Booking entity,
            ClaimsPrincipal principal,
            BookingQueryOptions options)
        {
            var validationData = new ValidationData();
            var userId = principal.Identity.Name;
            if (entity.BookMemberId != userId)
            {
                var managerIds = _memberService.QueryManagersOfMember(entity.BookMemberId)
                    .Select(o => o.UserId);
                var areaManagerIds = _memberService.QueryManagersOfArea(entity.Room.BuildingAreaCode)
                    .Select(o => o.UserId);
                var finalManagerIds = managerIds.Union(areaManagerIds).ToList();
                if (!finalManagerIds.Contains(userId))
                    validationData.Fail(code: AppResultCode.AccessDenied);
            }
            return validationData;
        }

        public ValidationData ValidateCancelBooking(ClaimsPrincipal principal,
            Booking entity,
            CancelBookingModel model)
        {
            var validationData = new ValidationData();
            var userId = principal.Identity.Name;
            if (entity.BookMemberId != userId)
                validationData.Fail(code: AppResultCode.AccessDenied);
            var now = DateTime.UtcNow;
            if (entity.Status == BookingStatusValues.FINISHED ||
                entity.Status == BookingStatusValues.DENIED ||
                entity.Status == BookingStatusValues.ABORTED ||
                (entity.BookedDate == now.Date && entity.FromTime <= now.TimeOfDay) || entity.BookedDate > now.Date)
                validationData.Fail(mess: "Not allowed", code: AppResultCode.FailValidation);
            if (model.Feedback == null)
                validationData.Fail(mess: "You must provide a reason in feedback", code: AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateFeedbackBooking(ClaimsPrincipal principal,
            Booking entity,
            FeedbackBookingModel model)
        {
            var validationData = new ValidationData();
            var userId = principal.Identity.Name;
            if (entity.BookMemberId != userId)
                validationData.Fail(code: AppResultCode.AccessDenied);
            var now = DateTime.UtcNow;
            var startTime = entity.BookedDate.Date
                .AddMinutes(entity.FromTime.TotalMinutes);
            var allowFeedbackTime = entity.BookedDate.Date
                .AddMinutes(entity.ToTime.TotalMinutes)
                .AddHours(4);
            if (entity.Status != BookingStatusValues.APPROVED || allowFeedbackTime <= now
                || startTime >= now)
                validationData.Fail(mess: "Not allowed", code: AppResultCode.FailValidation);
            if (model.Feedback == null)
                validationData.Fail(mess: "You must provide a reason in feedback", code: AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateApproveBooking(ClaimsPrincipal principal,
            Member manager,
            Booking entity,
            ApproveBookingModel model)
        {
            var validationData = new ValidationData();
            var userId = principal.Identity.Name;
            if (entity.Status == BookingStatusValues.PROCESSING)
            {
                var bookMemberId = entity.BookMemberId;
                var managerIds = _memberService.QueryManagersOfMember(bookMemberId)
                    .Select(o => o.UserId).ToList();
                if (!managerIds.Contains(userId))
                    validationData.Fail(code: AppResultCode.AccessDenied);
            }
            else if (entity.Status == BookingStatusValues.VALID)
            {
                var areaCode = entity.Room.BuildingAreaCode;
                var managerIds = _memberService.QueryManagersOfArea(areaCode)
                    .Select(o => o.UserId).ToList();
                if (!managerIds.Contains(userId))
                    validationData.Fail(code: AppResultCode.AccessDenied);
            }
            else validationData.Fail(mess: "Invalid status", code: AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateCreateBooking(ClaimsPrincipal principal,
            CreateBookingModel model)
        {
            var memberQuery = _memberService.Members;
            var validationData = new ValidationData();
            DateTime currentTime = DateTime.UtcNow;
            if (model.BookedDate == null)
                validationData.Fail("Booked date must not be empty", AppResultCode.FailValidation);
            if (model.NumOfPeople == null || model.NumOfPeople == 0)
                validationData.Fail("Number of people is not valid", AppResultCode.FailValidation);
            else if (currentTime >= model.BookedDate)
                validationData.Fail(mess: "Booked date must be greater than current", AppResultCode.FailValidation);
            if (model.FromTime == null || model.ToTime == null)
                validationData.Fail(mess: "From time and to time must not be empty", AppResultCode.FailValidation);
            else if (model.FromTime >= model.ToTime)
                validationData.Fail(mess: "Time range is not valid", AppResultCode.FailValidation);
            if (model.UsingEmails == null || model.UsingEmails.Count == 0)
                validationData.Fail(mess: "At lease one using email is required", AppResultCode.FailValidation);
            else
            {
                var ids = memberQuery.ByEmails(model.UsingEmails).Select(o => o.UserId).ToList();
                if (ids.Count != model.UsingEmails.Count)
                    validationData.Fail(mess: "One or more emails not found", AppResultCode.FailValidation);
                else validationData.TempData["using_member_ids"] = ids;
            }
            if (validationData.IsValid)
            {
                var availableRoom = _roomService.Rooms.Code(model.RoomCode)
                    .AvailableForBooking(Bookings, model.BookedDate.Value, model.FromTime.Value,
                        model.ToTime.Value, model.NumOfPeople.Value).FirstOrDefault();
                if (availableRoom == null)
                    validationData.Fail("Room is not available for this booking", AppResultCode.FailValidation);
                else validationData.TempData["booked_room"] = availableRoom;
            }
            return validationData;
        }
        #endregion
    }
}
