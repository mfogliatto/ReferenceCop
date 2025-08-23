namespace ReferenceCop.Roslyn.Tests
{
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ReferenceCop;

    [TestClass]
    public class NoWarnAssembliesProviderTests
    {
        [TestMethod]
        public void GetNoWarnByAssembly_WhenNullInput_ReturnsEmptyDictionary()
        {
            // Arrange.
            var provider = new NoWarnAssembliesProvider();

            // Act.
            var result = provider.GetNoWarnByAssembly(null);

            // Assert.
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void GetNoWarnByAssembly_WhenEmptyInput_ReturnsEmptyDictionary()
        {
            // Arrange.
            var provider = new NoWarnAssembliesProvider();

            // Act.
            var result = provider.GetNoWarnByAssembly(string.Empty);

            // Assert.
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void GetNoWarnByAssembly_WhenValidInput_ReturnsDictionaryWithEntries()
        {
            // Arrange.
            var provider = new NoWarnAssembliesProvider();
            string noWarnAssemblies = "Assembly1|RC0001,RC0002;Assembly2|RC0002";

            // Act.
            var result = provider.GetNoWarnByAssembly(noWarnAssemblies);

            // Assert.
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result.Should().ContainKey("Assembly1");
                result.Should().ContainKey("Assembly2");

                var assembly1Codes = result["Assembly1"].ToList();
                assembly1Codes.Should().HaveCount(2);
                assembly1Codes.Should().Contain("RC0001");
                assembly1Codes.Should().Contain("RC0002");

                var assembly2Codes = result["Assembly2"].ToList();
                assembly2Codes.Should().HaveCount(1);
                assembly2Codes.Should().Contain("RC0002");
            }
        }

        [TestMethod]
        public void GetNoWarnByAssembly_WhenInvalidEntries_IgnoresInvalidEntries()
        {
            // Arrange.
            var provider = new NoWarnAssembliesProvider();
            string noWarnAssemblies = "Assembly1|RC0001;InvalidEntry;Assembly2|RC0002";

            // Act.
            var result = provider.GetNoWarnByAssembly(noWarnAssemblies);

            // Assert.
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result.Should().ContainKey("Assembly1");
                result.Should().ContainKey("Assembly2");
            }
        }

        [TestMethod]
        public void GetNoWarnByAssembly_WhenInputContainsWhitespace_PreservesAssemblyNamesTrimsCodeValues()
        {
            // Arrange.
            var provider = new NoWarnAssembliesProvider();
            string noWarnAssemblies = " Assembly1 | RC0001 , RC0002 ; Assembly2 | RC0002 ";

            // Act.
            var result = provider.GetNoWarnByAssembly(noWarnAssemblies);

            // Assert.
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result.Should().ContainKey(" Assembly1 ");
                result.Should().ContainKey(" Assembly2 ");

                var assembly1Codes = result[" Assembly1 "].ToList();
                assembly1Codes.Should().HaveCount(2);
                assembly1Codes.Should().Contain("RC0001");
                assembly1Codes.Should().Contain("RC0002");
            }
        }

        [TestMethod]
        public void GetNoWarnByAssembly_WhenMultipleCodesForSameAssembly_StoresAllCodes()
        {
            // Arrange.
            var provider = new NoWarnAssembliesProvider();
            string noWarnAssemblies = "Assembly1|RC0001,RC0002,RC0003";

            // Act.
            var result = provider.GetNoWarnByAssembly(noWarnAssemblies);

            // Assert.
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(1);

                var codes = result["Assembly1"].ToList();
                codes.Should().HaveCount(3);
                codes.Should().Contain("RC0001");
                codes.Should().Contain("RC0002");
                codes.Should().Contain("RC0003");
            }
        }

        [TestMethod]
        public void GetNoWarnByAssembly_WhenEntryHasNoCodes_StoresEmptyCollection()
        {
            // Arrange.
            var provider = new NoWarnAssembliesProvider();
            string noWarnAssemblies = "Assembly1|";

            // Act.
            var result = provider.GetNoWarnByAssembly(noWarnAssemblies);

            // Assert.
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(1);
                result["Assembly1"].Should().BeEmpty();
            }
        }
    }
}
