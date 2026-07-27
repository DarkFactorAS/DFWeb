# DFAutomatedTests - Playwright UI Tests

This project contains automated UI tests for the DarkFactor Web application using Playwright.

## Setup

### Prerequisites
- .NET 9.0 or later
- Node.js (for Playwright browser binaries, optional)

### Installation

1. The Playwright NuGet packages are already configured in `DFAutomatedTests.csproj`
2. The first time you run tests, Playwright will automatically download the required browsers

### Running Tests

#### Using dotnet CLI
```bash
cd DFAutomatedTests
dotnet test
```

#### Using Visual Studio Test Explorer
1. Open Test Explorer (Ctrl+E, T)
2. Look for tests under "DFAutomatedTests"
3. Run individual tests or all tests

#### Run specific test class
```bash
dotnet test --filter "DFAutomatedTests.PlaywrightTests"
```

#### Run with verbose output
```bash
dotnet test -v detailed
```

## Test Configuration

The tests are configured with:
- **Browser**: Chromium (headless mode by default)
- **Framework**: NUnit 4.0
- **Base URL**: `http://localhost:5000`

### Modifying Test Configuration

Edit `PlaywrightTests.cs` to change:
- Browser type (Firefox, WebKit, Edge)
- Headless mode (set `Headless = false` to see browser)
- Base URL (if running on different port)

## Test Examples

Current tests include:
1. **NavigateToHomePage** - Verifies page navigation and title
2. **PageLoadsSuccessfully** - Checks HTTP response status
3. **CanInteractWithPageElements** - Tests page DOM elements

## Adding New Tests

1. Add new test methods to `PlaywrightTests.cs`
2. Use the `_page` instance to interact with the application:
   ```csharp
   [Test]
   public async Task MyNewTest()
   {
       await _page!.GotoAsync("http://localhost:5000/path");
       // Add assertions and interactions
   }
   ```

## Playwright Documentation

For more information on Playwright for .NET, visit:
https://playwright.dev/dotnet/

## Common Playwright Methods

- `_page!.GotoAsync(url)` - Navigate to URL
- `_page!.ClickAsync(selector)` - Click element
- `_page!.FillAsync(selector, text)` - Fill text input
- `_page!.QuerySelectorAsync(selector)` - Find element
- `_page!.WaitForLoadStateAsync()` - Wait for page load
- `_page!.ScreenshotAsync(path)` - Take screenshot

## Troubleshooting

### Tests fail with connection refused
- Ensure the web application is running on http://localhost:5000
- Check that DFWeb.FR.BootStrap is started before running tests

### Browser won't launch
- Run `dotnet build` to ensure Playwright packages are restored
- Check that you have write permissions in the output directory

### Tests timeout
- Increase timeout in test methods: `_page!.GotoAsync(url, new() { Timeout = 30000 })`
