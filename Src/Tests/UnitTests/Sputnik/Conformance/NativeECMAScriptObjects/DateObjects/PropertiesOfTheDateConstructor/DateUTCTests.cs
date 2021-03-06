// <auto-generated />
namespace IronJS.Tests.UnitTests.Sputnik.Conformance.NativeECMAScriptObjects.DateObjects.PropertiesOfTheDateConstructor
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class DateUTCTests : SputnikTestFixture
    {
        public DateUTCTests()
            : base(@"Conformance\15_Native_ECMA_Script_Objects\15.9_Date_Objects\15.9.4_Properties_of_the_Date_Constructor\15.9.4.3_Date.UTC")
        {
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.9.4.3")]
        [TestCase("S15.9.4.3_A1_T1.js", Description = "The Date property \"UTC\" has { DontEnum } attributes")]
        [TestCase("S15.9.4.3_A1_T2.js", Description = "The Date property \"UTC\" has { DontEnum } attributes")]
        [TestCase("S15.9.4.3_A1_T3.js", Description = "The Date property \"UTC\" has { DontEnum } attributes")]
        public void TheDatePropertyUTCHasDontEnumAttributes(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.9.4.3")]
        [TestCase("S15.9.4.3_A2_T1.js", Description = "The \"length\" property of the \"UTC\" is 7")]
        public void TheLengthPropertyOfTheUTCIs7(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.9.4.3")]
        [TestCase("S15.9.4.3_A3_T1.js", Description = "The Date.UTC property \"length\" has { ReadOnly, DontDelete, DontEnum } attributes")]
        [TestCase("S15.9.4.3_A3_T2.js", Description = "The Date.UTC property \"length\" has { ReadOnly, DontDelete, DontEnum } attributes")]
        [TestCase("S15.9.4.3_A3_T3.js", Description = "The Date.UTC property \"length\" has { ReadOnly, DontDelete, DontEnum } attributes")]
        public void TheDateUTCPropertyLengthHasReadOnlyDontDeleteDontEnumAttributes(string file)
        {
            RunFile(file);
        }
    }
}