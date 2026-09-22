# Imperivm Syncrash

Syncrash is an experimental community project to improve the stability of **Imperivm RTC: HD Edition — Great Battles of Rome** on Steam. Its two lines of work are:

- **AntiCrasher v2:** a targeted guard against one reproduced class of native crashes.
- **Syncro:** investigation of multiplayer simulation desynchronization. There is no validated Syncro patch yet.

The intended result is one straightforward product for players, with each correction identified, tested and reversible. A combined release has **not** been built or validated. This project is not affiliated with the game's publisher or developer.

## Current status

AntiCrasher v2 protects three identified call sites where an incompatible object type could trigger an invalid virtual call. The mechanism has been reconstructed from crash evidence and tested in isolated scenarios. Successful play sessions are encouraging, but they do not establish that all crashes are fixed or count how often the guard intervened.

For Syncro, paired incident logs show different shaman actions on two players' computers. An uninitialized script local is a plausible cause for a subset of incidents, but its candidate fix has not been validated in a real multiplayer game. It is not included as a release.

See the [development roadmap](docs/ROADMAP.md) for the evidence needed before the two lines of work can be distributed together.

## Repository scope

This first commit contains project documentation. Local crash dumps, player logs, saves, complete game executables, game packages, third-party tools and experimental binaries are excluded. No patch should be installed from this repository yet.

The local research archive remains outside version control. Future source code and reproducible patch instructions can be added after review, without distributing the game's proprietary files or other players' data.
