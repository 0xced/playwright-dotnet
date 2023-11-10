using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Playwright;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Playwright.Tooling;

internal class JsonSchemaGenerator : Command
{
    private readonly IAnsiConsole _console;

    public JsonSchemaGenerator(IAnsiConsole console)
    {
        _console = console;
    }

    private static void CleanupDescriptions(JsonSchema schema)
    {
        // https://github.com/SchemaStore/schemastore/blob/e3deae264e76db80cd1cb403cfcda805d3dedb1b/CONTRIBUTING.md#best-practices
        // > Don't end title/description values with colon.
        schema.Description = schema.Description?.Replace("\n", " ").TrimEnd('.');
        foreach (var (_, property) in schema.Properties)
        {
            CleanupDescriptions(property);
        }
    }

    private static string GetRootPath([CallerFilePath] string path = "") => Path.Combine(Path.GetDirectoryName(path)!, "..", "..", "..");

    private static string GetSchemaFilePath(string fileName) => Path.GetFullPath(Path.Combine(GetRootPath(), ".schema", fileName));

    public override int Execute(CommandContext context)
    {
        foreach (var type in typeof(JsonSchemaGenerator).Assembly.GetExportedTypes().Where(e => e.Name.EndsWith("Options")))
        {
            WriteSchema(type);
        }
        return 0;
    }

    private void WriteSchema(Type type)
    {
        var settings = new SystemTextJsonSchemaGeneratorSettings { XmlDocumentationFormatting = XmlDocsFormattingMode.Markdown };
        var jsonSchema = JsonSchema.FromType(type, settings);
        jsonSchema.Title = type.FullName;
        CleanupDescriptions(jsonSchema);
        var schemaFilePath = GetSchemaFilePath($"{type.Name}.json");
        File.WriteAllText(schemaFilePath, jsonSchema.ToJson());
        _console.WriteLine($"Written {schemaFilePath}");
    }
}
