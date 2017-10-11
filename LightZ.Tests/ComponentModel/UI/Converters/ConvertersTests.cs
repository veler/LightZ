using Microsoft.VisualStudio.TestTools.UnitTesting;
using LightZ.ComponentModel.UI.Converters;
using System.Globalization;
using LightZ.ComponentModel.Enums;
using System.Windows;

namespace LightZ.Tests.ComponentModel.UI.Converters
{
    [TestClass]
    public class ConvertersTests
    {
        [TestMethod]
        public void EnumDescriptionConverter()
        {
            var converter = new EnumDescriptionConverter();

            Assert.AreEqual("Manual full color", converter.Convert(LedStripMode.Manual, typeof(LedStripMode), null, CultureInfo.CurrentCulture));
            Assert.AreEqual("LeftButtonPressed", converter.Convert(MouseAction.LeftButtonPressed, typeof(MouseAction), null, CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void BooleanToVerticalAlignmentConverter()
        {
            var converter = new BooleanToVerticalAlignmentConverter();
            converter.True = VerticalAlignment.Bottom;
            converter.False = VerticalAlignment.Top;
            Assert.AreEqual(VerticalAlignment.Bottom, converter.Convert(true, typeof(ConvertersTests), null, CultureInfo.CurrentCulture));
            Assert.AreEqual(VerticalAlignment.Top, converter.Convert(false, typeof(ConvertersTests), null, CultureInfo.CurrentCulture));
        }
    }
}
