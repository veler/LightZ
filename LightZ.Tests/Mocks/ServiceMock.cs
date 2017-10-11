using LightZ.ComponentModel.Services.Base;

namespace LightZ.Tests.Mocks
{
    class ServiceMock : IService
    {
        public bool Reseted = true;

        public void Initialize()
        {
            Reseted = false;
        }

        public void Reset()
        {
            Reseted = true;
        }
    }
}
