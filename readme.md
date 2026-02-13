# Open Unity

![License](https://img.shields.io/badge/license-GPL--3.0-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)

**Productivity tools for Unity developers**

Open Unity is a set of open-source tools that aims to increase the productivity of Unity developers through automation, utilities, and features that facilitate common everyday tasks.

---

## 🎯 Project Proposal

Development in Unity often involves repetitive and time-consuming tasks that could be automated. Open Unity is a standalone application that detects Unity installations and IDEs on your system, offering a centralized hub with several tools to:

- **Manage Unity projects** more efficiently
- **Automate repetitive tasks** (configuration, cleanup, optimization)
- **Generate boilerplate code** and templates
- **Analyze and optimize** existing projects
- **Facilitate integration** with Git and version control
- **Batch process assets** (textures, audio, models)

**Open Unity does NOT modify Unity itself**—it works with project files in a non-invasive way, functioning as a productivity layer on top of the traditional workflow.

---

## ✨ Key Features

### 📂 Project Management
- Dashboard with all Unity projects in the system
- Quick launch (project + IDE with one click)
- Automatic cleaning of Library/Temp folders
- Project backup and restore

### 🛠️ Productivity Tools
- Code generator (Singleton, Object Pool, State Machine, etc.)
- ScriptableObjects templates
- Project creator with preconfigured structure
- Improved Package Manager

### 🔍 Analysis and Optimization
- Missing references detector
- Unused asset locator
- Asset dependency analyzer
- Scene complexity report

### 🎨 Asset Processing
- Batch texture conversion and compression
- 3D model optimization
- Audio file processing
- Sprite sheet generator

### 🔧 Utilities
- Improved Git integration (automatic LFS configuration, .gitignore)
- Automatic detection of installed IDEs
- Quick access to important project folders
- Snippet manager for reusing code between projects

---

## 🚀 Technologies Used

- **UI Framework**: [Avalonia UI](https://avaloniaui.net/) - Cross-platform interface in C#
- **Backend**: .NET 8+ (C#)
- **Parsing**: YamlDotNet, Newtonsoft.Json
- **Git Integration**: LibGit2Sharp
- **Persistence**: SQLite / Local JSON

---

## 📦 Installation

### Requirements
- .NET 8.0 Runtime or higher
- Windows 7+, macOS 10.15+, or Linux (kernel 4.15+)
- Unity Hub or standalone Unity installation (detected automatically)

### Download

**[📥 Download latest version](https://github.com/RagsProjects/OpenUnity/releases/latest)**

### Run

#### Windows
1. Extract the `.zip` file
2. Run `OpenUnity.exe`
3. If a SmartScreen warning appears: click “More info” → “Run anyway”

#### macOS
```bash
# Give execution permission
chmod +x OpenUnity.app/Contents/MacOS/OpenUnity
# Run
open OpenUnity.app
```

#### Linux
```bash
chmod +x OpenUnity
./OpenUnity
```

---

## 🤝 Contributing

Contributions are welcome! This is an open-source project, and your help is important to make it even better.

### How to Contribute

1. Fork the project
2. Create a branch for your feature (`git checkout -b feature/MyFeature`)
3. Commit your changes (`git commit -m ‘Add MyFeature’)
4. Push to the branch (`git push origin feature/MyFeature`)
5. Open a Pull Request

### Guidelines

- Follow established C# coding standards
- Add tests when applicable
- Document new features in the README
- Keep commits clear and descriptive

---

## 📋 Roadmap

- [ ] Project dashboard with search and filters
- [ ] Code generator with customizable templates
- [ ] Plugin system for extensibility
- [ ] Integration with Asset Store
- [ ] Unity Cloud Build support
- [ ] Build performance analysis
- [ ] Merge tool for .unity files (scenes)

See the [complete list of issues](https://github.com/RagsProjects/OpenUnity/issues) for more details.

---

## 📄 License

This project is licensed under the **GNU General Public License v3.0** (GPL-3.0).

### What does this mean?

- ✅ **You may** use, modify, and distribute this software freely
- ✅ **You may** use it in commercial projects
- ✅ **You must** keep the source code open if you distribute modified versions
- ✅ **You must** use the same GPL-3.0 license in derivative versions
- ✅ **You must** include the copyright and license notice

For more details, see the [LICENSE](LICENSE) file or visit [GNU GPL v3.0](https://www.gnu.org/licenses/gpl-3.0.html).

### Why GPL-3.0?

We chose GPL-3.0 to ensure that this project and all its derivatives remain **free and open** to the Unity developer community. We want everyone to benefit and contribute improvements.

---

## ⚠️ Legal Notice

**Open Unity is not affiliated, associated, authorized, endorsed by, or in any way officially connected with Unity Technologies or any of its subsidiaries or affiliates.**

The names “Unity,” “Unity Engine,” and related logos are trademarks of Unity Technologies.

This project is an **independent** tool created by the community to improve productivity in Unity development.

---

## 🐛 Report Bugs

Found a bug? [Open an issue](https://github.com/RagsProjects/OpenUnity/issues/new) with:
- Description of the problem
- Steps to reproduce
- Operating system and .NET version
- Screenshots (if applicable) 

---

## 💬 Community

- **Discussions**: [GitHub Discussions](https://github.com/RagsProjects/OpenUnity/discussions)
- **Discord**: [Join the server](https://discord.gg/seu-link) *(coming soon)*

---

## 🙏 Acknowledgments

- Unity community for inspiration and feedback
- [Avalonia UI](https://github.com/AvaloniaUI/Avalonia) project for the excellent framework
- All [contributors](https://github.com/RagsProjects/OpenUnity/graphs/contributors) to this project

---

## 📊 Project Status

🚧 **In active development** - This project is in its early stages. Features are being implemented continuously.

![GitHub Stars](https://img.shields.io/github/stars/RagsProjects/OpenUnity?style=social)
![GitHub Forks](https://img.shields.io/github/forks/RagsProjects/OpenUnity?style=social)
![GitHub Issues](https://img.shields.io/github/issues/RagsProjects/OpenUnity)

---

**Made with ❤️ for the Unity and open-source community**