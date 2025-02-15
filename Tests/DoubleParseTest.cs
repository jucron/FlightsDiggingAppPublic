
using System.Globalization;
using System.Text.Json;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Services;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tests
{
    public class DoubleParseTest
    {
        public DoubleParseTest(ITestOutputHelper output)
        {
            
        }

        [Fact]
        public void TestDoubleParse()
        {
            string numberString = "123.45";
            double result = double.Parse(numberString, CultureInfo.InvariantCulture);
            Console.WriteLine(result); // Output: 123.45

            double expectedResult = 123.45;
            Assert.Equal(expectedResult,result);
        }
    }
}