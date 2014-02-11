using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace NuGet.Test
{
    public class VersionSpecTests
    {

        [Theory]
        [PropertyData("VersionSpecNotInRange")]
        public void ParseVersionSpecDoesNotSatisfy(string spec, string version)
        {
            // Act
            var versionInfo = VersionSpec.Parse(spec);
            var middleVersion = NuGetVersion.Parse(version);

            // Assert
            Assert.False(versionInfo.Satisfies(middleVersion));
            Assert.False(versionInfo.Satisfies(middleVersion, VersionComparison.Default));
            Assert.False(versionInfo.Satisfies(middleVersion, VersionComparer.Default));
            Assert.False(versionInfo.Satisfies(middleVersion, VersionComparison.IgnoreMetadata));
            Assert.False(versionInfo.Satisfies(middleVersion, VersionComparer.IgnoreMetadata));
            Assert.False(versionInfo.Satisfies(middleVersion, VersionComparison.Strict));
            Assert.False(versionInfo.Satisfies(middleVersion, VersionComparer.Strict));
        }

        [Theory]
        [PropertyData("VersionSpecInRange")]
        public void ParseVersionSpecSatisfies(string spec, string version)
        {
            // Act
            var versionInfo = VersionSpec.Parse(spec);
            var middleVersion = NuGetVersion.Parse(version);

            // Assert
            Assert.True(versionInfo.Satisfies(middleVersion));
            Assert.True(versionInfo.Satisfies(middleVersion, VersionComparison.Default));
            Assert.True(versionInfo.Satisfies(middleVersion, VersionComparer.Default));
            Assert.True(versionInfo.Satisfies(middleVersion, VersionComparison.IgnoreMetadata));
            Assert.True(versionInfo.Satisfies(middleVersion, VersionComparer.IgnoreMetadata));
            Assert.True(versionInfo.Satisfies(middleVersion, VersionComparison.Strict));
            Assert.True(versionInfo.Satisfies(middleVersion, VersionComparer.Strict));
        }

        [Theory]
        [PropertyData("VersionSpecParts")]
        public void ParseVersionSpecParts(ISemanticVersion min, ISemanticVersion max, bool minInc, bool maxInc)
        {
            // Act
            var versionInfo = new VersionSpec(min, max, minInc, maxInc);

            // Assert
            Assert.Equal(min, versionInfo.MinVersion, VersionComparer.Default);
            Assert.Equal(max, versionInfo.MaxVersion, VersionComparer.Default);
            Assert.Equal(minInc, versionInfo.IsMinInclusive);
            Assert.Equal(maxInc, versionInfo.IsMaxInclusive);
        }

        [Theory]
        [PropertyData("VersionSpecParts")]
        public void ParseVersionSpecToStringReParse(ISemanticVersion min, ISemanticVersion max, bool minInc, bool maxInc)
        {
            // Act
            var original = new VersionSpec(min, max, minInc, maxInc);
            var versionInfo = VersionSpec.Parse(original.ToString());

            // Assert
            Assert.Equal(min, versionInfo.MinVersion, VersionComparer.Default);
            Assert.Equal(max, versionInfo.MaxVersion, VersionComparer.Default);
            Assert.Equal(minInc, versionInfo.IsMinInclusive);
            Assert.Equal(maxInc, versionInfo.IsMaxInclusive);
        }

        [Theory]
        [PropertyData("VersionSpecParts")]
        public void ParseVersionSpecNormalizeReParse(ISemanticVersion min, ISemanticVersion max, bool minInc, bool maxInc)
        {
            // Act
            var original = new VersionSpec(min, max, minInc, maxInc);
            var versionInfo = VersionSpec.Parse(original.ToNormalizedString());

            // Assert
            Assert.Equal(min, versionInfo.MinVersion, VersionComparer.Default);
            Assert.Equal(max, versionInfo.MaxVersion, VersionComparer.Default);
            Assert.Equal(minInc, versionInfo.IsMinInclusive);
            Assert.Equal(maxInc, versionInfo.IsMaxInclusive);
        }

        [Theory]
        [PropertyData("VersionSpecStrings")]
        public void ParseVersionSpecToString(string version)
        {
            // Act
            var versionInfo = VersionSpec.Parse(version);

            // Assert
            Assert.Equal(version, versionInfo.ToString());
        }

        [Fact]
        public void ParseVersionSpecWithNullThrows()
        {
            // Act & Assert
            ExceptionAssert.ThrowsArgNull(() => VersionSpec.Parse(null), "value");
        }

        [Fact]
        public void ParseVersionSpecSimpleVersionNoBrackets()
        {
            // Act
            var versionInfo = VersionSpec.Parse("1.2");

            // Assert
            Assert.Equal("1.2", versionInfo.MinVersion.ToString());
            Assert.True(versionInfo.IsMinInclusive);
            Assert.Equal(null, versionInfo.MaxVersion);
            Assert.False(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecSimpleVersionNoBracketsExtraSpaces()
        {
            // Act
            var versionInfo = VersionSpec.Parse("  1  .   2  ");

            // Assert
            Assert.Equal("1.2", versionInfo.MinVersion.ToString());
            Assert.True(versionInfo.IsMinInclusive);
            Assert.Equal(null, versionInfo.MaxVersion);
            Assert.False(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecMaxOnlyInclusive()
        {
            // Act
            var versionInfo = VersionSpec.Parse("(,1.2]");

            // Assert
            Assert.Equal(null, versionInfo.MinVersion);
            Assert.False(versionInfo.IsMinInclusive);
            Assert.Equal("1.2", versionInfo.MaxVersion.ToString());
            Assert.True(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecMaxOnlyExclusive()
        {
            var versionInfo = VersionSpec.Parse("(,1.2)");
            Assert.Equal(null, versionInfo.MinVersion);
            Assert.False(versionInfo.IsMinInclusive);
            Assert.Equal("1.2", versionInfo.MaxVersion.ToString());
            Assert.False(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecExactVersion()
        {
            // Act
            var versionInfo = VersionSpec.Parse("[1.2]");

            // Assert
            Assert.Equal("1.2", versionInfo.MinVersion.ToString());
            Assert.True(versionInfo.IsMinInclusive);
            Assert.Equal("1.2", versionInfo.MaxVersion.ToString());
            Assert.True(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecMinOnlyExclusive()
        {
            // Act
            var versionInfo = VersionSpec.Parse("(1.2,)");

            // Assert
            Assert.Equal("1.2", versionInfo.MinVersion.ToString());
            Assert.False(versionInfo.IsMinInclusive);
            Assert.Equal(null, versionInfo.MaxVersion);
            Assert.False(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeExclusiveExclusive()
        {
            // Act
            var versionInfo = VersionSpec.Parse("(1.2,2.3)");

            // Assert
            Assert.Equal("1.2", versionInfo.MinVersion.ToString());
            Assert.False(versionInfo.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.MaxVersion.ToString());
            Assert.False(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeExclusiveInclusive()
        {
            // Act
            var versionInfo = VersionSpec.Parse("(1.2,2.3]");

            // Assert
            Assert.Equal("1.2", versionInfo.MinVersion.ToString());
            Assert.False(versionInfo.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.MaxVersion.ToString());
            Assert.True(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeInclusiveExclusive()
        {
            // Act
            var versionInfo = VersionSpec.Parse("[1.2,2.3)");
            Assert.Equal("1.2", versionInfo.MinVersion.ToString());
            Assert.True(versionInfo.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.MaxVersion.ToString());
            Assert.False(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeInclusiveInclusive()
        {
            // Act
            var versionInfo = VersionSpec.Parse("[1.2,2.3]");

            // Assert
            Assert.Equal("1.2", versionInfo.MinVersion.ToString());
            Assert.True(versionInfo.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.MaxVersion.ToString());
            Assert.True(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeInclusiveInclusiveExtraSpaces()
        {
            // Act
            var versionInfo = VersionSpec.Parse("   [  1 .2   , 2  .3   ]  ");

            // Assert
            Assert.Equal("1.2", versionInfo.MinVersion.ToString());
            Assert.True(versionInfo.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.MaxVersion.ToString());
            Assert.True(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeIntegerRanges()
        {
            // Act
            var versionInfo = VersionSpec.Parse("   [1, 2]  ");

            // Assert
            Assert.Equal("1.0", versionInfo.MinVersion.ToString());
            Assert.True(versionInfo.IsMinInclusive);
            Assert.Equal("2.0", versionInfo.MaxVersion.ToString());
            Assert.True(versionInfo.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeNegativeIntegerRanges()
        {
            // Act
            IVersionSpec versionInfo;
            bool parsed = VersionSpec.TryParse("   [-1, 2]  ", out versionInfo);

            Assert.False(parsed);
            Assert.Null(versionInfo);
        }

        [Fact]
        public void ParseVersionThrowsIfExclusiveMinAndMaxVersionSpecContainsNoValues()
        {
            // Arrange
            var versionString = "(,)";

            // Assert
            var exception = Assert.Throws<ArgumentException>(() => VersionSpec.Parse(versionString));
            Assert.Equal("'(,)' is not a valid version string.", exception.Message);
        }

        [Fact]
        public void ParseVersionThrowsIfInclusiveMinAndMaxVersionSpecContainsNoValues()
        {
            // Arrange
            var versionString = "[,]";

            // Assert
            var exception = Assert.Throws<ArgumentException>(() => VersionSpec.Parse(versionString));
            Assert.Equal("'[,]' is not a valid version string.", exception.Message);
        }

        [Fact]
        public void ParseVersionThrowsIfInclusiveMinAndExclusiveMaxVersionSpecContainsNoValues()
        {
            // Arrange
            var versionString = "[,)";

            // Assert
            var exception = Assert.Throws<ArgumentException>(() => VersionSpec.Parse(versionString));
            Assert.Equal("'[,)' is not a valid version string.", exception.Message);
        }

        [Fact]
        public void ParseVersionThrowsIfExclusiveMinAndInclusiveMaxVersionSpecContainsNoValues()
        {
            // Arrange
            var versionString = "(,]";

            // Assert
            var exception = Assert.Throws<ArgumentException>(() => VersionSpec.Parse(versionString));
            Assert.Equal("'(,]' is not a valid version string.", exception.Message);
        }

        [Fact]
        public void ParseVersionThrowsIfVersionSpecIsMissingVersionComponent()
        {
            // Arrange
            var versionString = "(,1.3..2]";

            // Assert
            var exception = Assert.Throws<ArgumentException>(() => VersionSpec.Parse(versionString));
            Assert.Equal("'(,1.3..2]' is not a valid version string.", exception.Message);
        }

        [Fact]
        public void ParseVersionThrowsIfVersionSpecContainsMoreThen4VersionComponents()
        {
            // Arrange
            var versionString = "(1.2.3.4.5,1.2]";

            // Assert
            var exception = Assert.Throws<ArgumentException>(() => VersionSpec.Parse(versionString));
            Assert.Equal("'(1.2.3.4.5,1.2]' is not a valid version string.", exception.Message);
        }

        [Theory]
        [PropertyData("VersionSpecData")]
        public void ParseVersionParsesTokensVersionsCorrectly(string versionString, VersionSpec versionSpec)
        {
            // Act
            var actual = VersionSpec.Parse(versionString);

            // Assert
            Assert.Equal(versionSpec.IsMinInclusive, actual.IsMinInclusive);
            Assert.Equal(versionSpec.IsMaxInclusive, actual.IsMaxInclusive);
            Assert.Equal(versionSpec.MinVersion, actual.MinVersion);
            Assert.Equal(versionSpec.MaxVersion, actual.MaxVersion);
        }

        public static IEnumerable<object[]> VersionSpecData
        {
            get
            {
                yield return new object[] { "(1.2.3.4, 3.2)", new VersionSpec(new NuGetVersion("1.2.3.4"), new NuGetVersion("3.2"), false, false) };
                yield return new object[] { "(1.2.3.4, 3.2]", new VersionSpec(new NuGetVersion("1.2.3.4"), new NuGetVersion("3.2"), false, true) };
                yield return new object[] { "[1.2, 3.2.5)", new VersionSpec(new NuGetVersion("1.2"), new NuGetVersion("3.2.5"), true, false) };
                yield return new object[] { "[2.3.7, 3.2.4.5]", new VersionSpec(new NuGetVersion("2.3.7"), new NuGetVersion("3.2.4.5"), true, true) };
                yield return new object[] { "(, 3.2.4.5]", new VersionSpec(null, new NuGetVersion("3.2.4.5"), false, true) };
                yield return new object[] { "(1.6, ]", new VersionSpec(new NuGetVersion("1.6"), null, false, true) };
                yield return new object[] { "(1.6)", new VersionSpec(new NuGetVersion("1.6"), new NuGetVersion("1.6"), false, false) };
                yield return new object[] { "[2.7]", new VersionSpec(new NuGetVersion("2.7"), new NuGetVersion("2.7"), true, true) };
            }
        }

        public static IEnumerable<object[]> VersionSpecStrings
        {
            get
            {
                yield return new object[] { "1.2" };
                yield return new object[] { "1.2.3" };
                yield return new object[] { "1.2.3-beta" };
                yield return new object[] { "1.2.3-beta+900" };
                yield return new object[] { "1.2.3-beta.2.4.55.X+900" };
                yield return new object[] { "1.2.3-0+900" };
                yield return new object[] { "[1.2]" };
                yield return new object[] { "[1.2.3]" };
                yield return new object[] { "[1.2.3-beta]" };
                yield return new object[] { "[1.2.3-beta+900]" };
                yield return new object[] { "[1.2.3-beta.2.4.55.X+900]" };
                yield return new object[] { "[1.2.3-0+900]" };
                yield return new object[] { "(, 1.2]" };
                yield return new object[] { "(, 1.2.3]" };
                yield return new object[] { "(, 1.2.3-beta]" };
                yield return new object[] { "(, 1.2.3-beta+900]" };
                yield return new object[] { "(, 1.2.3-beta.2.4.55.X+900]" };
                yield return new object[] { "(, 1.2.3-0+900]" };
                yield return new object[] { "(, ]" };
                yield return new object[] { "(, )" };
                yield return new object[] { "[, )" };
            }
        }

        public static IEnumerable<object[]> VersionSpecParts
        {
            get
            {
                yield return new object[] { SemanticVersionStrict.Parse("1.0.0"), SemanticVersionStrict.Parse("2.0.0"), true, true };
                yield return new object[] { SemanticVersionStrict.Parse("1.0.0"), SemanticVersionStrict.Parse("1.0.1"), false, false };
                yield return new object[] { SemanticVersionStrict.Parse("1.0.0-beta+0"), SemanticVersionStrict.Parse("2.0.0"), false, true };
                yield return new object[] { SemanticVersionStrict.Parse("1.0.0-beta+0"), SemanticVersionStrict.Parse("2.0.0+99"), false, false };
                yield return new object[] { SemanticVersionStrict.Parse("1.0.0-beta+0"), SemanticVersionStrict.Parse("2.0.0+99"), true, true };
                yield return new object[] { SemanticVersionStrict.Parse("1.0.0"), SemanticVersionStrict.Parse("2.0.0+99"), true, true };
            }
        }

        public static IEnumerable<object[]> VersionSpecInRange
        {
            get
            {
                yield return new object[] { "1.0.0", "2.0.0" };
                yield return new object[] { "[1.0.0, 2.0.0]", "2.0.0" };
                yield return new object[] { "[1.0.0, 2.0.0]", "1.0.0" };
                yield return new object[] { "[1.0.0, 2.0.0]", "2.0.0" };
                yield return new object[] { "[1.0.0-beta+meta, 2.0.0-beta+meta]", "1.0.0" };
                yield return new object[] { "[1.0.0-beta+meta, 2.0.0-beta+meta]", "1.0.0-beta+meta" };
                yield return new object[] { "[1.0.0-beta+meta, 2.0.0-beta+meta]", "2.0.0-beta" };
                yield return new object[] { "[1.0.0-beta+meta, 2.0.0-beta+meta]", "1.0.0+meta" };
                yield return new object[] { "(1.0.0-beta+meta, 2.0.0-beta+meta)", "1.0.0" };
                yield return new object[] { "(1.0.0-beta+meta, 2.0.0-beta+meta)", "2.0.0-alpha+meta" };
                yield return new object[] { "(1.0.0-beta+meta, 2.0.0-beta+meta)", "2.0.0-alpha" };
                yield return new object[] { "(, 2.0.0-beta+meta)", "2.0.0-alpha" };
            }
        }

        public static IEnumerable<object[]> VersionSpecNotInRange
        {
            get
            {
                yield return new object[] { "1.0.0", "0.0.0" };
                yield return new object[] { "[1.0.0, 2.0.0]", "2.0.1" };
                yield return new object[] { "[1.0.0, 2.0.0]", "0.0.0" };
                yield return new object[] { "[1.0.0, 2.0.0]", "3.0.0" };
                yield return new object[] { "[1.0.0-beta+meta, 2.0.0-beta+meta]", "1.0.0-alpha" };
                yield return new object[] { "[1.0.0-beta+meta, 2.0.0-beta+meta]", "1.0.0-alpha+meta" };
                yield return new object[] { "[1.0.0-beta+meta, 2.0.0-beta+meta]", "2.0.0-rc" };
                yield return new object[] { "[1.0.0-beta+meta, 2.0.0-beta+meta]", "2.0.0+meta" };
                yield return new object[] { "(1.0.0-beta+meta, 2.0.0-beta+meta)", "2.0.0-beta+meta" };
                yield return new object[] { "(, 2.0.0-beta+meta)", "2.0.0-beta+meta" };
            }
        }
    }
}
