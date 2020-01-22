using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Brander
{
    class Program
    {
        static void Main(string[] args)
        {
            string branderFile = args[0];
            string branderDir = Directory.GetParent(branderFile).FullName;

            JObject branderConfig = JObject.Parse(File.ReadAllText(branderFile));

            if (branderConfig["templates"] != null)
            {
                foreach (JObject template in branderConfig["templates"])
                {
                    string templateSourcePath = Path.Combine(branderDir, template["source"].Value<string>());
                    string templateSourceContent = File.ReadAllText(templateSourcePath);

                    string finalContent = Regex.Replace(templateSourceContent, @"\{{([a-zA-Z0-9]*)}}", (m) =>
                    {
                        if (m.Groups != null && m.Groups.Count > 0)
                        {
                            JToken variableValue = branderConfig["variables"][m.Groups[1].Value];
                            if (variableValue != null)
                                return variableValue.Value<string>();
                        }
                        return string.Empty;
                    });

                    string templateTargetPath = Path.Combine(branderDir, template["target"].Value<string>());
                    File.WriteAllText(templateTargetPath, finalContent);
                }
            }

        }
    }
}