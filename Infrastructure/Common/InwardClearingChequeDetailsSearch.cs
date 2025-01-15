using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Common
{
    public class InwardClearingChequeDetailsSearch
    {
        readonly string _chequeImageLinkedKey;

        public InwardClearingChequeDetailsSearch(string chequeImageLinkedKey)
        {
            _chequeImageLinkedKey = chequeImageLinkedKey;
        }

        public bool StartsWith(InwardClearingChequeDetailsModel e)
        {
            if (e.ChequeImageLinkedKey == null)
                return false;

            return e.ChequeImageLinkedKey.StartsWith(_chequeImageLinkedKey, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
