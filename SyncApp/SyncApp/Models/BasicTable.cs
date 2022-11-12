using SQLite;

namespace SyncApp.Models
{
    public class BasicTable
    {
        [PrimaryKey, AutoIncrement]
        public int IdLocal { get; set; }

        public bool IsActive { get; set; }

        public int IdServer { get; set; }

        public BasicTable()
        {

        }
    }
}
