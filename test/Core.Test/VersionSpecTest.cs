using NuGet.Versioning;
using Xunit;

namespace NuGet.Test
{

    public class VersionRangeTest
    {
        [Fact]
        public void ToStringExactVersion()
        {
            // Arrange
            var spec = new NuGetVersionRange(new NuGetVersion("1.0"));

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("[1.0]", value);
        }

        [Fact]
        public void ToStringlowerBoundInclusive()
        {
            // Arrange
            var spec = new NuGetVersionRange(new NuGetVersion("1.0"), true);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("1.0", value);
        }

        [Fact]
        public void ToStringlowerBoundExclusive()
        {
            // Arrange
            var spec = new NuGetVersionRange(new NuGetVersion("1.0"), false);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("(1.0, )", value);
        }

        [Fact]
        public void ToStringupperBoundInclusive()
        {
            // Arrange
            var spec = new NuGetVersionRange(null, false, new NuGetVersion("1.0"), true);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("(, 1.0]", value);
        }

        [Fact]
        public void ToStringupperBoundExclusive()
        {
            // Arrange
            var spec = new NuGetVersionRange(null, false, new NuGetVersion("1.0"), false);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("(, 1.0)", value);
        }

        [Fact]
        public void ToStringlowerBoundExclusiveMaxInclusive()
        {
            // Arrange
            var spec = new NuGetVersionRange(new NuGetVersion("1.0"), false, new NuGetVersion("3.0"), true);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("(1.0, 3.0]", value);
        }

        [Fact]
        public void ToStringlowerBoundInclusiveMaxExclusive()
        {
            // Arrange
            var spec = new NuGetVersionRange(new NuGetVersion("1.0"), true, new NuGetVersion("4.0"), false);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("[1.0, 4.0)", value);
        }

        [Fact]
        public void ToStringlowerBoundInclusiveMaxInclusive()
        {
            // Arrange
            var spec = new NuGetVersionRange(new NuGetVersion("1.0"), true, new NuGetVersion("5.0"), true);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("[1.0, 5.0]", value);
        }

        [Fact]
        public void ToStringlowerBoundExclusiveMaxExclusive()
        {
            // Arrange
            var spec = new NuGetVersionRange(new NuGetVersion("1.0"), false, new NuGetVersion("5.0"), false);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("(1.0, 5.0)", value);
        }
    }
}
