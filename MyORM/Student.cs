using Framework.ObjectModule;
using System.Data;

namespace MyORM
{
    [Table("Student")]
    public class Student
    {
        [Column("Id")]
        [StringLength(128)]
        [DataType(SqlDbType.NChar)]
        public string Id { get; set; }

        [Column("Name")]
        [StringLength(10)]
        [DataType(SqlDbType.NChar)]
        public string Name { get; set; }

        public int Age { get; set; }
    }
}
