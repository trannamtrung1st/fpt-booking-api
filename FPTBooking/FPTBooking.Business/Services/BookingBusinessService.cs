﻿using FPTBooking.Business.Helpers;
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
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Inject]
        protected readonly MemberService _memberService;
        [Inject]
        protected readonly RoomBusinessService _roomService;

        public BookingBusinessService(ServiceInjection inj) : base(inj)
        {
        }

        #region Create Booking
        protected string GetBookingCode(Booking entity)
        {
            return "B-" + Global.Random.RandomStringFrom(RandomExtension.Uppers_Digits, 4) + "-" +
                entity.RoomCode + "-" +
                entity.BookedDate.ToDefaultTimeZone().ToString("ddMMyyyy") + "-" +
                entity.FromTime.ToString("hhmm") + entity.ToTime.ToString("hhmm");
        }
        protected void PrepareCreate(Booking entity, Member bookMember, Room bookedRoom)
        {
            entity.Code = GetBookingCode(entity);
            entity.Archived = false;
            entity.SentDate = DateTime.UtcNow;
            foreach (var e in entity.AttachedService)
                e.Booking = entity;
            //var managerDeps = _memberService.DepartmentMembers.OfMember(bookMember.UserId)
            //    .IsManager().Select(o => o.DepartmentCode).ToList();
            //var isDepManager = managerDeps.Contains(bookedRoom.DepartmentCode);
            var isDepManager = _memberService.DepartmentMembers.OfMember(bookMember.UserId)
                .IsManager().Any();
            var isAreaManager = _memberService.AreaMembers.OfMember(bookMember.UserId)
                .IsManager().Select(o => o.AreaCode)
                .Contains(bookedRoom.BuildingAreaCode);
            if (isDepManager)
            {
                entity.Status = BookingStatusValues.VALID;
                entity.DepartmentAccepted = true;
            }
            else if (isAreaManager)
            {
                entity.Status = BookingStatusValues.APPROVED;
                entity.DepartmentAccepted = true;
            }
            else entity.Status = BookingStatusValues.PROCESSING;
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

        public Booking ChangeApprovalStatusOfBooking(ChangeApprovalStatusOfBookingModel model, Booking entity)
        {
            model.CopyTo(entity);
            if (entity.Status == BookingStatusValues.PROCESSING)
            {
                if (model.IsApproved)
                {
                    entity.Status = BookingStatusValues.VALID;
                    entity.DepartmentAccepted = true;
                }
                else
                {
                    entity.Status = BookingStatusValues.DENIED;
                    entity.DepartmentAccepted = false;
                }
            }
            else if (model.IsApproved)
                entity.Status = BookingStatusValues.APPROVED;
            else entity.Status = BookingStatusValues.DENIED;
            return entity;
        }

        public Booking UpdateBooking(UpdateBookingModel model, Booking entity)
        {
            model.CopyTo(entity);
            if (model.RemoveServiceIds != null)
                RemoveBookingAttachedServices(model.RemoveServiceIds);
            return entity;
        }

        public IEnumerable<AttachedService> RemoveBookingAttachedServices(IEnumerable<int> serviceIds)
        {
            var entities = serviceIds.Select(o => new AttachedService
            {
                Id = o
            }).ToList();
            context.AttachedService.RemoveRange(entities);
            return entities;
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

        public bool CheckAvailabilityOfRoomForBooking(string userId, Booking entity, Room room)
        {
            var isAvailable = _roomService.Rooms.AvailableForBooking(
                userId,
                Bookings,
                entity.BookedDate,
                entity.FromTime,
                entity.ToTime,
                entity.NumOfPeople).Any(o => o.Code == room.Code);
            return isAvailable;
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
                            var sentDate = entity.SentDate
                                .ToTimeZone(options.time_zone, options.culture, Settings.Instance.SupportedLangs[0]);
                            var timeStr = sentDate.ToString(dateFormat: AppDateTimeFormat.DEFAULT_FORMAT_FOR_CONVERT);
                            obj["group_by_date_key"] = sentDate.ToString(dateFormat: AppDateTimeFormat.DEFAULT_DATE_FORMAT);
                            obj["sent_date"] = new
                            {
                                display = timeStr,
                                iso = $"{sentDate.ToUtc():s}Z"
                            };
                            var bookedDate = entity.BookedDate
                               .ToTimeZone(options.time_zone, options.culture, Settings.Instance.SupportedLangs[0]);
                            timeStr = bookedDate.ToString(options.date_format, options.culture, Settings.Instance.SupportedLangs[0]);
                            obj["booked_date"] = new
                            {
                                display = timeStr,
                                iso = $"{bookedDate.ToUtc():s}Z"
                            };
                            obj["from_time"] = entity.FromTime.ToString("hh\\:mm");
                            obj["to_time"] = entity.ToTime.ToString("hh\\:mm");
                            var startTime = bookedDate.Add(entity.FromTime);
                            timeStr = startTime.ToString(dateFormat: AppDateTimeFormat.DEFAULT_FORMAT_FOR_CONVERT);
                            obj["start_time"] = new
                            {
                                display = timeStr,
                                iso = $"{startTime.ToUtc():s}Z"
                            };
                            var finishTime = bookedDate.Add(entity.ToTime);
                            timeStr = finishTime.ToString(dateFormat: AppDateTimeFormat.DEFAULT_FORMAT_FOR_CONVERT);
                            obj["finish_time"] = new
                            {
                                display = timeStr,
                                iso = $"{finishTime.ToUtc():s}Z"
                            };
                            obj["type"] = entity.Id != 0 ? BookingTypeValues.BOOKING : BookingTypeValues.SCHEDULE;
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
                            if (entity != null)
                                obj["room"] = new
                                {
                                    code = entity.Code,
                                    name = entity.Name
                                };
                        }
                        break;
                    case BookingQueryProjection.SERVICES:
                        {
                            var entities = row.AttachedService?
                                .Select(o => new
                                {
                                    id = o.Id,
                                    code = o.BookingService.Code,
                                    name = o.BookingService.Name
                                }).ToList();
                            obj["attached_services"] = entities;
                        }
                        break;
                    case BookingQueryProjection.MEMBER:
                        {
                            var entity = row.BookMember;
                            if (entity != null)
                                obj["book_member"] = new
                                {
                                    user_id = entity.UserId,
                                    email = entity.Email,
                                    code = entity.Code
                                };
                        }
                        break;
                    case BookingQueryProjection.USING_EMAILS:
                        {
                            if (row.UsingMemberIds != null)
                            {
                                var ids = row.UsingMemberIds.Split('\n');
                                var usingEmails = _memberService.Members.Ids(ids)
                                    .Select(o => o.Email).ToList();
                                obj["using_emails"] = usingEmails;
                            }
                        }
                        break;
                }
            }
            return obj;
        }

        public IEnumerable<object> GetBookingDynamic(
            IEnumerable<Booking> rows, BookingQueryProjection projection,
            BookingQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetBookingDynamic(o, projection, options);
                list.Add(obj);
            }
            if (options.group_by == BookingQueryOptions.GROUP_BY_DATE)
            {
                return list.GroupBy(o => o["group_by_date_key"]).ToList();
            }
            return list;
        }

        public IQueryable<Booking> QueryBookingsManagedByManager(Member member)
        {
            var managerDeps = member.DepartmentMember.AsQueryable()
                .IsManager().Select(o => o.DepartmentCode).ToList();
            var managerAreas = member.AreaMember.AsQueryable()
                .IsManager().Select(o => o.AreaCode).ToList();
            var query = Bookings.ManagedByDepsOrAreasExceptStatuses(managerDeps, null,
                managerAreas, null);
            return query;
        }

        public async Task<QueryResult<object>> QueryCalendarDynamic(
            ClaimsPrincipal principal,
            Member member,
            BookingQueryProjection projection,
            DateTime date,
            IDictionary<string, object> tempData = null,
            BookingQueryOptions options = null)
        {
            var query = Bookings
                .BookedDate(date)
                .RelatedToMember(member.UserId)
                .Project(projection).ToList();
            int? totalCount = null;
            var emailInfo = member.Email.GetEmailInfo();
            List<Booking> fapBookings = new List<Booking>();
            try
            {
                var defaultTimeZone = date.ToDefaultTimeZone();
                if (emailInfo.Item2)
                    fapBookings = await Global.FapClient
                            .GetFAPStudentBookings(emailInfo.Item3, defaultTimeZone);
                else fapBookings = await Global.FapClient
                            .GetFAPTeacherBookings(emailInfo.Item3, defaultTimeZone);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            query.AddRange(fapBookings);
            query = query.AsQueryable().SortOldestBookedDateFirst().ToList();
            if (options != null)
            {
                if (options.count_total)
                    totalCount = query.Count;
            }
            var results = GetBookingDynamic(query, projection, options);
            return new QueryResult<object>()
            {
                Results = results,
                TotalCount = totalCount
            };
        }

        public async Task<QueryResult<object>> QueryBookingDynamic(
            ClaimsPrincipal principal,
            Member member,
            BookingPrincipalRelationship? relationship,
            BookingQueryProjection projection,
            IDictionary<string, object> tempData = null,
            BookingQueryFilter filter = null,
            BookingQuerySort sort = null,
            BookingQueryPaging paging = null,
            BookingQueryOptions options = null)
        {
            var query = Bookings.AsNoTracking();
            //---- multi-tenants ------
            if (principal != null)
            {
                var memberId = principal.Identity.Name;
                switch (relationship)
                {
                    case BookingPrincipalRelationship.Owner:
                        query = query.RelatedToMember(memberId);
                        break;
                    case BookingPrincipalRelationship.Manager:
                        query = QueryBookingsManagedByManager(member);
                        break;
                }
            }
            //-------------------------
            if (filter != null)
            {
                query = query.Filter(filter, tempData);
            }
            int? totalCount = null; Task<int> countTask = null;
            var countQuery = query;
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
                    countTask = countQuery.CountAsync();
                #endregion
            }
            if (options != null && options.count_total) totalCount = await countTask;
            var queryResult = await query.ToListAsync();
            if (options != null && options.single_only)
            {
                var single = queryResult.FirstOrDefault();
                if (single == null) return null;
                var singleResult = GetBookingDynamic(single, projection, options);
                return new QueryResult<object>()
                {
                    SingleResult = singleResult
                };
            }
            var results = GetBookingDynamic(queryResult, projection, options);
            return new QueryResult<object>()
            {
                Results = results,
                TotalCount = totalCount
            };
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetOwnerBookings(
            ClaimsPrincipal principal,
            DateTime date,
            BookingQueryProjection projection,
            BookingQueryOptions options)
        {
            var validationData = new ValidationData();
            return validationData;
        }

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
            if (filter.to_date != null && filter.from_date != null &&
                filter.to_date.Value.Subtract(filter.from_date.Value).TotalDays > 31)
                validationData.Fail(mess: "Only 1 month range is allowed", code: AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateGetRoomBookings(
            ClaimsPrincipal principal,
            BookingQueryFilter filter,
            BookingQuerySort sort,
            BookingQueryProjection projection,
            BookingQueryPaging paging,
            BookingQueryOptions options)
        {
            var validationData = new ValidationData();
            if (filter.to_date != null && filter.from_date != null &&
                filter.to_date.Value.Subtract(filter.from_date.Value).TotalDays > 31)
                validationData.Fail(mess: "Only 1 month range is allowed", code: AppResultCode.FailValidation);
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
                    .Select(o => o.UserId).ToList();
                var areaManagerIds = _memberService.QueryManagersOfArea(entity.Room.BuildingAreaCode)
                    .Select(o => o.UserId).ToList();
                var depManagerIds = _memberService.QueryManagersOfDepartment(entity.Room.DepartmentCode)
                    .Select(o => o.UserId).ToList();
                if (managerIds.Contains(userId) || depManagerIds.Contains(userId))
                    validationData.TempData["manager_type"] = "Department";
                else if (areaManagerIds.Contains(userId))
                    validationData.TempData["manager_type"] = "Area";
                else if (entity.UsingMemberIds.Contains(userId))
                    validationData.TempData["manager_type"] = "UsingMember";
                else validationData.Fail(code: AppResultCode.AccessDenied);
            }
            else validationData.TempData["manager_type"] = "Owner";
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
            var startTime = entity.BookedDate.Add(entity.FromTime);
            if (entity.Status == BookingStatusValues.FINISHED ||
                entity.Status == BookingStatusValues.DENIED ||
                entity.Status == BookingStatusValues.ABORTED ||
                startTime <= now)
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
            var bookedDateDefault = entity.BookedDate.ToDefaultTimeZone();
            var startTime = bookedDateDefault
                .AddMinutes(entity.FromTime.TotalMinutes).ToUtc();
            var allowFeedbackTime = bookedDateDefault
                .AddMinutes(entity.ToTime.TotalMinutes)
                .AddHours(4).ToUtc();
            if (entity.Status != BookingStatusValues.APPROVED || allowFeedbackTime <= now
                || startTime >= now)
                validationData.Fail(mess: "Not allowed", code: AppResultCode.FailValidation);
            if (model.Feedback == null)
                validationData.Fail(mess: "You must provide a reason in feedback", code: AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateChangeApprovalStausOfBooking(ClaimsPrincipal principal,
            Member manager,
            Booking entity,
            ChangeApprovalStatusOfBookingModel model)
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

        public ValidationData ValidateUpdateBooking(ClaimsPrincipal principal,
            Member manager,
            Booking entity,
            UpdateBookingModel model)
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
            if (model.RoomCode != entity.RoomCode)
            {
                var room = _roomService.Rooms.Code(model.RoomCode).FirstOrDefault();
                if (room == null)
                    validationData.Fail(mess: "Room not found", AppResultCode.FailValidation);
                else
                if (!CheckAvailabilityOfRoomForBooking(userId, entity, room))
                    validationData.Fail(mess: "New room is not available", AppResultCode.FailValidation);
                else validationData.TempData["room"] = room;
            }
            return validationData;
        }

        public ValidationData ValidateCreateBooking(ClaimsPrincipal principal,
            Member member,
            CreateBookingModel model)
        {
            var memberQuery = _memberService.Members;
            var userId = principal.Identity.Name;
            var validationData = new ValidationData();
            DateTime currentTime = DateTime.UtcNow;
            if (!member.DepartmentMember.Any() && !member.AreaMember.Any())
            {
                validationData.Fail(code: AppResultCode.AccessDenied);
                return validationData;
            }
            if (model.BookedDate == null)
                validationData.Fail("Booked date must not be empty", AppResultCode.FailValidation);
            if (model.NumOfPeople == null || model.NumOfPeople == 0)
                validationData.Fail("Number of people is not valid", AppResultCode.FailValidation);
            else if (model.FromTime == null || model.ToTime == null)
                validationData.Fail(mess: "From time and to time must not be empty", AppResultCode.FailValidation);
            else if (model.FromTime >= model.ToTime)
                validationData.Fail(mess: "Time range is not valid", AppResultCode.FailValidation);
            else
            {
                var bookedTime = model.BookedDate?.AddMinutes(model.FromTime.Value.TotalMinutes);
                if (currentTime >= bookedTime)
                    validationData.Fail(mess: "Booked time must be greater than current", AppResultCode.FailValidation);
            }
            if (model.UsingEmails == null || model.UsingEmails.Count == 0)
                validationData.Fail(mess: "At lease one using email is required", AppResultCode.FailValidation);
            else
            {
                var users = memberQuery.ByEmails(model.UsingEmails).Select(o => new
                {
                    user_id = o.UserId,
                    email = o.Email
                }).ToList();
                var ids = users.Select(o => o.user_id).ToList();
                var emails = users.Select(o => o.email).ToList();
                var notFoundEmails = model.UsingEmails.Where(o => !emails.Contains(o)).ToList();
                var notFoundEmailsList = string.Join(", ", notFoundEmails);
                if (notFoundEmails.Any())
                    validationData.Fail(mess: $"One or more emails not found: {notFoundEmailsList}\n" +
                        $"Maybe they need to log into the application at least once", AppResultCode.FailValidation);
                else validationData.TempData["using_member_ids"] = ids;
            }
            if (validationData.IsValid)
            {
                var availableRoom = _roomService.Rooms.Code(model.RoomCode)
                    .AvailableForBooking(userId, Bookings, model.BookedDate.Value, model.FromTime.Value,
                        model.ToTime.Value, model.NumOfPeople.Value).FirstOrDefault();
                if (availableRoom == null)
                    validationData.Fail("Room is not available for this booking", AppResultCode.FailValidation);
                else validationData.TempData["booked_room"] = availableRoom;
            }
            return validationData;
        }
        #endregion

        public BookingHistory CreateHistoryForCreateBooking(Booking booking, Member createMember)
        {
            var entity = new BookingHistory
            {
                Booking = booking,
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

        public BookingHistory CreateHistoryForChangeApprovalStatusOfBooking(Booking booking, string fromStatus,
            bool isApproved,
            Member approveManager)
        {
            var entity = new BookingHistory
            {
                Booking = booking,
                HappenedTime = DateTime.UtcNow,
                FromStatus = fromStatus,
                Id = Guid.NewGuid().ToString(),
                MemberId = approveManager.UserId,
                ToStatus = booking.Status,
            };
            if (isApproved)
            {
                entity.DisplayContent = $"{approveManager.Email} approved this booking";
                entity.Type = BookingHistoryTypes.APPROVE;
            }
            else
            {
                entity.DisplayContent = $"{approveManager.Email} denied this booking";
                entity.Type = BookingHistoryTypes.DENY;
            }
            return context.BookingHistory.Add(entity).Entity;
        }

        public BookingHistory CreateHistoryForUpdateBooking(Booking booking,
            Member approveManager)
        {
            var entity = new BookingHistory
            {
                Booking = booking,
                HappenedTime = DateTime.UtcNow,
                DisplayContent = $"{approveManager.Email} updated this booking",
                FromStatus = booking.Status,
                Id = Guid.NewGuid().ToString(),
                MemberId = approveManager.UserId,
                ToStatus = booking.Status,
                Type = BookingHistoryTypes.UPDATE,
            };
            return context.BookingHistory.Add(entity).Entity;
        }

        public BookingHistory CreateHistoryForFeedbackBooking(Booking booking, Member createMember)
        {
            var entity = new BookingHistory
            {
                Booking = booking,
                DisplayContent = $"{createMember.Email} posted a feedback for this booking",
                HappenedTime = DateTime.UtcNow,
                FromStatus = BookingStatusValues.APPROVED,
                Id = Guid.NewGuid().ToString(),
                MemberId = createMember.UserId,
                ToStatus = booking.Status,
                Type = BookingHistoryTypes.FEEDBACK,
            };
            return context.BookingHistory.Add(entity).Entity;
        }

        public BookingHistory CreateHistoryForCancelBooking(Booking booking, string fromStatus, Member createMember)
        {
            var entity = new BookingHistory
            {
                Booking = booking,
                DisplayContent = $"{createMember.Email} canceled this booking",
                HappenedTime = DateTime.UtcNow,
                FromStatus = fromStatus,
                Id = Guid.NewGuid().ToString(),
                MemberId = createMember.UserId,
                ToStatus = booking.Status,
                Type = BookingHistoryTypes.ABORT,
            };
            return context.BookingHistory.Add(entity).Entity;
        }
    }
}
