# Current Tasks Checklist

This document tracks what needs to be done before publishing each package.

---

## BEFORE FIRST PUBLISH (One-Time Setup)

### Account Setup
- [x] Create NuGet.org account: https://www.nuget.org/users/account/LogOn
  - **Username:** ForeverTools
  - **Profile:** https://www.nuget.org/profiles/ForeverTools
- [x] Generate NuGet API key (see DISTRIBUTION_GUIDE.md)
  - **Glob Pattern:** `ForeverTools.*`
  - **Expires:** December 2025
- [ ] Store API key in environment variable `NUGET_API_KEY`
- [x] Create GitHub account (if not already)
  - **Username:** ForeverTools
  - **Main repo:** https://github.com/ForeverTools/ForeverTools

### Affiliate Account Registration
Register for each service's affiliate program BEFORE publishing:

| Service | Affiliate Signup URL | Code | Status |
|---------|---------------------|------|--------|
| AI/ML API | https://aimlapi.getrewardful.com/signup | `?via=forevertools` | [x] Registered |
| 2Captcha | https://2captcha.com/affiliate | `soft_id: 2482` | [x] Registered |
| Anti-Captcha | https://anti-captcha.com/clients/tools/devcenter | - | [ ] Registered |
| CapSolver | https://dashboard.capsolver.com/dashboard/affiliate | - | [ ] Registered |
| BrightData | https://brightdata.com/affiliate | - | [ ] Registered |
| ScraperAPI | https://www.scraperapi.com/affiliates/ | - | [ ] Registered |
| SmartProxy | https://smartproxy.com/affiliate | - | [ ] Registered |
| APILayer | https://apilayer.com/affiliates/ | - | [ ] Registered |
| Postmark | https://postmarkapp.com/lp/referral-partner-program | - | [ ] Registered |
| BulkGate | https://www.bulkgate.com/en/partners/ | - | [ ] Registered |

---

## Package #1: ForeverTools.AIML

### Affiliate Setup
- [x] Register at: https://aimlapi.getrewardful.com/signup
- [x] Affiliate code: `?via=forevertools` (already embedded in code)
- [x] All affiliate links updated

### Affiliate Link Locations (for reference)

**Code:** `?via=forevertools` (CONFIRMED CORRECT)

| File | Line(s) | Current | Action |
|------|---------|---------|--------|
| `README.md` | Multiple | `aimlapi.com?via=forevertools` | Find & Replace all |
| `src/ForeverTools.AIML/AimlApiOptions.cs` | ~15 | `aimlapi.com?via=forevertools` | Update comment |
| `src/ForeverTools.AIML/AimlApiClient.cs` | ~24, 35, 173 | `aimlapi.com?via=forevertools` | Update comments |
| `src/ForeverTools.AIML/Models/AimlModels.cs` | ~8 | `aimlapi.com?via=forevertools` | Update comment |
| `src/ForeverTools.AIML/Extensions/ServiceCollectionExtensions.cs` | ~15 | `aimlapi.com?via=forevertools` | Update comment |

**How to update all at once (PowerShell):**
```powershell
cd C:\xampp2\htdocs\git_projects\ForeverTools
$files = Get-ChildItem -Recurse -Include *.cs,*.md | Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\' }
foreach ($file in $files) {
    (Get-Content $file.FullName) -replace '\?via=forevertools', '?via=YOUR_ACTUAL_CODE' | Set-Content $file.FullName
}
```

### Pre-Publish Checklist
- [x] Package builds without errors (`dotnet build -c Release`)
- [x] Tests pass (run `dotnet test`)
- [x] Package packs successfully (`dotnet pack -c Release`)
- [ ] Affiliate account registered
- [ ] Affiliate code updated in all files
- [ ] Icon converted from SVG to PNG (128x128)
- [ ] README looks correct (open in VS Code preview)
- [ ] Version number is correct in .csproj

### Publish Steps
```powershell
cd C:\xampp2\htdocs\git_projects\ForeverTools
dotnet pack src/ForeverTools.AIML/ForeverTools.AIML.csproj -c Release -o ./packages
dotnet nuget push packages/ForeverTools.AIML.1.0.0.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

### Post-Publish Verification
- [ ] Package visible at https://www.nuget.org/packages/ForeverTools.AIML/
- [ ] README displays correctly
- [ ] Can install: `dotnet add package ForeverTools.AIML`
- [ ] IntelliSense works in test project

---

## Important Notes on Affiliate Updates

### Q: What happens if I change my affiliate code after publishing?

**For Link-Based Affiliates (like AI/ML API):**
- The links are in README.md and XML comments
- Users who already downloaded v1.0.0 have the OLD README in their local cache
- **BUT** the NuGet.org package page shows your UPDATED README
- New users see updated links, existing users keep old version
- **Impact:** Minimal - most signups come from NuGet page, not cached README

**For API-Embedded Affiliates (like Captcha packages):**
- The affiliate ID is compiled INTO the DLL
- Users who downloaded v1.0.0 will ALWAYS use the affiliate ID from that version
- To change it, you must publish v1.0.1
- **Impact:** Higher - you'd need users to update their package

### Q: Should I worry about this?

**No, because:**
1. You're setting up affiliate accounts ONCE, before first publish
2. Your affiliate ID won't change
3. If a service changes your ID, publish a new version

### Q: Can I pre-register affiliate accounts before building packages?

**Yes! Recommended approach:**
1. Register for ALL affiliate programs now (see list above)
2. Note down all your affiliate codes
3. Update packages before publishing

---

## Package Status Tracker

| Package | Built | Tested | Affiliate | Published | NuGet URL |
|---------|-------|--------|-----------|-----------|-----------|
| ForeverTools.AIML | Yes | Yes | Pending | No | - |
| ForeverTools.Captcha | No | No | No | No | - |
| ForeverTools.ScraperAPI | No | No | No | No | - |
| ForeverTools.BrightData | No | No | No | No | - |
| ForeverTools.APILayer | No | No | No | No | - |

---

## Quick Commands Reference

```powershell
# Build all
dotnet build -c Release

# Run tests
dotnet test

# Pack specific package
dotnet pack src/ForeverTools.AIML/ForeverTools.AIML.csproj -c Release -o ./packages

# Publish to NuGet
dotnet nuget push packages/ForeverTools.AIML.1.0.0.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json

# Find and replace affiliate code in all files
$files = Get-ChildItem -Recurse -Include *.cs,*.md | Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\' }
foreach ($file in $files) {
    (Get-Content $file.FullName) -replace 'OLD_CODE', 'NEW_CODE' | Set-Content $file.FullName
}
```

---

*Last Updated: December 2024*
