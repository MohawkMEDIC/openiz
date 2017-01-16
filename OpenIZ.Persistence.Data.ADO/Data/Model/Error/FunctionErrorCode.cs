using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Model.Error
{
    /// <summary>
    /// This class exists to be able to extract the function error code for functions which cannot raise an exception because
    /// they the transaction manager will rollback
    /// </summary>
    public class FunctionErrorCode
    {

        [Column("err_code")]
        public String ErrorCode { get; set; }
    }
}
