# Current Tasks

Rolling task list - most important at top. Update as tasks complete.

---

## NOW - Priority Tasks

### 1. Test ForeverTools.AIML NuGet Installation - PASSED
Package installed and tested successfully:

```powershell
# Create test project
dotnet new console -n TestAimlInstall -o C:\xampp2\htdocs\git_projects\TestAimlInstall
cd C:\xampp2\htdocs\git_projects\TestAimlInstall

# Install package
dotnet add package ForeverTools.AIML

# Verify IntelliSense works - add this to Program.cs:
# using ForeverTools.AIML;
# var client = new AimlApiClient("test");
```

**Verification Checklist:**
- [x] Package installs successfully
- [x] IntelliSense shows `AimlApiClient`, `AimlModels`, etc.
- [x] Code compiles and runs
- [ ] Package page looks correct: https://www.nuget.org/packages/ForeverTools.AIML
- [ ] Test with real API key (optional)

### 2. Test with Real API Key (Optional but Recommended)
If you have an AI/ML API key, test actual functionality:

```csharp
using ForeverTools.AIML;

var client = new AimlApiClient("your-actual-api-key");
var response = await client.ChatAsync("Say hello in 5 words or less");
Console.WriteLine(response);
```

### 3. Update GitHub Organization README
After confirming package works, copy `docs/README_TEMPLATE.md` content to:
https://github.com/ForeverTools/ForeverTools (edit README.md)

---

## NEXT - Upcoming Tasks

### 4. Build ForeverTools.Captcha
- Affiliate ready: `soft_id: 2482` (2Captcha)
- Multi-provider: 2Captcha, Anti-Captcha, CapSolver
- Same structure as AIML package

### 5. Register More Affiliate Accounts
Before building each package, register for affiliates:

| Service | Signup URL | Status |
|---------|------------|--------|
| Anti-Captcha | https://anti-captcha.com/clients/tools/devcenter | [ ] |
| CapSolver | https://dashboard.capsolver.com/dashboard/affiliate | [ ] |
| ScraperAPI | https://www.scraperapi.com/affiliates/ | [ ] |
| BrightData | https://brightdata.com/affiliate | [ ] |

---

## DONE - Completed Tasks

### ForeverTools.AIML - Published
- [x] Build package (multi-target: net8.0, net6.0, netstandard2.0)
- [x] 21 unit tests passing
- [x] Affiliate link embedded (`?via=forevertools`)
- [x] Icon created (SVG→PNG)
- [x] Push to GitHub: https://github.com/ForeverTools/ForeverTools
- [x] Push to NuGet: https://www.nuget.org/packages/ForeverTools.AIML
- [ ] **Verify installation works** ← Current task

### One-Time Setup - Complete
- [x] NuGet account: ForeverTools
- [x] GitHub account: ForeverTools
- [x] API key set in environment variable
- [x] SVG→PNG converter tool created

---

## Package Status

| Package | Status | NuGet |
|---------|--------|-------|
| ForeverTools.AIML | Published - Testing | [Link](https://www.nuget.org/packages/ForeverTools.AIML) |
| ForeverTools.Captcha | Next | - |
| ForeverTools.ScraperAPI | Planned | - |
| ForeverTools.BrightData | Planned | - |

---

## Registered Affiliate Codes

| Service | Code | Type |
|---------|------|------|
| AI/ML API | `?via=forevertools` | Link-based (30%) |
| 2Captcha | `soft_id: 2482` | API-embedded (10%) |

---

## Quick Commands

```powershell
# Test package install
dotnet new console -n TestInstall
cd TestInstall
dotnet add package ForeverTools.AIML

# Build & test solution
cd C:\xampp2\htdocs\git_projects\ForeverTools
dotnet build -c Release
dotnet test

# Convert SVG to PNG
dotnet run --project tools/SvgToPng -- "assets/icon.svg" "assets/icon.png" 128
```

---

*Last Updated: December 2024*
