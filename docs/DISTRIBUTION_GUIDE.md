# NuGet Distribution Guide

Step-by-step instructions for publishing ForeverTools packages to NuGet.org.

---

## Prerequisites

### 1. Create a NuGet.org Account

1. Go to https://www.nuget.org/users/account/LogOn
2. Sign in with Microsoft account (or create one)
3. Verify your email address

### 2. Create an API Key

1. Go to https://www.nuget.org/account/apikeys
2. Click "Create"
3. Configure:
   - **Key Name:** `ForeverTools-Publish` (or any name)
   - **Expiration:** 365 days (maximum)
   - **Package Owner:** Your username
   - **Glob Pattern:** `ForeverTools.*` (restricts key to your packages)
   - **Scopes:** Select "Push" and "Push new packages and package versions"
4. Click "Create"
5. **IMPORTANT:** Copy the API key immediately - you won't see it again!

### 3. Store Your API Key Securely

Option A: Environment variable (recommended)
```powershell
# Windows PowerShell (run as admin for permanent)
[Environment]::SetEnvironmentVariable("NUGET_API_KEY", "your-key-here", "User")

# Or temporary for current session
$env:NUGET_API_KEY = "your-key-here"
```

Option B: Store in a secure file
```
C:\Users\YourName\.nuget\api-key.txt
```

---

## Publishing a Package

### Step 0: Create Package Icon (if new package)

Each package needs a 128x128 PNG icon. Use the SVG→PNG converter:

```powershell
# Create your icon as SVG in assets/ folder, then convert:
dotnet run --project tools/SvgToPng -- "assets/icon.svg" "assets/icon.png" 128
```

### Step 1: Build the Package

```powershell
cd C:\xampp2\htdocs\git_projects\ForeverTools

# Build in Release mode
dotnet build -c Release

# Create the NuGet package
dotnet pack src/ForeverTools.AIML/ForeverTools.AIML.csproj -c Release -o ./packages
```

This creates:
- `packages/ForeverTools.AIML.1.0.0.nupkg` - The package
- `packages/ForeverTools.AIML.1.0.0.snupkg` - Debug symbols (optional to publish)

### Step 2: Verify the Package (Optional but Recommended)

```powershell
# List package contents
dotnet nuget verify packages/ForeverTools.AIML.1.0.0.nupkg

# Or use NuGet Package Explorer (GUI tool)
# Download: https://github.com/NuGetPackageExplorer/NuGetPackageExplorer
```

### Step 3: Push to NuGet.org

```powershell
# Using environment variable
dotnet nuget push packages/ForeverTools.AIML.1.0.0.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json

# Or with key directly (less secure)
dotnet nuget push packages/ForeverTools.AIML.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# Also push symbols (optional, for debugging support)
dotnet nuget push packages/ForeverTools.AIML.1.0.0.snupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

### Step 4: Wait for Indexing

- Package appears immediately at: `https://www.nuget.org/packages/ForeverTools.AIML/`
- **BUT** it takes 15-30 minutes before it's searchable
- Full indexing can take up to a few hours

### Step 5: Verify Publication

1. Go to https://www.nuget.org/packages/ForeverTools.AIML/
2. Check that README displays correctly
3. Verify all metadata (description, tags, license)
4. Test installation:
   ```powershell
   dotnet new console -n TestInstall
   cd TestInstall
   dotnet add package ForeverTools.AIML
   ```

---

## Publishing Updates

### Increment Version Number

Edit `ForeverTools.AIML.csproj`:
```xml
<Version>1.0.1</Version>  <!-- Was 1.0.0 -->
```

### Versioning Strategy

| Change Type | Version Bump | Example |
|-------------|--------------|---------|
| Bug fix | Patch (1.0.X) | 1.0.0 → 1.0.1 |
| New feature (backward compatible) | Minor (1.X.0) | 1.0.1 → 1.1.0 |
| Breaking change | Major (X.0.0) | 1.1.0 → 2.0.0 |

### Build and Push Update

```powershell
dotnet pack src/ForeverTools.AIML/ForeverTools.AIML.csproj -c Release -o ./packages
dotnet nuget push packages/ForeverTools.AIML.1.0.1.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

---

## Unlisting a Package (Soft Delete)

If you need to hide a version (but not delete):

```powershell
dotnet nuget delete ForeverTools.AIML 1.0.0 --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

**Note:** This only unlists - existing users can still restore it. NuGet doesn't allow true deletion.

---

## Alternative Registries

### GitHub Packages

1. Create Personal Access Token with `write:packages` scope
2. Add NuGet source:
   ```powershell
   dotnet nuget add source https://nuget.pkg.github.com/YOUR_USERNAME/index.json -n github -u YOUR_USERNAME -p YOUR_TOKEN
   ```
3. Push:
   ```powershell
   dotnet nuget push packages/ForeverTools.AIML.1.0.0.nupkg --source github
   ```

### MyGet (Private feeds)

Useful for testing before public release.

1. Create account at https://myget.org
2. Create a feed
3. Push to your feed URL

---

## Batch Publishing Script

For publishing multiple packages at once:

```powershell
# publish-all.ps1
$packages = Get-ChildItem -Path "./packages" -Filter "*.nupkg" | Where-Object { $_.Name -notlike "*.snupkg" }

foreach ($pkg in $packages) {
    Write-Host "Publishing $($pkg.Name)..."
    dotnet nuget push $pkg.FullName --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
}
```

The `--skip-duplicate` flag prevents errors if package version already exists.

---

## Troubleshooting

### "API key is invalid"
- Check key hasn't expired
- Verify glob pattern matches package name
- Regenerate key if needed

### "Package already exists"
- You cannot overwrite existing versions
- Increment version number and republish

### "README not displaying"
- Ensure README.md is included in package (check csproj)
- Markdown must be valid
- Wait for indexing (can take hours)

### "Package not showing in search"
- Wait 30+ minutes for indexing
- Check package wasn't unlisted
- Verify tags and description contain search terms

---

## Post-Publish: Update GitHub Organization README

After publishing a new package to NuGet, update the ForeverTools GitHub organization README:

### 1. Edit `docs/README_TEMPLATE.md`

Add the new package to the packages table:

```markdown
| Package | Description | NuGet |
|---------|-------------|-------|
| [ForeverTools.AIML](...) | Access 400+ AI models | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| [ForeverTools.NewPackage](...) | Description here | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.NewPackage.svg)](https://www.nuget.org/packages/ForeverTools.NewPackage) |
```

### 2. Push to GitHub Organization Repo

```powershell
# Copy README_TEMPLATE.md content to the GitHub org repo
# Option A: Manual - copy content to https://github.com/ForeverTools/ForeverTools/README.md

# Option B: If ForeverTools local folder is linked to GitHub
cd C:\xampp2\htdocs\git_projects\ForeverTools
git add docs/README_TEMPLATE.md
git commit -m "Add ForeverTools.{PackageName} to package list"
git push
```

### 3. Update Tracking Documents

- Mark package as published in `docs/CURRENT_TASKS.md`
- Update `docs/MASTER_PLAN.md` completed packages table
- Add entry to `CHANGELOG.md`

---

## Quick Reference

```powershell
# Build and pack
dotnet pack src/ForeverTools.AIML/ForeverTools.AIML.csproj -c Release -o ./packages

# Push to NuGet
dotnet nuget push packages/ForeverTools.AIML.1.0.0.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json

# Check package info
dotnet nuget locals all --list

# Clear local cache (if testing updates)
dotnet nuget locals all --clear
```

---

*Last Updated: December 2024*
