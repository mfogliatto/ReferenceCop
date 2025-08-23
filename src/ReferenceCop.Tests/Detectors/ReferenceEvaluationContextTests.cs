namespace ReferenceCop.Tests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ReferenceEvaluationContextTests
    {
        [TestMethod]
        public void ToString_WhenReferenceIsNull_ReturnsFormattedStringWithNull()
        {
            // Arrange.
            var context = new ReferenceEvaluationContext<string>(null);

            // Act.
            string result = context.ToString();

            // Assert.
            result.Should().Be("Reference: null (Warnings Enabled)");
        }

        [TestMethod]
        public void ToString_WhenReferenceIsNotNull_ReturnsFormattedStringWithReference()
        {
            // Arrange.
            const string assemblyName = "TestAssembly";
            var context = new ReferenceEvaluationContext<string>(assemblyName);

            // Act.
            string result = context.ToString();

            // Assert.
            result.Should().Be("Reference: TestAssembly (Warnings Enabled)");
        }

        [TestMethod]
        public void ToString_WhenWarningsAreSuppressed_ReturnsFormattedStringWithSuppressedStatus()
        {
            // Arrange.
            const string assemblyName = "TestAssembly";
            var context = new ReferenceEvaluationContext<string>(assemblyName, isWarningSuppressed: true);

            // Act.
            string result = context.ToString();

            // Assert.
            result.Should().Be("Reference: TestAssembly (Warnings Suppressed)");
        }

        [TestMethod]
        public void ToString_WithCustomReferenceType_UsesReferenceToStringMethod()
        {
            // Arrange.
            var testReference = new TestReference("CustomAssembly");
            var context = new ReferenceEvaluationContext<TestReference>(testReference);

            // Act.
            string result = context.ToString();

            // Assert.
            result.Should().Be("Reference: CustomAssembly (Warnings Enabled)");
        }

        private class TestReference
        {
            public TestReference(string name)
            {
                this.Name = name;
            }

            public string Name { get; }

            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}
