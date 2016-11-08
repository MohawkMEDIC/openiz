using Jint.Native;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.BusinessRules.JavaScript.JNI
{
    /// <summary>
    /// Represents business rules bridge
    /// </summary>
    public class BusinessRulesBridge
    {

        /// <summary>
        /// Add a business rule for the specified object
        /// </summary>
        public void AddBusinessRule(String target, String trigger, Func<Object, Object> _delegate)
        {
            JavascriptBusinessRulesEngine.Current.RegisterRule(target, trigger, _delegate);
        }

        /// <summary>
        /// Adds validator
        /// </summary>
        public void AddValidator(String target, Func<Object, Object[]> _delegate)
        {
            JavascriptBusinessRulesEngine.Current.RegisterValidator(target, _delegate);
        }

        /// <summary>
        /// Create detected issue
        /// </summary>
        public DetectedIssue CreateDetectedIssue(String message, int priority)
        {
            return new DetectedIssue() { Text = message, Priority = (DetectedIssuePriorityType)priority };
        }
    }
}
