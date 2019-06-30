using Framework.ObjectModule;
using System.Data;

namespace MyORM
{
    [Table("Student")]
    public class Student22
    {
        [Column("Id")]
        [StringLength(128)]
        [DataType(SqlDbType.NChar)]
        [PrimaryKey]
        public string Id { get; set; }

        [Column("Name")]
        [StringLength(10)]
        [DataType(SqlDbType.NChar)]
        public string Name { get; set; }

        [Column("Age")]
        public int Age1 { get; set; }
    }
}
