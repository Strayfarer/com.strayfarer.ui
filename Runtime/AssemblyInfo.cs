#nullable enable
using System.Runtime.CompilerServices;
using Strayfarer.UI;

[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_EDITOR)]
[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_TESTS_EDITMODE)]
[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_TESTS_PLAYMODE)]

namespace Strayfarer.UI {
    static class AssemblyInfo {
        public const string ID = "net.slothsoft.test-runner";

        public const string NAMESPACE_RUNTIME = "Strayfarer.UI";
        public const string NAMESPACE_EDITOR = "Strayfarer.UI.Editor";

        public const string NAMESPACE_TESTS_PLAYMODE = "Strayfarer.UI.Tests.PlayMode";
        public const string NAMESPACE_TESTS_EDITMODE = "Strayfarer.UI.Tests.EditMode";

        public const string NAMESPACE_PROXYGEN = "DynamicProxyGenAssembly2";
    }
}