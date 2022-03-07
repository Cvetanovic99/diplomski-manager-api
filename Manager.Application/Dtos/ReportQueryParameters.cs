using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Dtos
{
    public class ReportQueryParameters
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int? ProjectId { get; set; }
    }
}
