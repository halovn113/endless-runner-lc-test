# Endless Runner - Game Test Submission

**Candidate Name:** TChau Kien Hung
**Date:** 23-24/7/2025

This is a very simple game, almost require Mechanics are done.

## How to Run the Project

1.  Unzip the project folder.
2.  Open the project using Unity Hub with Unity version `2022.3.28f1`.
3.  Open the `Assets/Scenes/Game.unity` file.
4.  Press the "Play" button in the editor.

## Features Implemented

- [x] Automatic player movement 
- [x] Side-to-side controls
- [x] Fixed side-scrolling camera
- [x] Random obstacle spawning
- [x] Player health and damage system
- [x] Shooting and destroying obstacles
- [x] Gun with ammo and reload mechanic
-  Ragdoll on player death (not yet because don't have enough time)
- [x] Increasing difficulty over time
- [x] UI for Health, Score, and Ammo

## Architectural Decisions

### Singleton Pattern
GameManager and ScoreManager. Although I want only GameManager to reduce using Singleton. ScoreManager in the other hand, I don't have time to change it before deadline.


### Observer Pattern
EventManager: I use it for most of events.
