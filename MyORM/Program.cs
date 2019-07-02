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
            Query();

            //Insert();
        }

        private static void Query()
        {
            using (var context = new SchoolContext())
            {
                var data = context.Students2;

                //var result = data.Where(a => a.Name == "name12").OrderByDescending(a => a.Age1).First(a => a.Age1 == 12);

                //var result = data.First(a => a.Name == "name12");

                //var result2 = data.First(a => a.Age1 == 12);

                //var result23 = data.Where(a => a.Name == "name12").First();

                var result111 = data.Count(d => d.Age1 == 12);
                var result222 = data.Where(a => a.Name == "name12").Count();
                var result333 = data.Where(a => a.Name == "name12").Count(d => d.Age1 == 12);


                //var result2 = data.Where(a => a.Name == "name12").Count();

                //var lst = new List<string> { "name12", "name21" };
                //var result152 = data.Where(a => lst.Contains(a.Name)).Where(b => b.Age1 > 12 && b.Age1 < 1200).ToList();

                //var result5 = data.Select(c => c).ToList();

                //var result1 = data.Select(b => new { Name = b.Name, Age12 = b.Age1 }).ToList();

                //var result13 = data.Select(b => b.Name).ToList();

                //var result2 = data.Select(b => new Student { Name = b.Name, Age = b.Age }).ToList();

                //var result3 = data.OrderBy(b => b.Name).ThenBy(b => b.Age1).ToList();

                //var result4 = data.GroupBy(a => new { Age12 = a.Age1 }).SelectMany(group => group).Select(c => c.Age1).ToList();

                //var result52 = data.GroupBy(a => new { Age2 = a.Age, Name2 = a.Name }).SelectMany(group => group).Select(b => new { Name = b.Name, Age = b.Age }).ToList();

                //var result15 = data.Where(a => a.Name == "name2").ToList();

                //var result23 = data.OrderBy(a => a.Age1).ThenBy(a => a.Name).ToList();

                //var result32 = data.OrderByDescending(a => a.Age1).Where(a => a.Name == "name2" && a.Age1 >= 3).ToList();

                //var result143 = data.Where(a => a.Name == "name2" && a.Age1 >= 3).Select(b => new Student22 { Name = b.Name, Age1 = b.Age1 }).OrderBy(a => a.Age1).ToList();

                //var result = data.Where(a => a.Name == "name12").Where(b => b.Age1 >= 12).ToList();

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

                var updateData = new Student22 { Id = "235092f4-e363-4497-ae8b-0b159ffa7638", Name = "name1222222", Age1 = 1234 };
                //context.Students2.UpdateEntry(updateData).Modified(a => a.Age1).Condition(a => a.Name == "name122" && a.Id == "9321e81a-6357-4a51-81fc-8e529a7c2c1d");
                //context.Students2.UpdateEntry(updateData).Modified(a => a.Id).Condition(a => string.Equals(a.Name, "name122"));
                context.Students2.UpdateByPrimary(updateData);

                var ret = context.Merge();
            }
        }
    }
}
