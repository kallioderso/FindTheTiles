# 🎮 Find the Tiles

A short and clever puzzle game inspired by Minesweeper — but with a twist.  
Your only goal: uncover a hidden pattern on the board. Each tile you click reveals part of the mystery — but beware! Too many wrong guesses, and the pattern slips away. No bombs, just pure deduction and intuition.

---

## 🧠 Gameplay Overview

- 🔍 Discover a hidden pattern on a grid of tiles.
- ❌ You only get **two missclicks** per round — choose wisely!
- ✅ Completing a pattern earns you points and generates a new challenge.
- 📊 Track your progress with a **pattern completion bar** and **fail counter**.
- 🧩 Patterns are procedurally generated using a smart path-construction AI.

---

## 🧬 Pattern Generation

The game uses a custom algorithm to create interesting and challenging patterns:

- A random **starting point** is selected — with a higher chance of being near the center.
- Adjacent tiles are added based on a **base 25% chance**, reduced by:
  - Nearby existing pattern tiles
  - Proximity to the grid’s outer edges

This results in clean, well-spread patterns that are fun to uncover and hard to guess.

---

## 🛍️ Upgrade System (Coming Soon)

Instead of a leveling system, players earn **coins** through scoring.  
These coins will be used to buy upgrades like:
- Extra missclick chances
- Increased difficulty
- Rogue-like modifiers for replayability

---

## 🎓 Interactive Tutorial

New players are guided through a **hands-on tutorial round**, where each mechanic is introduced step-by-step. No walls of text — just intuitive learning through play.

---

## 🚀 Try It Out

- [Download the demo](https://github.com/kallioderso/FindTheTiles/releases/tag/demo)
- [View the source code](https://github.com/kallioderso/FindTheTiles)

---

## 🤖 About This Project

This is my first .NET MAUI application, and I’ve learned a lot along the way.  
The devlogs were written with the help of AI, because my English isn’t perfect yet — but I’m passionate about creating quality content and sharing my work beyond language barriers.

Thanks for checking out *Find the Tiles* — more updates coming soon!
