Adrena-line solver
=================


A quick dumb project which automatically solves any level of [Daniel Linssen's](https://managore.itch.io/) game [ADRENA-LINE](https://managore.itch.io/adrena-line), where you are a square that can only travel in straight lines until you hit a wall.

Works by finding the game's window and then taking a snapshot of it, interpretting that snapshot to work out the level, and then running a bredth first search to find the quickest route. Fake keypresses are then sent to the game window.

Written in C#, uses WPF for the UI and a bunch of gross PInvoke methods to grab the screen and to send inputs to the game window. Will only work on Windows.