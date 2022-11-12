using System;
using SQLite;
using SyncApp.Helpers;

namespace SyncApp.Models
{
    [Table(Constants.TableEvents)]
    public class Event : BasicTable
    {
        [MaxLength(255)]
        public string EventName { get; set; }

        public DateTime EventDate { get; set; }
    }
}
