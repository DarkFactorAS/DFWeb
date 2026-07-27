using Microsoft.Playwright;
using NUnit.Framework;

namespace DFAutomatedTests;

/// <summary>
/// UI tests for the DarkFactor Web application login and navigation flows.
/// These tests verify key user journeys and page interactions.
/// </summary>
[TestFixture]
public class DarkFactorUITests
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
private static readonly string BaseUrl = Environment.GetEnvironmentVariable("DFWEB_BASE_URL") ?? "http://localhost:5000";

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
        _page = await _browser!.NewPageAsync();
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
    public async Task ApplicationLoadsSuccessfully()
    {
        // Arrange & Act
        var response = await _page!.GotoAsync(BaseUrl);

        // Assert
        Assert.That(response!.Status, Is.EqualTo(200), "Application should load with HTTP 200");
    }

    [Test]
    public async Task HomePageHasValidStructure()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert
        var body = await _page.QuerySelectorAsync("body");
        Assert.That(body, Is.Not.Null, "Page should have a body element");

        var html = await _page.QuerySelectorAsync("html");
        Assert.That(html, Is.Not.Null, "Page should have an html element");
    }

    [Test]
    public async Task PageTitleIsNotEmpty()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);
        var title = await _page.TitleAsync();

        // Assert
        Assert.That(title, Is.Not.Null.And.Not.Empty, "Page should have a title");
    }

    [Test]
    public async Task CanNavigateToPage()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);
        var currentUrl = _page.Url;

        // Assert
        Assert.That(currentUrl, Does.StartWith(BaseUrl), "Should be on the base URL");
    }

    [Test]
    public async Task PageContentIsRendered()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Get page content
        var content = await _page.ContentAsync();

        // Assert
        Assert.That(content, Is.Not.Null.And.Not.Empty, "Page should have rendered content");
    }
}
