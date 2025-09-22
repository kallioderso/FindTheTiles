# Find the Tiles

Find the Tiles is a short little Game where you have to uncover a hidden pattern inside a 7 x 7 Grid, to get a easy start you have 2 start tiles which are 100% inside the pattern, later you can improve this start tiles by collecting Tile-Coins and using them inside the Shop area. while playing there are some specifix rules you should look at such as limited fails before loosing a game session. for more informations click here
---
## Multiple Languages
Using the language change button it is possible to chose beetwheen 3 different languages (currently)
- English
- German
- Frensh

On basic the chosen language would be English according to the fact that most people understand English

---
## Main-menu

the main menu is where your experience will start (after the tutorial), here you have much different options and informations.
- the first thing that is relevant for you is the menu-bar where you can find the tutorial, the shop and language setting (***1***)
- next we have your most favorite button, the start button, with it you can start your sessions of pure enjoyment (***2***)
- the next important button would be the continue game button. any unfinished sessions? just click on it and you can continue them (***3***)
- but not only buttons are here, you can as well see your general and last progress as well as your earned shop coins. and the difficulty (what will it be???) (***4***)
![Main-Menu](images/mainmenu.png)

---
## Game-sessions

- start with 2 green bordered Tiles which refers to the hidden pattern
- get hints where the next pattern tiles could be by clicking on tiles and scan the attached 4 tiles (left, up, right, down) for tiles beeing part of the pattern (all scaned tiles are marked in a yellow color). the amount of scaned tiles which are part of pattern is shown inside the clicked tile with a blakc number.
- by uncovering the pattern you will automaticly increase a progressbar to see how much of the Pattern is stil missing.
- if you click to much times on non Pattern tiles your failure count will reach zero and you instantly loose the current game session.
- you have a main 7x7 tile-grid (***1***)
- theres a spcial menu-bar on the left bottom, containing tutorial-button and language-button (***2***) 
- you can use special items obtained in the Tile-Coins Shop (***3***)
- in the left middle of the screen you have a generall Information Field with much information about the current game session, such as the points multiplier, score, progress and more (***4***)
- to leave the current game session you can just click on the exit button (***5***)
![Game Preview](images/gamescreen.png)

---

## 🧬 Pattern Generation

The game uses a custom algorithm to create interesting and challenging patterns:

- A random **starting point** is selected — with a higher chance of being near the center.
- Adjacent tiles are added based on a **base 25% chance**, reduced by:
  - Nearby existing pattern tiles (1 = **-0%**, 2 = **-5%**, 3 = **-10%**, 4 = **-15%**)
  - Proximity to the grid’s outer edges (same as the starting point uses)

This results in clean, well-spread patterns that are fun to uncover and hard to guess.

in the future there will be more difficult Generating methods such as by neural networks or special trained ai (but currently i need to continue learning such methods because i want to keep everything in c#)

---

## 🛍️ Tile-Coins Shop

Instead of a leveling system, players earn **tile-coins** through scoring.  
These coins will be used to buy upgrades like:
- Extra missclick chances
- Increased difficulty
- Rogue-like modifiers for replayability
- special items (non reusable)

the shop laterly will have a option to buy more different types of level and designs to customise the user experiences
---

## 🎓 Interactive Tutorial

New players can learn the basic mechanics through our interactiv and self explaining **Tutorial**, which can be skipped if already known by clicking on the text boxes, they still need to be improved in therms of user friendly explanations and more as well as less time beetwheen the different text boxes, another cool thing would be some sort of mascotchen to explain everything

---

## 🚀 Try It Out

- [Download the demo](https://github.com/kallioderso/FindTheTiles/releases/tag/demo)
- [View the source code](https://github.com/kallioderso/FindTheTiles)

---

## 🤖 About This Project

This is my first complexer .NET MAUI application, and I’ve learned a lot along the way.  
The devlogs were written with the help of AI, because my English isn’t perfect yet — but I’m passionate about creating quality content and sharing my work beyond language barriers.

Thanks for checking out *Find the Tiles* — more updates coming soon!
