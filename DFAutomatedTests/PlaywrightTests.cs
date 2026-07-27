using Microsoft.Playwright;
using NUnit.Framework;

namespace DFAutomatedTests;

[TestFixture]
public class PlaywrightTests
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;

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

    [Test]
    public async Task NavigateToHomePage()
    {
        // Test navigating to the home page
        await _page!.GotoAsync("http://localhost:5000");
        
        // Verify page title contains expected text
        var title = await _page.TitleAsync();
        Assert.That(title, Is.Not.Null.And.Not.Empty, "Page should have a title");
    }

    [Test]
    public async Task PageLoadsSuccessfully()
    {
        // Navigate to the application
        var response = await _page!.GotoAsync("http://localhost:5000");
        
        // Verify successful navigation
        Assert.That(response!.Status, Is.EqualTo(200), "Page should load successfully");
    }

    [Test]
    public async Task CanInteractWithPageElements()
    {
        // Navigate to home page
        await _page!.GotoAsync("http://localhost:5000");
        
        // Wait for page to be fully loaded
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Verify page content is present
        var body = await _page.QuerySelectorAsync("body");
        Assert.That(body, Is.Not.Null, "Page should have a body element");
    }
}
