using Newtonsoft.Json;
using FPTBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBooking.Business.Models
{
    public class UpdateMemberModel : MappingModel<Member>
    {
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("departments")]
        public List<UpdateDepartmentMemberModel> UpdateDepartmentMembers { get; set; }
        [JsonProperty("roles")]
        public HashSet<string> Roles { get; set; }

        public UpdateMemberModel()
        {
        }

        public UpdateMemberModel(Member src) : base(src)
        {
        }
    }

    public class UpdateDepartmentMemberModel : MappingModel<DepartmentMember>
    {

        [JsonProperty("department_code")]
        public string DepartmentCode { get; set; }
        [JsonProperty("is_manager")]
        public bool? IsManager { get; set; }

        public UpdateDepartmentMemberModel()
        {
        }

        public UpdateDepartmentMemberModel(DepartmentMember src) : base(src)
        {
        }
    }

    public class CreateMemberModel : MappingModel<Member>
    {
        [JsonProperty("is_manager")]
        public bool IsManager { get; set; }
        [JsonProperty("department_code")]
        public string DepartmentCode { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("roles")]
        public HashSet<string> Roles { get; set; }

        public CreateMemberModel()
        {
        }

        public CreateMemberModel(Member src) : base(src)
        {
        }
    }

    #region Query
    public class MemberQueryProjection
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
        public const string DEPARTMENT = "department";
        public const string ROLES = "roles";
        private const string T = AppUser.TBL_NAME;
    }

    public class MemberQuerySort
    {
        public const string NAME = "name";
        public const string EMAIL = "email";
        private const string DEFAULT = "a" + EMAIL;
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

    public class MemberQueryFilter
    {
        public string search { get; set; }
    }

    public class MemberQueryPaging
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

    public class MemberQueryOptions
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
