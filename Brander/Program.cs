using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Brander
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string branderPath = args[0];
            string branderFile = string.Empty;
            string branderDir = string.Empty;

            if (Directory.Exists(branderPath))
            {
                branderFile = Path.Combine(branderPath, "brander.json");
                branderDir = branderPath;
            }
            else
            {
                if (File.Exists(branderPath))
                {
                    branderFile = branderPath;
                    branderDir = Directory.GetParent(branderFile).FullName;
                }
                else
                {
                    HttpClient client = new HttpClient();

                    HttpResponseMessage response = await client.GetAsync(branderPath);

                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    {
                        using (FileStream fs = new FileStream("branding.zip", FileMode.Create))
                        {
                            await stream.CopyToAsync(fs);
                        }
                    }

                    ZipFile.ExtractToDirectory("branding.zip", args[1]);

                    File.Delete("branding.zip");

                    branderFile = Path.Combine(args[1], "brander.json");
                    branderDir = args[1];
                }
            }

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
            if (branderConfig["replace"] != null)
            {
                foreach (JObject replace in branderConfig["replace"])
                {
                    string replaceSourcePath = Path.GetFullPath(Path.Combine(branderDir, replace["source"].Value<string>()));
                    string replaceTargetPath = Path.GetFullPath(Path.Combine(branderDir, replace["target"].Value<string>()));


                    if (File.Exists(replaceSourcePath))
                    {
                        File.Copy(replaceSourcePath, replaceTargetPath, true);
                    }
                    else
                    {
                        string[] filesToReplace = Directory.GetFiles(replaceSourcePath, "*.*", SearchOption.AllDirectories);
                        foreach(string file in filesToReplace)
                        {
                            File.Copy(file, file.Replace(replaceSourcePath, replaceTargetPath), true);
                        }
                    }
                }
            }

            if(args.Length > 1)
            {
                Directory.Delete(args[1], true);
            }
        }
    }
}