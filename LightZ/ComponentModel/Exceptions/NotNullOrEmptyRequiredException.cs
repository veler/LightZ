using System;

namespace LightZ.ComponentModel.Exceptions
{
    internal sealed class NotNullOrEmptyRequiredException : ArgumentException
    {
        public NotNullOrEmptyRequiredException(string parameterName)
            : base(parameterName, "The value must not be null or empty")
        {
        }
    }
}
