# ForeverTools Project Context

**Use this file to quickly onboard Claude to the ForeverTools project.**

## Quick Start Prompt

```
I'm working on ForeverTools, a portfolio of .NET NuGet packages that wrap popular developer APIs.
The goal is passive income through affiliate tracking embedded in packages.

Please read these files in order:
1. CLAUDE_PROMPT.md (this file) - credentials, workflow, quick reference
2. docs/MASTER_PLAN.md - full project plan, affiliate research, package roadmap
3. docs/CURRENT_TASKS.md - pre-publish checklist, affiliate codes
4. docs/DISTRIBUTION_GUIDE.md - NuGet publishing workflow

Project location: C:\xampp2\htdocs\git_projects\ForeverTools\
```

---

## Account Credentials

### NuGet.org
- **Username:** ForeverTools
- **Profile:** https://www.nuget.org/profiles/ForeverTools
- **API Key:** Stored in environment variable `NUGET_API_KEY` (expires Dec 2025)
- **Glob Pattern:** `ForeverTools.*`

### GitHub
- **Username:** ForeverTools
- **Organization README:** Update `docs/README_TEMPLATE.md` then push to main repo
- **Repos:**
  - https://github.com/ForeverTools/ForeverTools (main org README)
  - https://github.com/ForeverTools/ForeverTools.AIML (coming soon)

---

## Affiliate Codes (IMPORTANT)

| Service | Type | Code/Link | Commission |
|---------|------|-----------|------------|
| AI/ML API | Link-based | `https://aimlapi.com?via=forevertools` | 30% recurring |
| 2Captcha | API-embedded | `soft_id: 2482` | 10% |
| CapSolver | API-embedded | App ID: `0E76F2D8-D6C0-41DD-A3B6-3708694C47B0` | 5-20% |
| CapSolver | Link-based | `https://dashboard.capsolver.com/passport/register?inviteCode=n2xFnv9zTix7` | - |
| Anti-Captcha | Link-based | `https://getcaptchasolution.com/03pywzqopu` | 10% |
| ScraperAPI | Link-based | `https://www.scraperapi.com?fp_ref=chris88` | 50% recurring |
| APILayer | Link-based | `https://apilayer.com?fpr=chris72` | 15-20% |

**Future packages should use these codes. AI/ML API link is already embedded in ForeverTools.AIML.**

---

## Current Package Status

