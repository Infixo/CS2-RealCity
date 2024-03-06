
# City Services Rebalance Mod
This mod allows to build a self-sufficient city in terms of resources production and consumption. Except for a single change in HouseholdBehaviorSystem, it achieves that by only modifying existing game parameters like companies profitability, resources price and base consumption or workplaces complexity. The mod's effects become more visible and useful the bigger the city is (>100k).

Version 0.3 introduces the config file where all modded params are kept and can be easily modified.

**Important note.**
Tuning and balancing is an iterative process. The feedback about how companies and economy behave is highly appreciated and welcomed. Please see the Support paragraph below on how to reach me. If you decide to try out the mod, please make sure to turn off other mods that affect economy, resource management and companies. While the mod technically will work as advertised, the results produced may be distorted and not very useful for further balancing.

### Key features.
 - (v0.3) Configuration in the xml file can be easily modified.
 
### Comparison to vanilla city
 - It is possible to turn on the mod on an existing city. The effects will be seen after few in-game hours, but it will take a few in-game days for all processes to fully adjust.

## Features

### Config file (v0.3)
  - Entire mod configuration is now kept in the xml config file that comes together with the mod.
  - The file is loaded when the game is started, so for new params to take effect you need to restart the game.
  - Note for the future. Please note that new mod versions will overwrite the file so if you did any changes and want to keep them - make a backup before update and then reapply to the updated version.

### Workplaces distribution
 - The structure of workfoce education does not match the workplaces distribution, by a huge factor.
 - In the vanilla game, the population eventually becomes well and highly educated, and there is not enough jobs for them. As a result, most of highly educated cims will be underemployed.
 - The mod "shifts" workplaces distribution to higher education levels. There will be much more demand for well and highly educated cims.
 - (v0.3) Industrial buildings have increased workplace capacity by 30%, office buildings by 20% and commercial buildings by 20%. Can be turned off in the config file.

## Technical

### Requirements and Compatibility
- Cities Skylines II version 1.0.19f1 or later; check GitHub or Discord if the mod is compatible with the latest game version.
- BepInEx 5.
- Modified systems: HouseholdBehaviorSystem.

### Installation
1. Place the `RealEco.dll` file in your BepInEx `Plugins` folder.

### Known Issues
- Nothing atm.

### Changelog
- v0.1.0 (2024-03-06)
  - Initial build.

### Support
- Please report bugs and issues on [GitHub](https://github.com/Infixo/CS2-RealEco).
- You may also leave comments on [Discord1](https://discord.com/channels/1169011184557637825/1207641575362920508) or [Discord2](https://discord.com/channels/1024242828114673724/1207641284647587922).

## Disclaimers and Notes

> [!NOTE]
The mod uses Cities: Skylines 2 Mod Template by Captain-Of-Coit.
