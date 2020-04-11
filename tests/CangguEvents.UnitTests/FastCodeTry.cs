using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CangguEvents.UnitTests
{
    public class FastCodeTry
    {
        private readonly ITestOutputHelper _output;

        public FastCodeTry(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Foo()
        {
            Enumerable.Range(0, 100)
                .Select(FizzBuzz)
                .ToList()
                .ForEach(_output.WriteLine);
        }

        private static string FizzBuzz(int x) => (x % 3, x % 5) switch
        {
            (0, 0) => "FizzBuzz",
            (0, _) => "Fizz",
            (_, 0) => "Buzz",
            _ => x.ToString()
        };

        [Fact]
        public void BasicTests()
        {
            SplitString.Solution("abc").Should().BeEquivalentTo("ab", "c_");
            SplitString.Solution("abcdef").Should().BeEquivalentTo("ab", "cd", "ef");
        }
    }

    public class SplitString
    {
        public static string[] Solution(string str)
        {
            var letterLength = 2;
            str = str.Length % 2 == 0 ? str : str + "_";

            return Enumerable.Range(0, str.Length / letterLength)
                .Select(index => str.Substring(letterLength * index, letterLength))
                .ToArray();
        }
    }
}