#nullable enable
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Strayfarer.UI.Editor {
    sealed class ParsedQuery {
        static readonly Regex matcher = new(@"[?&]([^=]+)=([^&#]+)");

        readonly Dictionary<string, string> parameters = new();

        public ParsedQuery(string text) {
            foreach (Match match in matcher.Matches(text)) {
                parameters[match.Groups[1].Value] = match.Groups[2].Value;
            }
        }

        public string this[string key] => parameters.GetValueOrDefault(key);
    }
}
