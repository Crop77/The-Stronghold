
---

### 🏰 `Stronghold Defense` – `README.md`

```md
# 🏰 Stronghold Defense

Genre: Strategy • Tower Defense / Auto Battler Hybrid  
Engine: Unity  
Role: Solo Designer & Developer

## 🎯 Overview

Stronghold Defense is a strategy game that mixes **tower defense**, **auto battler**, and **resource management**.  
Players place and upgrade units on tiles, manage their gold economy, and survive increasingly difficult waves of enemies.

## ⚙️ Core Systems

-AI for player and enemy units
  - Target selection and attack logic
  - Autonomous combat once units are placed

-Tile-based placement system
  - Defined PlacementSpots where units can be spawned
  - Prevents invalid placements and keeps layouts readable
  - Grid-like logic that supports clear strategy

-Unit upgrades
  - Units can be upgraded to become stronger at increasing gold costs
  - Upgrades significantly change stats and impact tactical decisions

-Economy & game loop
  - Core loop: **Survive waves → Earn gold → Buy/upgrade units**
  - Higher difficulty rewards more gold but requires better planning
  - Player choices in spending vs saving matter for long-term survival

-Config-driven wave design
  - Levels and waves defined via configuration / lists
  - New waves can be added by **inserting prefabs and setting stats**
  - Supports **endless levels** by expanding the config

## 🧩 Design Responsibilities

- Designed and implemented:
  - Unit AI and combat logic
  - Tile placement system
  - Gold-based **economy and upgrade loop**
  - Wave configuration for scalable difficulty
- Balanced multiple levels to keep the difficulty curve fair but challenging

## 🛠️ Tech & Tools

- Unity (C#)
- ScriptableObjects or config lists for wave data (if used)
- Basic UI for gold, health, and wave status
- Git & GitHub

## 📸 Screenshots
