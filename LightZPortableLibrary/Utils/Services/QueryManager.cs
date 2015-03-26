namespace LightZPortableLibrary.Utils.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using LightZPortableLibrary.Enums;
    using LightZPortableLibrary.Model;

    /// <summary>
    /// Generate queries to send to the Arduino
    /// </summary>
    public class QueryManager
    {
        #region Methods

        /// <summary>
        /// Generates a query to ask the Arduino to switch modes
        /// </summary>
        /// <param name="mode">The desired mode</param>
        /// <returns>The generated query</returns>
        public static byte[] GenerateModeQuery(Mode mode)
        {
            return new[] { (byte)Target.Mode, (byte)mode, (byte)1, (byte)1 };
        }

        /// <summary>
        /// Generates a query to ask the Arduino to change the overall contrast of the led strip
        /// </summary>
        /// <param name="brightness">The desired contrast</param>
        /// <returns>The generated query</returns>
        public static byte[] GenerateBrightnessQuery(byte brightness)
        {
            return new[] { (byte)Target.Brightness, brightness, (byte)1, (byte)1 };
        }

        /// <summary>
        /// Generates a query to ask the Arduino to change the led strip based on sound detected
        /// </summary>
        /// <param name="levels">The left and right sound levels</param>
        /// <returns>The generated query</returns>
        public static byte[] GenerateSoundQuery(Dictionary<string, byte> levels)
        {
            return new[] { (byte)Target.Audio, levels["left"], levels["right"], (byte)1 };
        }

        /// <summary>
        /// Generates a query to ask the Arduino to change the color of an LED
        /// </summary>
        /// <param name="led">LED with desired color</param>
        /// <returns>The generated query</returns>
        public static byte[] GenerateLedQuery(Led led)
        {
            if (led.Color == null)
                led.Color = new Color(0, 0, 0);
            return new[] { (byte)led.LedIndex, led.Color.Red, led.Color.Green, led.Color.Blue };
        }

        /// <summary>
        /// Generates a query to ask the Arduino to change the color of several LEDs
        /// </summary>
        /// <param name="leds">The list of desired colors with LED</param>
        /// <returns>The generated query</returns>
        public static byte[] GenerateLedQuery(List<Led> leds)
        {
            IEnumerable<byte> result = new List<byte>();

            foreach (var led in leds)
                result = result.Concat(GenerateLedQuery(led));

            return result.ToArray();
        }

        #endregion
    }
}