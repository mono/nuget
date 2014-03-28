using Xunit;

namespace NuGet.Test
{

    public class PackageDependencyTest
    {
        [Fact]
        public void ToStringExactVersion()
        {
            // Arrange
            PackageDependency dependency = PackageDependency.CreateDependency("A", "[1.0]");

            // Act
            string value = dependency.ToString();

            // Assert
            Assert.Equal("A (= 1.0)", value);
        }

        [Fact]
        public void ToStringlowerBoundInclusive()
        {
            // Arrange
            PackageDependency dependency = PackageDependency.CreateDependency("A", "1.0");

            // Act
            string value = dependency.ToString();

            // Assert
            Assert.Equal("A (\u2265 1.0)", value);
        }

        [Fact]
        public void ToStringlowerBoundExclusive()
        {
            // Arrange
            PackageDependency dependency = PackageDependency.CreateDependency("A", "(1.0,)");

            // Act
            string value = dependency.ToString();

            // Assert
            Assert.Equal("A (> 1.0)", value);
        }

        [Fact]
        public void ToStringupperBoundInclusive()
        {
            // Arrange
            PackageDependency dependency = PackageDependency.CreateDependency("A", "[,1.0]");

            // Act
            string value = dependency.ToString();

            // Assert
            Assert.Equal("A (\u2264 1.0)", value);
        }

        [Fact]
        public void ToStringupperBoundExclusive()
        {
            // Arrange
            PackageDependency dependency = PackageDependency.CreateDependency("A", "[,1.0)");

            // Act
            string value = dependency.ToString();

            // Assert
            Assert.Equal("A (< 1.0)", value);
        }

        [Fact]
        public void ToStringlowerBoundExclusiveMaxInclusive()
        {
            // Arrange
            PackageDependency dependency = PackageDependency.CreateDependency("A", "(1.0,5.0]");

            // Act
            string value = dependency.ToString();

            // Assert
            Assert.Equal("A (> 1.0 && \u2264 5.0)", value);
        }

        [Fact]
        public void ToStringlowerBoundInclusiveMaxExclusive()
        {
            // Arrange
            PackageDependency dependency = PackageDependency.CreateDependency("A", "[1.0,5.0)");

            // Act
            string value = dependency.ToString();

            // Assert
            Assert.Equal("A (\u2265 1.0 && < 5.0)", value);
        }

        [Fact]
        public void ToStringlowerBoundInclusiveMaxInclusive()
        {
            // Arrange
            PackageDependency dependency = PackageDependency.CreateDependency("A", "[1.0,5.0]");

            // Act
            string value = dependency.ToString();

            // Assert
            Assert.Equal("A (\u2265 1.0 && \u2264 5.0)", value);
        }

        [Fact]
        public void ToStringlowerBoundExclusiveMaxExclusive()
        {
            // Arrange
            PackageDependency dependency = PackageDependency.CreateDependency("A", "(1.0,5.0)");

            // Act
            string value = dependency.ToString();

            // Assert
            Assert.Equal("A (> 1.0 && < 5.0)", value);
        }
    }
}
