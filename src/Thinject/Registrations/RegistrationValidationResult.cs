using System.Collections.Generic;

namespace Thinject
{
    internal class RegistrationValidationResult
    {
        private readonly IList<string> _errors = new List<string>();

        public IEnumerable<string> Errors
        {
            get { return _errors; }
        }

        public bool IsValid
        {
            get { return _errors.Count == 0; }
        }

        public void AddError(string format, params object[] args)
        {
            _errors.Add(string.Format(format, args));
        }
    }
}