using LightZ.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightZ.Tests.ComponentModel.Services.Base
{
    [TestClass]
    public class ServiceLocatorTests
    {
        [TestMethod]
        public void ServiceLocator()
        {
            var service1 = LightZ.ComponentModel.Services.Base.ServiceLocator.GetService<ServiceMock>();
            var service2 = LightZ.ComponentModel.Services.Base.ServiceLocator.GetService<ServiceMock>();

            Assert.AreSame(service1, service2);
            Assert.IsFalse(service1.Reseted);

            LightZ.ComponentModel.Services.Base.ServiceLocator.ResetAll();

            Assert.IsTrue(service1.Reseted);
        }
    }
}
