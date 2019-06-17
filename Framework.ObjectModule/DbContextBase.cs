using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ObjectModule
{
    public class DbContextBase : IDisposable
    {
        public DbContextBase(string db)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
