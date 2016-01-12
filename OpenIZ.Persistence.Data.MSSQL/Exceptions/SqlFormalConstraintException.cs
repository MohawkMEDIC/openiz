using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Exceptions
{

    /// <summary>
    /// SQL Formal Constraint types
    /// </summary>
    public enum SqlFormalConstraintType
    {
        /// <summary>
        /// Indicates the identity was on an insert operation
        /// </summary>
        IdentityInsert,
        /// <summary>
        /// Indicates the identity was not present on an update or obsolete
        /// </summary>
        NonIdentityUpdate
    }
    /// <summary>
    /// Represents a violation of a formal constraint
    /// </summary>
    public class SqlFormalConstraintException : ConstraintException
    {

        // The type of constraint violation
        private SqlFormalConstraintType m_violation;

        /// <summary>
        /// Formal constraint message
        /// </summary>
        public SqlFormalConstraintException(SqlFormalConstraintType violation) : base()
        {
            this.m_violation = violation;
        }

        /// <summary>
        /// Gets the violation type
        /// </summary>
        public SqlFormalConstraintType ViolationType
        {
            get { return this.m_violation; }
        }

        /// <summary>
        /// Gets the message of the exception
        /// </summary>
        public override string Message
        {
            get
            {
                ILocalizationService locale = ApplicationContext.Current.GetService<ILocalizationService>();
                if (locale == null)
                    return this.m_violation.ToString();
                else switch (this.m_violation)
                    {
                        case SqlFormalConstraintType.IdentityInsert:
                            return locale.GetString("DBCE001");
                        case SqlFormalConstraintType.NonIdentityUpdate:
                            return locale.GetString("DBCE002");
                        default:
                            return this.m_violation.ToString();
                    }
            }
        }
    }
}
