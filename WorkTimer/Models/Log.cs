using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimer.Models
{
    public class Log
    {
        [PrimaryKey, AutoIncrement]
        public int LogId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
    }
}
