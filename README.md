# Warcraft 🛡
<p align="center">
  <img alt="Version" src="https://img.shields.io/github/v/tag/kyuuzou/warcraft?label=version" />
  <a href="https://github.com/kyuuzou/warcraft/issues" target="_blank">
     <img alt="GitHub issues" src ="https://img.shields.io/github/issues-raw/kyuuzou/warcraft" />
  </a>
  <a href="https://github.com/kyuuzou/warcraft/pulls" target="_blank">
   <img alt="GitHub pull requests" src ="https://img.shields.io/github/issues-pr-raw/kyuuzou/warcraft" />
  </a>
  <img alt="GitHub last commit" src ="https://img.shields.io/github/last-commit/kyuuzou/warcraft" />
</p>
<p align="center">
  <a href="https://www.codacy.com/gh/kyuuzou/warcraft/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=kyuuzou/warcraft&amp;utm_campaign=Badge_Grade" target="_blank"><img src="https://app.codacy.com/project/badge/Grade/7bc8986a48644f83b70de0a55fe29342"/></a>
  <a href="https://unity3d.com/get-unity/download/archive" target="_blank"><img src="https://img.shields.io/badge/Unity-2021.3-blue" /></a>
  <a href="https://github.com/kyuuzou/warcraft/blob/master/LICENSE" target="_blank">
    <img alt="License: MIT" src="https://img.shields.io/badge/License-MIT-blue.svg" />
  </a>
</p>

## Description
A clone of Blizzard's classic Warcraft: Orcs and Humans from 1994, made in Unity.

## Gameplay Video

[![Gameplay Video](https://github.com/kyuuzou/warcraft/blob/master/thumbnail.png)](https://www.youtube.com/watch?v=JBtrjr9go2c "Gameplay Video")

*Disclaimer: the art assets on the video are copyrighted to Blizzard Entertainment, and are not included on this repository.*

## Why it Exists

Sometime in 2012, I decided to create a pixel-perfect version of Warcraft: Orcs & Humans, both as a passion project and as a learning experience.

At some point in 2015, I neglected the project and ended up losing it.

In 2022 I finally got my hands on an few backups of the project, and got to work trying to restore it. This project is the result of that recovery process, and the ongoing upgrade from the original version in Unity 4.0.12f1.

## Getting Started
```
1. Open the Asset Importer (Window->Warcraft Asset Importer)
2. Set the path to DATA.WAR
3. Press "Import assets" to extract the Warcraft assets
4. Close Unity
5. Revert any git changes to restore potentially removed .meta files
6. Reopen Unity, and you're all set up!
```

## Version History

* *0.0.2*
    * Upgraded project from Unity 2018.3.12f1 to 2021.3.0f1
    * All units may now spawn
    * All instances of obsolete GUITexture and GUIText are now phased out

* *0.0.1*
    * Upgraded original project from Unity 4.0.12f1 to 2018.3.12f1
    * Restored unit movement, active combat and death
    * Units can once again build and mine
    * Fixed unit training

## Contacting the Author

* Email: newita@gmail.com
* Github: [@kyuuzou](https://github.com/kyuuzou)
* LinkedIn: [@nelson-rodrigues-ba4ab263](https://linkedin.com/in/nelson-rodrigues-ba4ab263)
* Portfolio: http://kyuandcaffeine.com

## License

Copyright © 2012 [Nelson Rodrigues](https://github.com/kyuuzou).<br />
This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

## Acknowledgments
Thank you [@Wargus](https://github.com/Wargus) for the excellent [asset extraction tools](https://github.com/Wargus/war1gus/blob/master/war1tool.cpp), without which this project would be much less fun to make.