using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContractWork.Data
{
    public class Repository : IRepository<Word>
    {
        private readonly Context db;

        public Repository(Context context)
        {
            db = context;
        }

        IEnumerable<Word> IRepository<Word>.GetAll()
        {
            return db.Words.ToList();
        }
    }
}
