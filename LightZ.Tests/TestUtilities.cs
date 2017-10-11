using LightZ.ComponentModel.Services.Base;

namespace LightZ.Tests
{
    internal static class TestUtilities
    {
        private static bool _initialized;

        internal static void Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;
            }

            ServiceLocator.ResetAll();
        }
    }
}