| Package | Built | Tested | Affiliate | Published | Notes |
|---------|-------|--------|-----------|-----------|-------|
| ForeverTools.AIML | Yes | Yes (21 tests) | Yes | **Yes** | [NuGet](https://www.nuget.org/packages/ForeverTools.AIML) |
| ForeverTools.Captcha | No | No | Yes (3 providers) | No | Next priority |
| ForeverTools.ScraperAPI | No | No | Yes (50%!) | No | High commission |
| ForeverTools.APILayer | No | No | Yes (15-20%) | No | 23+ APIs |

---

## Publish Workflow

### For Each New Package:

1. **Build & Test Locally**
   ```powershell
   cd C:\xampp2\htdocs\git_projects\ForeverTools
   dotnet build -c Release
   dotnet test
   ```

2. **Pack**
   ```powershell
   dotnet pack src/ForeverTools.{PackageName}/ForeverTools.{PackageName}.csproj -c Release -o ./packages
   ```

3. **Publish to NuGet FIRST**
   ```powershell
   dotnet nuget push packages/ForeverTools.{PackageName}.1.0.0.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
   ```

4. **Wait & Test NuGet Package (BEFORE GitHub)**
   - Wait 15-30 min for indexing
   - Test installation in fresh project:
   ```powershell
   dotnet new console -n TestInstall -o C:\xampp2\htdocs\git_projects\TestInstall
   cd C:\xampp2\htdocs\git_projects\TestInstall
   dotnet add package ForeverTools.{PackageName}
   dotnet run
   ```

5. **Push to GitHub (AFTER testing)**
   Only after NuGet package verified working:
   ```powershell
   cd C:\xampp2\htdocs\git_projects\ForeverTools
   git add .
   git commit -m "Release ForeverTools.{PackageName} v1.0.0"
   git push
   ```

6. **Update Organization README**
   - Edit `docs/README_TEMPLATE.md` - add new package to table
   - Push to https://github.com/ForeverTools/ForeverTools

7. **Update Tracking Docs**
   - `docs/CURRENT_TASKS.md` - mark package as tested & published
   - `CHANGELOG.md` - add version entry

---

## Solution Structure

```
ForeverTools/
├── src/
│   └── ForeverTools.AIML/           # First package (ready)
│       ├── AimlApiClient.cs
│       ├── AimlApiOptions.cs
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs
│       └── Models/
│           └── AimlModels.cs
├── tests/
│   └── ForeverTools.AIML.Tests/     # 21 unit tests
├── docs/
│   ├── MASTER_PLAN.md               # Project roadmap & research
│   ├── DISTRIBUTION_GUIDE.md        # NuGet publishing steps
│   ├── CURRENT_TASKS.md             # Checklist & tracking
│   └── README_TEMPLATE.md           # GitHub org README (update per package)
├── assets/
│   └── icon.svg                     # Convert to PNG before publish
├── .github/
│   └── workflows/
│       └── publish.yml              # CI/CD (build on PR, publish on tag)
├── CLAUDE_PROMPT.md                 # THIS FILE - session context
├── CHANGELOG.md
├── README.md                        # Package README (for NuGet)
├── LICENSE
└── ForeverTools.sln
```

---

## Key Technical Decisions

1. **Multi-targeting:** .NET 8.0, .NET 6.0, .NET Standard 2.0
2. **Affiliate Integration:**
   - Link-based (AI/ML API) - in README/XML comments
   - API-embedded (2Captcha) - `soft_id` in request parameters
3. **OpenAI Compatibility:** Uses OpenAI SDK pointed at AI/ML API endpoint
4. **DI Support:** `AddForeverTools{Service}()` extension methods
5. **Documentation:** Full XML docs for IntelliSense

---

## Common Tasks

### Continue Previous Work
```
Read CLAUDE_PROMPT.md and docs/CURRENT_TASKS.md, then continue where we left off.
```

### Add New Package
```
I want to add ForeverTools.{Name}. Read CLAUDE_PROMPT.md for context,
then follow the same structure as ForeverTools.AIML.
```

### Publish Package
```
Read docs/DISTRIBUTION_GUIDE.md and help me publish ForeverTools.{Name} to NuGet.
```

### Update Affiliate Code
```
Read docs/CURRENT_TASKS.md - I need to update affiliate codes in the codebase.
```

---

## Tools

### SVG to PNG Converter
Located at `tools/SvgToPng/` - converts SVG icons to PNG format for NuGet packages.

```powershell
# Basic usage (outputs to same folder, 128px default)
dotnet run --project tools/SvgToPng -- "assets/icon.svg"

# Full options
dotnet run --project tools/SvgToPng -- "assets/icon.svg" "assets/icon.png" 128
```

---

## Important Reminders

- **Icon:** Convert `assets/icon.svg` to `assets/icon.png` (128x128) using `tools/SvgToPng`
- **API Key Security:** Never commit NuGet API key - use environment variable only
- **Affiliate Links:** Double-check all `?via=forevertools` links before publishing
- **GitHub Sync:** Update `docs/README_TEMPLATE.md` after each package publish

---

## Reference Links

- [AI/ML API](https://aimlapi.com?via=forevertools) - 400+ AI models
- [AI/ML API Docs](https://docs.aimlapi.com/)
- [2Captcha API](https://2captcha.com/2captcha-api) - Use soft_id: 2482
- [CapSolver API](https://docs.capsolver.com/) - Use App ID: 0E76F2D8-D6C0-41DD-A3B6-3708694C47B0
- [Anti-Captcha API](https://anti-captcha.com/apidoc)
- [ScraperAPI](https://www.scraperapi.com?fp_ref=chris88) - 50% recurring commission
- [APILayer](https://apilayer.com?fpr=chris72) - 23+ APIs (geo, currency, phone, email, etc.)
- [NuGet Publishing Docs](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package)

---

*Last Updated: December 2024*
