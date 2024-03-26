
# City Services Rebalance Mod
The mod allows for virtually any changes to City Services buildings. At the moment, it mainly **increases number of workers in City Services buildings** and does other small adjustments (see below for details). It does so by changing Prefab parameters during the game start-up, so no changes to game systems are required. In the long term this can serve as a foundation for **tuning and balancing any parameter**. I encourage to contribute to the mod by raising issues and/or PRs on GitHub with proposed changes and rationale behind such changes.

Number of configured buildings and extensions: **197**.


## Features

### Version 0.1
- All buildings have more workplaces and some have a bit different requirements as for what education levels are required. There is no formula used, all have been set manually taking into consideration various aspects. Some have only slightly more workers, but some have many more. You need to take a look into Config.xml to find out about specific buildings.
- Crematorium has 2x higher processing rate to handle increased deaths from Population Rebalance.
- Post Sorting Facility have 2x higher sorting speed.

### Building configuration
- The configuration is kept in the `Config.xml` file that comes together with the mod. Please do not confuse with BepInEx `RealCity.cfg` config file.
- The file is loaded when the game is started, so for new params to take effect you need to restart the game.
- Note for the future. Please note that new mod versions will overwrite the Config.xml file so if you did any customizations and want to keep them - make a backup before update and then reapply to the updated version.

### Customizing the parameters
- As for now the mod allows to change any parameter that is stored as either an integer number (so Enums too) or a float number. This covers like 90% of params :)
- It allows to change a singular field in a specific component within a prefab.
- To apply your own changes, you need to know a) prefab's name - almost all are already in the Config.xml, so just find what you need b) component type c) field name and type.

### Turning the mod on/off on existing cities
- City Service buildings have some of their params set when plopped (e.g. workplaces). The new params will be applied to new buildings. You need to rebuild existing buildings in order to fully apply new params.
- Conversly, when the mod is deactivated, the buildings will retain their modded params. New buildings will have again vanilla params.


## Technical

### Requirements and Compatibility
- Cities Skylines II version 1.1.0f1 or later; check GitHub or Discord if the mod is compatible with the latest game version.
- BepInEx 5.
- The mod does NOT modify savefiles.
- The mod does NOT modify game systems.

### Installation
1. Place the `RealCity.dll` file in your BepInEx `Plugins` folder.
2. The BepInEx config file is automatically created in BepInEx\config when the game is run once.

### Known Issues
- Nothing atm.

### Changelog
- v0.2.0 (2024-03-20)
  - Mod updated for v1.1 of the game.
- v0.1.0 (2024-03-08)
  - Initial build.

### Support
- Please report bugs and issues on [GitHub](https://github.com/Infixo/CS2-RealCity).
- You may also leave comments on [Discord1](https://discord.com/channels/1169011184557637825/1215734718654451892) or [Discord2](https://discord.com/channels/1024242828114673724/1215735062428123196).

## Disclaimers and Notes

> [!NOTE]
The mod uses Cities: Skylines 2 Mod Template by Captain-Of-Coit.
