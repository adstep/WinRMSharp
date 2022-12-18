using System.Text;
using System.Xml.Linq;
using WinRMSharp.Utils;

namespace WinRMSharp
{
    internal class Powershell
    {
        public static string Command(string command)
        {
            // Disable unnecessary progress bars which considered as stderr.
            //command = $"$ProgressPreference = 'SilentlyContinue'; {command}";

            command = string.Join("\n", command.Split('\n').Select(s => s.Trim()));

            byte[] plainTextBytes = Encoding.Unicode.GetBytes(command);
            string encodedCommand = Convert.ToBase64String(plainTextBytes);

            return $"powershell.exe -EncodedCommand {encodedCommand}";
        }

        public static string ParseError(string text)
        {
            int startIndex = text.IndexOf("<Objs ");
            int endIndex = text.IndexOf("</Objs>") + 7;

            text = text.Substring(startIndex, endIndex - startIndex);

            XDocument root = Xml.Parse(text);

            string[] errorLines = root.Descendants()
                .Where(e => e.Attribute("S")?.Value == "Error")
                .Select(e => e.Value.Replace("_x000D__x000A_", string.Empty))
                .ToArray();

            return string.Join(Environment.NewLine, errorLines);
        }
    }
}
