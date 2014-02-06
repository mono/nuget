using NuGet.Versioning;
using Xunit;

namespace NuGet.Test
{

    public class VersionSpecTest
    {
        [Fact]
        public void ToStringExactVersion()
        {
            // Arrange
            var spec = new VersionSpec(new NuGetVersion("1.0"));

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("[1.0]", value);
        }

        [Fact]
        public void ToStringMinVersionInclusive()
        {
            // Arrange
            var spec = new VersionSpec(new NuGetVersion("1.0"), true);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("1.0", value);
        }

        [Fact]
        public void ToStringMinVersionExclusive()
        {
            // Arrange
            var spec = new VersionSpec(new NuGetVersion("1.0"), false);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("(1.0, )", value);
        }

        [Fact]
        public void ToStringMaxVersionInclusive()
        {
            // Arrange
            var spec = new VersionSpec(new NuGetVersion("1.0"), true);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("(, 1.0]", value);
        }

        [Fact]
        public void ToStringMaxVersionExclusive()
        {
            // Arrange
            var spec = new VersionSpec(new NuGetVersion("1.0"), false);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("(, 1.0)", value);
        }

        [Fact]
        public void ToStringMinVersionExclusiveMaxInclusive()
        {
            // Arrange
            var spec = new VersionSpec(new NuGetVersion("1.0"), new NuGetVersion("3.0"), false, true);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("(1.0, 3.0]", value);
        }

        [Fact]
        public void ToStringMinVersionInclusiveMaxExclusive()
        {
            // Arrange
            var spec = new VersionSpec(new NuGetVersion("1.0"), new NuGetVersion("4.0"), true, false);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("[1.0, 4.0)", value);
        }

        [Fact]
        public void ToStringMinVersionInclusiveMaxInclusive()
        {
            // Arrange
            var spec = new VersionSpec(new NuGetVersion("1.0"), new NuGetVersion("5.0"), true, false);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("[1.0, 5.0]", value);
        }

        [Fact]
        public void ToStringMinVersionExclusiveMaxExclusive()
        {
            // Arrange
            var spec = new VersionSpec(new NuGetVersion("1.0"), new NuGetVersion("5.0"), false, false);

            // Act
            string value = spec.ToString();

            // Assert
            Assert.Equal("(1.0, 5.0)", value);
        }
    }
}
