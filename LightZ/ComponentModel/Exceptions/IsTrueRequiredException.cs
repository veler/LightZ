using System;

namespace LightZ.ComponentModel.Exceptions
{
    internal sealed class IsTrueRequiredException : Exception
    {
        public IsTrueRequiredException()
            : base("The value must be true")
        {
        }
    }
}
