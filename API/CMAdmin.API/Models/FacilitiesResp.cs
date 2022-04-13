using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Models
{
    public class FacilitiesResp
    {
        public TableData TableData { get; set; }
        public int TotalRecords { get; set; }
    }

    public class TableData
    {
        public List<KeyValueData> TableHeader { get; set; }
        public List<KeyValueData> TableRowData { get; set; }
    }

    public class KeyValueData
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class InstituteRole
    {
        public string CollegeId { get; set; }
        public string CollegeName { get; set; }
    }
}
