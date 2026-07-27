using Microsoft.Playwright;
using NUnit.Framework;

namespace DFAutomatedTests;

public abstract class PlaywrightFixtureBase
{
    protected IPlaywright? _playwright;
    protected IBrowser? _browser;
    protected IPage? _page;

    [OneTimeSetUp]
    public async Task InitializePlaywright()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
    }

    [SetUp]
    public async Task CreatePage()
    {
        Assert.That(_browser, Is.Not.Null, "Playwright browser must be initialized in [OneTimeSetUp] before creating pages.");
        _page = await _browser.NewPageAsync();
    }

    [TearDown]
    public async Task ClosePage()
    {
        if (_page != null)
        {
            await _page.CloseAsync();
        }
    }

    [OneTimeTearDown]
    public async Task DisposePlaywright()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
        }
        _playwright?.Dispose();
    }
}
