# Sew Own Game (SOG)

![License](https://img.shields.io/badge/license-GPL--3.0-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)

**Stitching together your game development workflow**

Just as a tailor needs sewing tools to make final adjustments to their creation, game developers need Sew Own Game to fine-tune and polish their projects. SOG is an open-source suite of productivity tools designed to automate repetitive tasks and streamline your development process.

---

## 🎯 Project Proposal

Game development often involves repetitive and time-consuming tasks that could be automated. Sew Own Game is a standalone application that detects game engine installations and IDEs on your system, offering a centralized hub with various tools to:

- **Manage projects** more efficiently
- **Automate repetitive tasks** (setup, cleanup, optimization)
- **Generate boilerplate code** and templates
- **Analyze and optimize** existing projects
- **Facilitate integration** with Git and version control
- **Batch process assets** (textures, audio, models)

**SOG does NOT modify your engine** - it works with project files in a non-invasive way, functioning as a productivity layer over your traditional workflow.

### Current Focus & Future Plans

**Currently**, SOG focuses on **Unity development**, as it's one of the most popular game engines. However, we plan to **expand support to other game engines** (Unreal Engine, Godot, GameMaker, etc.) in the future with the help of community contributors.

If you're interested in adding support for other engines, we'd love your contribution! See our [Contributing Guidelines](CONTRIBUTING.md).

---

## ✨ Main Features

### 📂 Project Management
- Dashboard with all game projects on your system
- Quick launch (project + IDE with one click)
- Automatic Library/Temp folder cleanup
- Project backup and restore

### 🛠️ Productivity Tools
- Code generator (Singleton, Object Pool, State Machine, etc.)
- ScriptableObject templates
- Pre-configured project creator with folder structure
- Enhanced Package Manager

### 🔍 Analysis & Optimization
- Missing reference detector
- Unused asset finder
- Asset dependency analyzer
- Scene complexity reports

### 🎨 Asset Processing
- Batch texture conversion and compression
- 3D model optimization
- Audio file processing
- Sprite sheet generator

### 🔧 Utilities
- Enhanced Git integration (automatic LFS, .gitignore setup)
- Automatic IDE detection
- Quick access to important project folders
- Snippet manager for code reuse across projects

---

## 🚀 Technologies Used

- **UI Framework**: [Avalonia UI](https://avaloniaui.net/) - Cross-platform C# interface
- **Backend**: .NET 8+ (C#)
- **Parsing**: YamlDotNet, Newtonsoft.Json
- **Git Integration**: LibGit2Sharp
- **Persistence**: SQLite / JSON local storage

---

## 📦 Installation

### Requirements
- .NET 8.0 Runtime or higher
- Windows 7+, macOS 10.15+, or Linux (kernel 4.15+)
- Game engine installation (automatically detected)

### Download

**[📥 Download Latest Version](https://github.com/RagsProjects/Sew-Own-Game/releases/latest)**

### Run

#### Windows
1. Extract the `.zip` file
2. Run `SewOwnGame.exe`
3. If SmartScreen warning appears: click "More info" → "Run anyway"

#### macOS
```bash
# Grant execution permission
chmod +x SewOwnGame.app/Contents/MacOS/SewOwnGame
# Run
open SewOwnGame.app
```

#### Linux
```bash
chmod +x SewOwnGame
./SewOwnGame
```

---

## 🤝 Contributing

Contributions are welcome! This is an open-source project and your help is important to make it even better.

### How to Contribute

1. Fork the project
2. Create a branch for your feature (`git checkout -b feature/MyFeature`)
3. Commit your changes (`git commit -m 'Add MyFeature'`)
4. Push to the branch (`git push origin feature/MyFeature`)
5. Open a Pull Request

### Expanding to Other Engines

We're actively looking for contributors to help add support for:
- **Unreal Engine**
- **Godot**
- **GameMaker Studio**
- **Other popular engines**

If you have experience with any of these engines and would like to contribute, please check our [Contributing Guidelines](CONTRIBUTING.md) and open a discussion to coordinate efforts!

### Guidelines

- Follow established C# code standards
- Add tests when applicable
- Document new features in the README
- Keep commits clear and descriptive

---

## 📋 Roadmap

**Current Focus (Unity Support):**
- [x] Project dashboard with search and filters
- [ ] Code generator with customizable templates
- [ ] Plugin system for extensibility
- [ ] Enhanced Git integration
- [ ] Batch asset processing tools

**Future Expansion:**
- [ ] Unreal Engine support
- [ ] Godot support
- [ ] GameMaker Studio support
- [ ] Cross-engine project templates
- [ ] Universal asset optimization tools

See the [full issue list](https://github.com/RagsProjects/Sew-Own-Game/issues) for more details.

---

## 📄 License

This project is licensed under the **GNU General Public License v3.0** (GPL-3.0).

### What does this mean?

- ✅ **You can** use, modify, and distribute this software freely
- ✅ **You can** use it in commercial projects
- ✅ **You must** keep the source code open if you distribute modified versions
- ✅ **You must** use the same GPL-3.0 license on derivative works
- ✅ **You must** include copyright and license notices

For more details, see the [LICENSE](LICENSE) file or visit [GNU GPL v3.0](https://www.gnu.org/licenses/gpl-3.0.html).

### Why GPL-3.0?

We chose GPL-3.0 to ensure this project and all its derivatives remain **free and open** for the game development community. We want everyone to benefit and contribute with improvements.

---

## ⚠️ Legal Notice

**Sew Own Game is not affiliated with, associated with, authorized by, endorsed by, or in any way officially connected with Unity Technologies, Epic Games, Godot Engine, YoYo Games, or any other game engine company or their subsidiaries or affiliates.**

All product names, logos, and brands mentioned in this project are property of their respective owners. Use of these names does not imply endorsement.

This project is an **independent tool** created by the community to improve productivity in game development.

---

## 🐛 Report Bugs

Found a bug? [Open an issue](https://github.com/RagsProjects/Sew-Own-Game/issues/new) with:
- Problem description
- Steps to reproduce
- Operating system and .NET version
- Screenshots (if applicable)

---

## 💬 Community

- **Discussions**: [GitHub Discussions](https://github.com/RagsProjects/Sew-Own-Game/discussions)
- **Discord**: [Join the server](https://discord.gg/your-link) *(coming soon)*

---

## 🙏 Acknowledgments

- Game development community for inspiration and feedback
- [Avalonia UI](https://github.com/AvaloniaUI/Avalonia) project for the excellent framework
- All [contributors](https://github.com/RagsProjects/Sew-Own-Game/graphs/contributors) to this project

---

## 📊 Project Status

🚧 **In active development** - This project is in early stages. Features are being implemented continuously.

![GitHub Stars](https://img.shields.io/github/stars/RagsProjects/Sew-Own-Game?style=social)
![GitHub Forks](https://img.shields.io/github/forks/RagsProjects/Sew-Own-Game?style=social)
![GitHub Issues](https://img.shields.io/github/issues/RagsProjects/Sew-Own-Game)

---

**Made with ❤️ for the game development and open-source community**

*Fine-tune your game, stitch by stitch* 🪡🎮

