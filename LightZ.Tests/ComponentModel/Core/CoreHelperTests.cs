using LightZ.ComponentModel.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightZ.Tests.ComponentModel.Core
{
    [TestClass]
    public class CoreHelperTests
    {
        [TestMethod]
        public void GetApplicationName()
        {
            var hash = CoreHelper.GetApplicationName();

            Assert.AreEqual(hash, "UnitTestApp");
        }
    }
}
