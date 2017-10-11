using LightZ.ComponentModel.Services;
using LightZ.ComponentModel.Services.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LightZ.Tests.ComponentModel.Services
{
    [TestClass]
    public class AudioServiceTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            TestUtilities.Initialize();
        }

        [TestMethod]
        public void BassAverage()
        {
            var service = ServiceLocator.GetService<AudioService>();

            Assert.IsTrue(service.GetAudioDevices().Count > 0);
            Assert.IsNull(service.CurrentAudioDevice);

            service.CurrentAudioDevice = service.GetAudioDevices().First();

            Assert.IsNotNull(service.GetBassAverage());

            service.Pause();

            Assert.IsNull(service.GetBassAverage());

            service.Resume();

            Assert.IsNotNull(service.GetBassAverage());
            Assert.IsNotNull(service.GetBassAverage());
            Assert.IsNotNull(service.GetBassAverage());
            Assert.IsNotNull(service.GetBassAverage());
            Assert.IsNotNull(service.GetBassAverage()); // If everything runs correctly, the service must be reseted at some point.
        }
    }
}
