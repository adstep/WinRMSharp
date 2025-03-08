namespace WinRMSharp.Tests.Sessions
{
    using System;
    using System.Collections.Generic;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    internal class SecretsMaskingConverter : IYamlTypeConverter
    {
        private readonly Dictionary<string, string>? _replacements;

        public SecretsMaskingConverter(Dictionary<string, string>? replacements)
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
                foreach ((string original, string replacement) in _replacements)
                {
                    str = str.Replace(original, replacement);
                }
            }

            emitter.Emit(new Scalar(str));
        }
    }
}
