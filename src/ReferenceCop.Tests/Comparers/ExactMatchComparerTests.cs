namespace ReferenceCop.Tests
{
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
            Assert.IsTrue(result);
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
            Assert.IsFalse(result);
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
            Assert.AreEqual(obj.GetHashCode(), result);
        }
    }
}
