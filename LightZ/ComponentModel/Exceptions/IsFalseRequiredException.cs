using System;

namespace LightZ.ComponentModel.Exceptions
{
    internal sealed class IsFalseRequiredException : Exception
    {
        public IsFalseRequiredException()
            : base("The value must be false")
        {
        }
    }
}
