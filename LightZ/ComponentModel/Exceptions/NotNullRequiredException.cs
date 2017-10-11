using System;

namespace LightZ.ComponentModel.Exceptions
{
    internal sealed class NotNullRequiredException : ArgumentNullException
    {
        public NotNullRequiredException(string parameterName)
            : base(parameterName, "The value must not be null")
        {
        }
    }
}
