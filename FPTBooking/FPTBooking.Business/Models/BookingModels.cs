using FPTBooking.Business.Helpers;
using FPTBooking.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Models
{
    public class CreateBookingModel : MappingModel<Booking>
    {
        public CreateBookingModel()
        {
        }

        public CreateBookingModel(Booking src) : base(src)
        {
        }

        [JsonConverter(typeof(DefaultDateTimeConverter))]
        public DateTime? BookedDate { get; set; }
        public int? NumOfPeople { get; set; }
        public string Note { get; set; }
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }
        public string RoomCode { get; set; }
        public List<string> UsingEmails { get; set; }

        public virtual ICollection<CreateAttachedServiceModel> AttachedService { get; set; }

    }

    public class CreateAttachedServiceModel : MappingModel<AttachedService>
    {
        public CreateAttachedServiceModel()
        {
        }

        public CreateAttachedServiceModel(AttachedService src) : base(src)
        {
        }

        [JsonProperty("service_code")]
        public string BookingServiceCode { get; set; }

    }

    #region Query
    public class BookingQueryProjection
    {
        private const string DEFAULT = INFO;
        private string _fields = DEFAULT;
        public string fields
        {
            get
            {
                return _fields;
            }
            set
            {
                if (value?.Length > 0)
                {
                    _fields = value;
                    _fieldsArr = value.Split(',').OrderBy(v => v).ToArray();
                }
            }
        }

        private string[] _fieldsArr = DEFAULT.Split(',');
        public string[] GetFieldsArr()
        {
            return _fieldsArr;
        }

        //---------------------------------------

        public const string INFO = "info";
        public const string SELECT = "select";
        public const string ROOM = "room";
        private const string B = nameof(Booking);
    }

    public class BookingQuerySort
    {
        public const string DATE = "date";
        private const string DEFAULT = "a" + DATE;
        private string _sorts = DEFAULT;
        public string sorts
        {
            get
            {
                return _sorts;
            }
            set
            {
                if (value?.Length > 0)
                {
                    _sorts = value;
                    _sortsArr = value.Split(',');
                }
            }
        }

        public string[] _sortsArr = DEFAULT.Split(',');
        public string[] GetSortsArr()
        {
            return _sortsArr;
        }

    }

    public class BookingQueryFilter
    {
        public int? id { get; set; }
        public string code { get; set; }
        [DefaultDateTimeModelBinder]
        public DateTime? date { get; set; }
        [DefaultDateTimeModelBinder]
        public DateTime? from_date { get; set; }
        [DefaultDateTimeModelBinder]
        public DateTime? to_date { get; set; }
        public BoolOptions? archived { get; set; } //default: false
    }

    public class BookingQueryPaging
    {
        private int _page = 1;
        public int page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value > 0 ? value : _page;
            }
        }

        private int _limit = 10;
        public int limit
        {
            get
            {
                return _limit;
            }
            set
            {
                if (value >= 1 && value <= 100)
                    _limit = value;
            }
        }
    }

    public class BookingQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public string time_zone { get; set; }
        public string culture { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }

    #endregion
}
