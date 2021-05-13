# SynthRiders Custom Stages

Unity Project template that help make custom stages for [Synth Riders](https://synthridersvr.com/)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

Currenlty supported on Unity 2019.4.21f1. It is recomended the installation of [Unity Hub](https://store.unity.com/download?ref=personal/) for the ease of management of multiple versions of Unity

### Installing

Just clone the contents of this repository and open the project on Unity

## Deployment

Open the Demo Scene and make the changes to the Assets, once all the changes are made be sure to apply the changes to the StageTile prefabs. To export just go to the menu SynthRiders > Export Stage Bundle, you will be asked to set the Name of the Stage Bundle, once the name is set press Export.

When the export is completed just copy the file with the .stage extension and paste it into the CustomStages folder inside Synth Riders game, and the stage should appear in the stage selection menu.

All elements inside the _CustomStageElements folder will be exported, you can use all the non-code Assets (such as Models, Textures, Prefabs, Audio clips, etc), to build your custom stage.

Please preserve the size of the Thumb and BG image elements, for the correct display in the game.

Be aware that the names of the following items must not be changed:

    * MenuItems/BG
    * MenuItems/Thumb
    * Prefabs/StageTile01
    * Prefabs/StageTile02
    * Prefabs/StageTile03
    * Prefabs/Platform
    * Skybox/CustomSkybox
    * Prefabs/SpinEmitter

If you don't want to replace the default platform or default spin emitter, just delete o move the prefabs out of the _CustomStageElements folder

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/klugeinteractive/synth-riders-editor/tags). 

## Authors

* **Jhean Ceballos** - *Developer* - [shogoki-vnz](https://github.com/shogoki-vnz)
