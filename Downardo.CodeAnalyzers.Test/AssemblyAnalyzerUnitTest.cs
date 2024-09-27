using Downardo.CodeAnalyzers.Test.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using Downardo.CodeAnalyzers.DesignAnalyzers;

namespace Wintellect.Analyzers.Test
{
    [TestClass]
    public class AssemblyAttributesUnitTests : CodeFixVerifier
    {
        const String AssembliesHaveCompanyAttributeAnalyzerId = "DOW0001";
        const String AssembliesHaveCompanyAttributeAnalyzerMessageFormat = "Assemblies should have a company attribute";
        const String AssembliesHaveCopyrightAttributeAnalyzerId = "DOW0002";
        const String AssembliesHaveCopyrightAttributeAnalyzerMessageFormat = "Assemblies should have a copyright attribute";
        const String AssembliesHaveDescriptionAttributeAnalyzerId = "DOW0003";
        const String AssembliesHaveDescriptionAttributeAnalyzerMessageFormat = "Assemblies should have a description attribute";
        const String AssembliesHaveTitleAttributeAnalyzerId = "DOW0004";
        const String AssembliesHaveTitleAttributeAnalyzerMessageFormat = "Assemblies should have a title attribute";

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesNonePresent()
        {
            var expected = new DiagnosticResult[4];
            expected[0] = CompanyAttributeResult;
            expected[1] = CopyrightAttributeResult;
            expected[2] = DescriptionAttributeResult;
            expected[3] = TitleAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

namespace SomeTests
{
    public class BasicClass
    {
        public void SomeWork(String message)
        {
            Console.WriteLine(message);
        }
    }
}
",          expected);
        }

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesPresentButEmpty()
        {
            var expected = new DiagnosticResult[4];
            expected[0] = CompanyAttributeResult;
            expected[1] = CopyrightAttributeResult;
            expected[2] = DescriptionAttributeResult;
            expected[3] = TitleAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

[assembly: AssemblyTitle("""")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyCopyright("""")]

namespace SomeTests
{
    public class BasicClass
    {
        public void SomeWork(String message)
        {
            Console.WriteLine(message);
        }
    }
}
", expected);
        }

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesOnlyTitleFilled()
        {
            var expected = new DiagnosticResult[3];
            expected[0] = CompanyAttributeResult;
            expected[1] = CopyrightAttributeResult;
            expected[2] = DescriptionAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

[assembly: AssemblyTitle(""De oppresso liber"")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyCopyright("""")]

namespace SomeTests
{
    public class BasicClass
    {
        public void SomeWork(String message)
        {
            Console.WriteLine(message);
        }
    }
}
", expected);
        }

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesOnlyDescriptionFilled()
        {
            var expected = new DiagnosticResult[3];
            expected[0] = CompanyAttributeResult;
            expected[1] = CopyrightAttributeResult;
            expected[2] = TitleAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

[assembly: AssemblyTitle("""")]
[assembly: AssemblyDescription(""De oppresso liber"")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyCopyright("""")]

namespace SomeTests
{
    public class BasicClass
    {
        public void SomeWork(String message)
        {
            Console.WriteLine(message);
        }
    }
}
", expected);
        }

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesOnlyCopyrightFilled()
        {
            var expected = new DiagnosticResult[3];
            expected[0] = CompanyAttributeResult;
            expected[1] = DescriptionAttributeResult;
            expected[2] = TitleAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

[assembly: AssemblyTitle("""")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyCopyright(""De oppresso liber"")]

namespace SomeTests
{
    public class BasicClass
    {
        public void SomeWork(String message)
        {
            Console.WriteLine(message);
        }
    }
}
", expected);
        }

        [TestMethod]
        [TestCategory("AssemblyAttributeUnitTests")]
        public void TestCSharpAtrributesOnlyCompanyFilled()
        {
            var expected = new DiagnosticResult[3];
            expected[0] = CopyrightAttributeResult;
            expected[1] = DescriptionAttributeResult;
            expected[2] = TitleAttributeResult;

            VerifyCSharpDiagnostic(@"
using System;
using System.Reflection;

[assembly: AssemblyTitle("""")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyCompany(""De opresson liber"")]
[assembly: AssemblyCopyright("""")]

namespace SomeTests

    public class BasicClass
    {
        public void SomeWork(String message)
        {
            Console.WriteLine(message);
        }
    }
}
", expected);
        }


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new AssemblyAnalyzer();

        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer() => new AssemblyAnalyzer();

        private static DiagnosticResult CompanyAttributeResult = new DiagnosticResult
        {
            Id = AssembliesHaveCompanyAttributeAnalyzerId,
            Message = AssembliesHaveCompanyAttributeAnalyzerMessageFormat,
            Severity = DiagnosticSeverity.Warning,
        };

        private static DiagnosticResult CopyrightAttributeResult = new DiagnosticResult
        {
            Id = AssembliesHaveCopyrightAttributeAnalyzerId,
            Message = AssembliesHaveCopyrightAttributeAnalyzerMessageFormat,
            Severity = DiagnosticSeverity.Warning,
        };

        private static DiagnosticResult DescriptionAttributeResult = new DiagnosticResult
        {
            Id = AssembliesHaveDescriptionAttributeAnalyzerId,
            Message = AssembliesHaveDescriptionAttributeAnalyzerMessageFormat,
            Severity = DiagnosticSeverity.Warning,
        };

        private static DiagnosticResult TitleAttributeResult = new DiagnosticResult
        {
            Id = AssembliesHaveTitleAttributeAnalyzerId,
            Message = AssembliesHaveTitleAttributeAnalyzerMessageFormat,
            Severity = DiagnosticSeverity.Warning,
        };
    }
}