using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Common.Exceptions
{
    public class ApplicationValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public ApplicationValidationException(IEnumerable<string> errors)
            : base("One or more validation failures have occurred.")
        {
            Errors = errors;
        }
    }
}
