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

**Future packages should use these codes. AI/ML API link is already embedded in ForeverTools.AIML.**

---

## Current Package Status

| Package | Built | Tested | Affiliate | Published | Notes |
|---------|-------|--------|-----------|-----------|-------|
| ForeverTools.AIML | Yes | Yes (21 tests) | Yes | No | Ready to publish |
| ForeverTools.Captcha | No | No | Yes (2482) | No | Next priority |

---

## Publish Workflow

### For Each New Package:

1. **Build & Test**
   ```powershell
   cd C:\xampp2\htdocs\git_projects\ForeverTools
   dotnet build -c Release
   dotnet test
   ```

2. **Pack**
   ```powershell
   dotnet pack src/ForeverTools.{PackageName}/ForeverTools.{PackageName}.csproj -c Release -o ./packages
   ```

3. **Publish to NuGet**
   ```powershell
   dotnet nuget push packages/ForeverTools.{PackageName}.1.0.0.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
   ```

4. **Create GitHub Repo** (if separate repo per package)
   - Create repo: `ForeverTools/ForeverTools.{PackageName}`
   - Push code
   - Enable GitHub Actions

5. **Update Organization README**
   - Edit `docs/README_TEMPLATE.md` - add new package to table
   - Push to https://github.com/ForeverTools/ForeverTools

6. **Update Tracking Docs**
   - `docs/CURRENT_TASKS.md` - mark package as published
   - `docs/MASTER_PLAN.md` - update completed packages table
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
- [NuGet Publishing Docs](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
- [2Captcha API](https://2captcha.com/2captcha-api) - Use soft_id: 2482

---

*Last Updated: December 2024*
