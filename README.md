# Brander
CLI tool for your white-label needs

We created Brander to provide a fast environment to replace, change and add files to brand our while label mobile apps. In the end, Brander can be used to brand any kind project.

## Usage

Create a file inside a folder named ```brander.json``` with content like the sample:

```json
{
  "variables": {
    "greetingMessage": "hello earth!",
    "dateFormat": "MM-dd-yy"
  },
  "templates": [
    {
      "source": "Constants.txt",
      "target": "../Constants.cs"
    }
  ],
  "replace": [
    {
        "source": "ASCIIImage.txt",
        "target": "../ASCIIImage.txt"
    },
    {
        "source": "BrandedFolder",
        "target": "../SampleFolder"
    }
  ]
}
```
Where the variables a going to be replaced in the template files. The template is something like:

```csharp
namespace Brander.Sample
{
    public static class Constants
    {
        public const string GreetingMessage = "{{greetingMessage}}";
        public const string DateFormat = "{{dateFormat}}";
    }
}
```
Note the variables names inside {{variableName}}

The replace group will replace single files or folders and files inside.

Then we can call our Brander with brander.json path

```sh
./Brander branderFolder/brander.json
```

Whe can provide only the folder name, if the ```brander.json``` file is inside that folder.

```sh
./Brander branderFolder/brander.json
```

If your branding is hosted remotely, we can pass an url to a zip file, with a ```brander.json``` file and content inside that zip and set the destination path to unzip and brand:

```sh
./Brander https://example.com/branding.zip pathToUnzipBranding/
```
