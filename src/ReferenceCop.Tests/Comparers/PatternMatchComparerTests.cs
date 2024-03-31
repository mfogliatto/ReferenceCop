namespace ReferenceCop.Tests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class PatternMatchComparerTests
    {
        [TestMethod]
        public void Equals_WhenYIsDefaultPattern_ReturnsTrue()
        {
            // Arrange.
            var comparer = new PatternMatchComparer();
            string x = "Reference";
            string y = "*";

            // Act.
            bool result = comparer.Equals(x, y);

            // Assert.
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Equals_WhenXIsDefaultPattern_ReturnsTrue()
        {
            // Arrange.
            var comparer = new PatternMatchComparer();
            string x = "*";
            string y = "Reference";

            // Act.
            bool result = comparer.Equals(x, y);

            // Assert.
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Equals_WhenXEqualsY_ReturnsTrue()
        {
            // Arrange.
            var comparer = new PatternMatchComparer();
            string x = "Reference";
            string y = "Reference";

            // Act.
            bool result = comparer.Equals(x, y);

            // Assert.
            result.Should().BeTrue();
        }

        [TestMethod]
        [DataRow("Reference.*", "Reference.Level1")]
        [DataRow("Reference.*", "Reference.Level1.Level2")]
        [DataRow("Reference*", "Reference")]
        public void Equals_WhenXContainsAPrefixOfY_ReturnsTrue(string x, string y)
        {
            // Arrange.
            var comparer = new PatternMatchComparer();

            // Act.
            bool result = comparer.Equals(x, y);

            // Assert.
            result.Should().BeTrue();
        }

        [TestMethod]
        [DataRow("*.Abstractions", "Reference.Abstractions")]
        [DataRow("*Abstractions", "ReferenceAbstractions")]
        public void Equals_WhenXContainsASuffixOfY_ReturnsTrue(string x, string y)
        {
            // Arrange.
            var comparer = new PatternMatchComparer();

            // Act.
            bool result = comparer.Equals(x, y);

            // Assert.
            result.Should().BeTrue();
        }

        [TestMethod]
        [DataRow("ReferenceA", "ReferenceB")]
        [DataRow("Reference.Level1.*", "Reference.Level2")]
        [DataRow("Reference.Level1.Level2.*", "Reference.Level1")]
        [DataRow("ReferenceA*", "Reference")]
        [DataRow("*ReferenceA", "Reference")]
        public void Equals_WhenXNotEqualsY_ReturnsFalse(string x, string y)
        {
            // Arrange.
            var comparer = new PatternMatchComparer();

            // Act.
            bool result = comparer.Equals(x, y);

            // Assert.
            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow("Reference.*", "Reference.Level1")]
        [DataRow("Reference.*", "Reference.Level1.Level2")]
        [DataRow("Reference*", "Reference")]
        public void Equals_WhenYContainsAPrefixOfX_ReturnsTrue(string x, string y)
        {
            // Arrange.
            var comparer = new PatternMatchComparer();

            // Act.
            bool result = comparer.Equals(y, x);

            // Assert.
            result.Should().BeTrue();
        }

        [TestMethod]
        [DataRow("*.Abstractions", "Reference.Abstractions")]
        [DataRow("*Abstractions", "ReferenceAbstractions")]
        public void Equals_WhenYContainsASuffixOfX_ReturnsTrue(string x, string y)
        {
            // Arrange.
            var comparer = new PatternMatchComparer();

            // Act.
            bool result = comparer.Equals(y, x);

            // Assert.
            result.Should().BeTrue();
        }

        [TestMethod]
        [DataRow("ReferenceA", "ReferenceB")]
        [DataRow("Reference.Level1.*", "Reference.Level2")]
        [DataRow("Reference.Level1.Level2.*", "Reference.Level1")]
        [DataRow("ReferenceA*", "Reference")]
        [DataRow("*ReferenceA", "Reference")]
        public void Equals_WhenYNotEqualsX_ReturnsFalse(string x, string y)
        {
            // Arrange.
            var comparer = new PatternMatchComparer();

            // Act.
            bool result = comparer.Equals(y, x);

            // Assert.
            result.Should().BeFalse();
        }

        [TestMethod]
        public void GetHashCode_ReturnsHashCodeOfObject()
        {
            // Arrange.
            var comparer = new PatternMatchComparer();
            string obj = "Reference";

            // Act.
            int result = comparer.GetHashCode(obj);

            // Assert.
            obj.GetHashCode().Should().Be(result);
        }
    }
}
