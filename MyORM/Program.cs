using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyORM
{
    class Program
    {
        static void Main(string[] args)
        {
            //Query();

            Insert();
        }

        private static void Query()
        {
            using (var context = new SchoolContext())
            {
                var data = context.Students2;


                //var result5 = data.Select(c => c).ToList();

                var result1 = data.Select(b => new { Name = b.Name, Age12 = b.Age1 }).ToList();

                var result13 = data.Select(b => b.Name).ToList();

                //var result2 = data.Select(b => new Student { Name = b.Name, Age = b.Age }).ToList();

                var result3 = data.OrderBy(b => b.Name).ThenBy(b => b.Age1).ToList();

                var result4 = data.GroupBy(a => new { Age12 = a.Age1 }).SelectMany(group => group).Select(c => c.Age1).ToList();

                //var result52 = data.GroupBy(a => new { Age2 = a.Age, Name2 = a.Name }).SelectMany(group => group).Select(b => new { Name = b.Name, Age = b.Age }).ToList();

                //var result15 = data.Where(a => a.Name == "name2").ToList();

                //var result23 = data.OrderBy(a => a.Age).ThenBy(a => a.Name).ToList();

                //var result32 = data.OrderByDescending(a => a.Age).Where(a => a.Name == "name2" && a.Age >= 3).ToList();

                //var result143 = data.Where(a => a.Name == "name2" && a.Age >= 3).Select(b => new Student { Name = b.Name, Age = b.Age }).OrderBy(a => a.Age).ToList();

                //var result = data.Where(a => a.Name == "name2").Where(b => b.Age >= 3).Where(c => c.Id == "2").ToList();

                //var result43 = data.Where(a => a.Name == "name2").Where(b => b.Age >= 3 && b.Id == "2").ToList();
            }
        }

        private static void Insert()
        {
            using (var context = new SchoolContext())
            {
                //context.Students2.Add(new Student22 { Id = Guid.NewGuid().ToString(), Age1 = 21, Name = "name221" });
                //context.Students2.Add(new Student22 { Id = Guid.NewGuid().ToString(), Age1 = 12, Name = "name122" });
                //context.Students2.AddRange(new List<Student22>());

                //var datas = context.Students2.Where(s => s.Age1 == 21);
                //context.Students2.Remove(datas);

                //context.Students2.UpdateEntry(new Student22 { Id = "123" }).Attach(a => a.Age1 == 0);

                var updateData = new Student22 { Id = Guid.NewGuid().ToString(), Name = "name122", Age1 = 22223 };
                //context.Students2.UpdateEntry(updateData).Modified(a => a.Age1).Condition(a => a.Name == "name122" && a.Id == "9321e81a-6357-4a51-81fc-8e529a7c2c1d");
                context.Students2.UpdateEntry(updateData).Modified(a => a.Id).Condition(a => string.Equals(a.Name, "name122"));

                var ret = context.Merge();
            }
        }
    }
}
