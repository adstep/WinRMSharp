namespace WinRMSharp.Tests.Sessions
{
    using System;
    using System.Text.RegularExpressions;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    internal class SecretsMaskingConverter : IYamlTypeConverter
    {
        private readonly Tuple<string, string, bool>[]? _replacements;

        public SecretsMaskingConverter(Tuple<string, string, bool>[]? replacements)
        {
            _replacements = replacements;
        }

        public bool Accepts(Type type) => type == typeof(string);

        public object ReadYaml(IParser parser, Type type)
        {
            // Read as normal
            return parser.Consume<Scalar>().Value;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            string str = value as string ?? string.Empty;

            if (_replacements != null)
            {
                foreach ((string pattern, string replacement, bool isRegEx) in _replacements)
                {
                    string escapedPattern = isRegEx ? pattern : Regex.Escape(pattern);

                    str = Regex.Replace(str, escapedPattern, replacement);
                }
            }

            emitter.Emit(new Scalar(str));
        }
    }
}
