# Contributing to Sew Own Game

First off, thank you for considering contributing to Sew Own Game! It's people like you that make SOG such a great tool for the game development community.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
  - [Reporting Bugs](#reporting-bugs)
  - [Suggesting Features](#suggesting-features)
  - [Adding Engine Support](#adding-engine-support)
  - [Pull Requests](#pull-requests)
- [Development Setup](#development-setup)
- [Coding Guidelines](#coding-guidelines)
- [Commit Message Guidelines](#commit-message-guidelines)
- [Project Structure](#project-structure)

---

## Code of Conduct

This project and everyone participating in it is governed by our commitment to providing a welcoming and inspiring community for all. By participating, you are expected to uphold this code.

**Be respectful, be constructive, be collaborative.**

Examples of behavior that contributes to a positive environment:
- Using welcoming and inclusive language
- Being respectful of differing viewpoints and experiences
- Gracefully accepting constructive criticism
- Focusing on what is best for the community
- Showing empathy towards other community members

Examples of unacceptable behavior:
- Trolling, insulting/derogatory comments, and personal or political attacks
- Public or private harassment
- Publishing others' private information without explicit permission
- Other conduct which could reasonably be considered inappropriate in a professional setting

---

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the [existing issues](https://github.com/RagsProjects/sew-own-game/issues) to avoid duplicates.

When creating a bug report, include as many details as possible:

**Bug Report Template:**
```markdown
**Describe the bug**
A clear and concise description of what the bug is.

**To Reproduce**
Steps to reproduce the behavior:
1. Go to '...'
2. Click on '...'
3. See error

**Expected behavior**
What you expected to happen.

**Screenshots**
If applicable, add screenshots.

**Environment:**
- OS: [e.g., Windows 11, macOS 14.1, Ubuntu 22.04]
- .NET Version: [e.g., 8.0.1]
- SOG Version: [e.g., 0.1.0]
- Game Engine: [e.g., Unity 2022.3.10f1, Unreal 5.3]

**Additional context**
Add any other context about the problem.
```

### Suggesting Features

We love to hear about new ideas! Before creating feature suggestions, check [existing feature requests](https://github.com/RagsProjects/sew-own-game/issues?q=is%3Aissue+label%3Aenhancement).

**Feature Request Template:**
```markdown
**Is your feature request related to a problem?**
A clear description of the problem. Ex. I'm always frustrated when [...]

**Target Engine (if applicable)**
Which game engine is this feature for? (Unity, Unreal, Godot, All, etc.)

**Describe the solution you'd like**
A clear and concise description of what you want to happen.

**Describe alternatives you've considered**
Other solutions or features you've considered.

**Additional context**
Screenshots, mockups, or examples.
```

### Adding Engine Support

We're actively looking to expand SOG to support multiple game engines! If you want to add support for a new engine:

**Steps to Add Engine Support:**

1. **Open a Discussion** first in [GitHub Discussions](https://github.com/RagsProjects/sew-own-game/discussions)
   - Announce your intention to add support for [Engine Name]
   - Discuss architecture and approach with maintainers
   - Coordinate to avoid duplicate efforts

2. **Research Engine Structure**
   - Project file formats
   - Configuration files
   - Asset organization
   - Build systems
   - Common pain points for developers

3. **Implement Core Features**
   - Engine detection (installation paths, versions)
   - Project detection and parsing
   - Basic project management (open, backup, clean)
   - Asset processing (if applicable)

4. **Follow the Plugin Architecture**
   - Create a new module under `src/SewOwnGame.Engines/[EngineName]/`
   - Implement the `IEngineSupport` interface
   - Add appropriate tests

5. **Document Everything**
   - Add engine-specific documentation
   - Update README with new engine support
   - Provide examples and screenshots

**Priority Engines:**
- 🔥 Unreal Engine
- 🔥 Godot
- 🔥 GameMaker Studio
- 🔥 Defold
- 🔥 Cocos2d-x

### Pull Requests

1. **Fork the repository** and create your branch from `main`
2. **Follow the coding guidelines** (see below)
3. **Test your changes** thoroughly
4. **Update documentation** if needed
5. **Submit a pull request**

**Pull Request Checklist:**
- [ ] Code follows the project's style guidelines
- [ ] Self-review completed
- [ ] Comments added to complex code sections
- [ ] Documentation updated (if applicable)
- [ ] No new warnings generated
- [ ] Tests added/updated (if applicable)
- [ ] All tests pass locally
- [ ] Engine-specific changes tested with actual engine projects

---

## Development Setup

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- IDE of your choice:
  - [JetBrains Rider](https://www.jetbrains.com/rider/) (recommended)
  - [Visual Studio 2022](https://visualstudio.microsoft.com/)
  - [Visual Studio Code](https://code.visualstudio.com/) with C# extension
- At least one game engine installed for testing (Unity, Unreal, Godot, etc.)

### Setup Steps

1. **Clone the repository**
```bash
   git clone https://github.com/RagsProjects/sew-own-game.git
   cd sew-own-game
```

2. **Install dependencies**
```bash
   dotnet restore
```

3. **Build the project**
```bash
   dotnet build
```

4. **Run the application**
```bash
   dotnet run --project src/SewOwnGame.UI
```

### Running Tests
```bash
dotnet test
```

---

## Coding Guidelines

### C# Style Guide

We follow standard C# coding conventions with some specific preferences:

**Naming Conventions:**
- `PascalCase` for class names, method names, properties, and public fields
- `camelCase` for private fields, parameters, and local variables
- Prefix private fields with underscore: `_myField`
- Use descriptive names: `projectRepository` not `pr`

**Code Style:**
```csharp
// Good
public class ProjectManager
{
    private readonly IProjectService _projectService;
    
    public async Task<Project> GetProjectAsync(string projectPath)
    {
        if (string.IsNullOrEmpty(projectPath))
        {
            throw new ArgumentException("Project path cannot be empty", nameof(projectPath));
        }
        
        return await _projectService.LoadProjectAsync(projectPath);
    }
}

// Avoid
public class project_manager
{
    IProjectService ps;
    
    public Task<Project> get(string p)
    {
        return ps.LoadProjectAsync(p);
    }
}
```

**General Principles:**
- Keep methods short and focused (Single Responsibility Principle)
- Use `async/await` for I/O operations
- Dispose of resources properly (use `using` statements)
- Avoid magic numbers - use named constants
- Comment complex logic, but prefer self-documenting code

**XAML Style:**
```xml
<!-- Good: Organized and readable -->
<Button Classes="primary"
        Content="Open Project"
        Command="{Binding OpenProjectCommand}"
        Margin="0,10,0,0" />

<!-- Avoid: Cramped and hard to read -->
<Button Classes="primary" Content="Open Project" Command="{Binding OpenProjectCommand}" Margin="0,10,0,0"/>
```

### Engine Support Architecture

When adding support for a new engine:
```csharp
// Implement the IEngineSupport interface
public interface IEngineSupport
{
    string EngineName { get; }
    string[] SupportedVersions { get; }
    
    Task<bool> IsEngineInstalledAsync();
    Task<IEnumerable<string>> DetectProjectsAsync();
    Task<Project> LoadProjectAsync(string path);
    Task<bool> ValidateProjectAsync(string path);
}

// Example implementation
public class UnrealEngineSupport : IEngineSupport
{
    public string EngineName => "Unreal Engine";
    public string[] SupportedVersions => new[] { "5.0", "5.1", "5.2", "5.3" };
    
    // Implementation details...
}
```

### File Organization
```
src/
├── SewOwnGame.Core/           # Business logic and services
│   ├── Models/                # Data models
│   ├── Services/              # Business services
│   └── Interfaces/            # Abstraction interfaces
├── SewOwnGame.Engines/        # Engine-specific implementations
│   ├── Unity/                 # Unity support
│   ├── Unreal/                # Unreal support (future)
│   ├── Godot/                 # Godot support (future)
│   └── Common/                # Shared engine utilities
├── SewOwnGame.UI/             # Avalonia UI project
│   ├── Views/                 # XAML views
│   ├── ViewModels/            # ViewModels (MVVM)
│   ├── Controls/              # Custom controls
│   └── Assets/                # Images, icons, etc.
└── SewOwnGame.Tests/          # Unit tests
    ├── Core.Tests/
    ├── Engines.Tests/
    └── Integration.Tests/
```

---

## Commit Message Guidelines

We follow [Conventional Commits](https://www.conventionalcommits.org/) specification:

**Format:**
```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, no logic change)
- `refactor`: Code refactoring
- `perf`: Performance improvements
- `test`: Adding or updating tests
- `chore`: Maintenance tasks, dependencies
- `engine`: Engine-specific changes

**Scopes (examples):**
- `unity`: Unity-specific changes
- `unreal`: Unreal-specific changes
- `godot`: Godot-specific changes
- `core`: Core functionality
- `ui`: User interface
- `git`: Git integration

**Examples:**
```
feat(unity): add quick launch functionality

Implements one-click project and IDE launch from dashboard.
Includes automatic IDE detection and preference saving.

Closes #42
```
```
engine(unreal): add initial Unreal Engine 5 support

Implements project detection, parsing .uproject files,
and basic project management features.

Related to #15
```
```
fix(asset-processor): resolve texture conversion error

Fixed issue where PNG to JPG conversion failed for images
larger than 4096x4096 pixels.

Fixes #87
```

---

## Project Structure
```
sew-own-game/
├── .github/                  # GitHub-specific files
│   ├── ISSUE_TEMPLATE/       # Issue templates
│   └── workflows/            # CI/CD workflows
├── docs/                     # Documentation
│   ├── engines/              # Engine-specific docs
│   │   ├── unity.md
│   │   ├── unreal.md
│   │   └── godot.md
│   └── development/          # Development guides
├── src/                      # Source code
│   ├── SewOwnGame.Core/      # Core business logic
│   ├── SewOwnGame.Engines/   # Engine implementations
│   ├── SewOwnGame.UI/        # UI layer (Avalonia)
│   └── SewOwnGame.Tests/     # Unit tests
├── assets/                   # Project assets (logos, screenshots)
├── .gitignore
├── CONTRIBUTING.md           # This file
├── LICENSE                   # GPL-3.0 license
└── README.md                 # Project readme
```

---

## Getting Help

- **Questions?** Open a [Discussion](https://github.com/RagsProjects/sew-own-game/discussions)
- **Bug or feature?** Open an [Issue](https://github.com/RagsProjects/sew-own-game/issues)
- **Want to add engine support?** Start a [Discussion](https://github.com/RagsProjects/sew-own-game/discussions) first!
- **Want to chat?** Join our [Discord](https://discord.gg/your-link) *(coming soon)*

---

## Recognition

Contributors will be recognized in:
- GitHub contributors page
- Release notes
- README acknowledgments section
- Special recognition for engine support contributors

---

## License

By contributing to Sew Own Game, you agree that your contributions will be licensed under the GNU General Public License v3.0.

---

**Thank you for contributing to Sew Own Game! 🪡🎮**

Together we're making game development more productive for everyone, across all engines!
