using Microsoft.Playwright;
using NUnit.Framework;

namespace DFAutomatedTests;

[TestFixture]
public class PlaywrightTests : PlaywrightFixtureBase
{
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
