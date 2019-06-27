using Framework.ObjectModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyORM
{
    public class SchoolContext : DbContextBase
    {
        public SchoolContext() : base("dbname")
        {
            //this.Students = new QuerySet<Student>();
            //this.Addresses = new QuerySet<Address>();
            this.Students2 = new QuerySet<Student22>();
        }

        //public QuerySet<Student> Students { get; set; }

        //public QuerySet<Address> Addresses { get; set; }

        public QuerySet<Student22> Students2 { get; set; }
    }
}
