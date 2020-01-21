using System;
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

            foreach (var template in branderConfig["templates"])
            {
                var templateSourcePath = Path.Combine(branderDir, template["source"].Value<string>());
                
            }

            var rep = Regex.Replace("hello {{greetingMessage}} you are so cool {{sss}})", @"\{{([a-zA-Z0-9]*)}}", (m) =>
             {
                 if(m.Groups != null && m.Groups.Count > 0)
                 {
                     JToken variableValue = branderConfig["variables"][m.Groups[1].Value];
                     if (variableValue != null)
                         return variableValue.Value<string>();
                 }
                 return string.Empty;
             });

            Console.WriteLine(rep);
        }
    }
}
