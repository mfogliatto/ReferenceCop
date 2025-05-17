namespace ReferenceCop.Tests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExactMatchComparerTests
    {
        [TestMethod]
        public void Equals_WhenXEqualsY_ReturnsTrue()
        {
            // Arrange.
            var comparer = new ExactMatchComparer();
            string x = "Reference";
            string y = "Reference";

            // Act.
            bool result = comparer.Equals(x, y);

            // Assert.
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Equals_WhenXIsNotEqualsY_ReturnsFalse()
        {
            // Arrange.
            var comparer = new ExactMatchComparer();
            string x = "ReferenceA";
            string y = "ReferenceB";

            // Act.
            bool result = comparer.Equals(x, y);

            // Assert.
            result.Should().BeFalse();
        }

        [TestMethod]
        public void GetHashCode_ReturnsHashCodeOfObject()
        {
            // Arrange.
            var comparer = new ExactMatchComparer();
            string obj = "Reference";

            // Act.
            int result = comparer.GetHashCode(obj);

            // Assert.
            result.Should().Be(obj.GetHashCode());
        }
    }
}
