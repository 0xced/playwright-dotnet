using System;
using Xunit;

namespace SampleCode;

public class UnitTest
{
    static UnitTest()
    {
        Environment.SetEnvironmentVariable("https_proxy", "http://127.0.0.1:33333");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void InstallFirefox(bool throwOnError)
    {
        var playwright = new Microsoft.Playwright.Program();
        var exitCode = playwright.Run(["install", "--with-deps", "firefox"], throwOnError);
        Assert.Equal(0, exitCode);
    }
}
