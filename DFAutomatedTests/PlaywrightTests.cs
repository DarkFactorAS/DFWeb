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
        await _page!.GotoAsync(BaseUrl);
        
        // Verify we successfully navigated (URL should start with BaseUrl)
        var currentUrl = _page.Url;
        Assert.That(currentUrl, Does.StartWith(BaseUrl), "Should be able to navigate to home page");
    }

    [Test]
    public async Task PageLoadsSuccessfully()
    {
        // Navigate to the application
        await _page!.GotoAsync(BaseUrl);
        
        // Verify we can access the page (it loads without error)
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var content = await _page.ContentAsync();
        Assert.That(content, Is.Not.Null.And.Not.Empty, "Page should load with content");
    }

    [Test]
    public async Task CanInteractWithPageElements()
    {
        // Navigate to home page
        await _page!.GotoAsync(BaseUrl);
        
        // Wait for page to be fully loaded
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Verify page content is present
        var body = await _page.QuerySelectorAsync("body");
        Assert.That(body, Is.Not.Null, "Page should have a body element");
    }
}
