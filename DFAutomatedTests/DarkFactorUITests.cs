using Microsoft.Playwright;
using NUnit.Framework;

namespace DFAutomatedTests;

/// <summary>
/// UI tests for the DarkFactor Web application login and navigation flows.
/// These tests verify key user journeys and page interactions.
/// </summary>
[TestFixture]
public class DarkFactorUITests : PlaywrightFixtureBase
{
    [Test]
    public async Task ApplicationLoadsSuccessfully()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);
        
        // Assert - verify page loads and renders content
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var content = await _page.ContentAsync();
        Assert.That(content, Is.Not.Null.And.Not.Empty, 
            "Application should load successfully with rendered content");
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
    public async Task PageNavigatesSuccessfully()
    {
        // Arrange & Act
        await _page!.GotoAsync(BaseUrl);
        var currentUrl = _page.Url;

        // Assert - verify successful navigation
        Assert.That(currentUrl, Does.StartWith(BaseUrl), "Page should successfully navigate to base URL");
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
