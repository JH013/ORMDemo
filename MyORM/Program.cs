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
            using (var context = new SchoolContext())
            {
                var data = context.Students2;


                //var result5 = data.Select(c => c).ToList();

                //var result1 = data.Select(b => new { Name = b.Name, Age = b.Age }).ToList();

                //var result13 = data.Select(b => b.Name).ToList();

                //var result2 = data.Select(b => new Student { Name = b.Name, Age = b.Age }).ToList();

                //var result3 = data.OrderBy(b => b.Name).ThenBy(b => b.Age).ThenBy(b => b.Age).ToList();

                //var result4 = data.GroupBy(a => new { Name = a.Name }).SelectMany(group => group).Select(c => c.Name).ToList();

                var result52 = data.GroupBy(a => new { Age2 = a.Age, Name2 = a.Name }).Select(c => c).ToList();

                //var result15 = data.Where(a => a.Name == "name2").ToList();

                //var result23 = data.OrderBy(a => a.Age).ThenBy(a => a.Name).ToList();

                //var result3 = data.OrderByDescending(a => a.Age).Where(a => a.Name == "name2" && a.Age >= 3).ToList();

                //var result1 = data.Where(a => a.Name == "name2" && a.Age >= 3).Select(b => new Student { Name = b.Name, Age = b.Age }).OrderBy(a => a.Age).ToList();

                //var result = data.Where(a => a.Name == "name2").Where(b => b.Age >= 3).Where(c => c.Id == "2").ToList();

                //var result = data.Where(a => a.Name == "name2").Where(b => b.Age >= 3 && b.Id == "2").ToList();
            }
        }
    }
}
