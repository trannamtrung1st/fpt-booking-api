using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Models
{
    #region Query
    public class RoomQueryProjection
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
        public const string ROOM_TYPE = "type";
        public const string DEPARTMENT = "department";
        public const string AREA = "area";
        public const string LEVEL = "level";
        public const string BLOCK = "block";
        private const string R = nameof(Room);
        private const string RT = nameof(RoomType);
        private const string D = nameof(Department);
        private const string A = nameof(BuildingArea);
        private const string L = nameof(BuildingLevel);
        private const string B = nameof(BuildingBlock);

        public static readonly IDictionary<string, string> FIELDS_MAPPING =
            new Dictionary<string, string>()
            {
                { ROOM_TYPE, nameof(Room.RoomType) },
                { DEPARTMENT, nameof(Room.Department) },
                { AREA, nameof(Room.BuildingArea) },
                { LEVEL,nameof(Room.BuildingLevel) },
                { BLOCK,nameof(Room.BuildingLevel) + "." + nameof(Room.BuildingLevel.BuildingBlock)},
            };

    }

    public class RoomQuerySort
    {
        public const string CODE = "code";
        private const string DEFAULT = "a" + CODE;
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

    public class RoomQueryFilter
    {
        public string code { get; set; }
        public string name_contains { get; set; }
        public string date_str { get; set; }
        public string from_time { get; set; }
        public string to_time { get; set; }
        public int? num_of_people { get; set; }
        public string room_type { get; set; }
        public bool empty { get; set; } //default false
        public BoolOptions? available { get; set; } //default both
        public BoolOptions? archived { get; set; } //default false
    }

    public class RoomQueryPaging
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

    public class RoomQueryOptions
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
