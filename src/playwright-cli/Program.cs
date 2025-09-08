using System;

Environment.SetEnvironmentVariable("https_proxy", "http://127.0.0.1:33333");

try
{
    var playwright = new Microsoft.Playwright.Program();
    var exitCode = playwright.Run(["install", "--with-deps", "firefox"], throwOnError: args is ["--throwOnError"]);
    Console.WriteLine($"install exit code: {exitCode}");
}
catch (Exception exception)
{
    Console.WriteLine($"💥 {exception}");
}
