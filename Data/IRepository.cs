using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContractWork.Data
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
    }
}
