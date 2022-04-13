using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Models
{
    public class AdminDashboard
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Alias { get; set; }
        public string SequenceNo { get; set; }
        public string URL { get; set; }
        public string CssClass { get; set; }
        public string IconClass { get; set; }
        public string Type { get; set; }
        public string DashboardCount { get; set; }
    }
}
