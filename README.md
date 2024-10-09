# Simple Setup for ECS 2D

![spritesheet animations](docs~/preview.gif)

This repo demonstrates an approach to Unity ECS in 2D that answers these questions:

- How to author and bake entity prefab for 2D character?
- How to store and use information of spritesheets globally?
- How to implement spritesheet animation logic in ECS?
- How to synchronize sprite renderers with entities?
- How to react to UI events in ECS?
- How to implement object-pool for sprite presenters?
- How to utilize [TransformAccessArray](https://docs.unity3d.com/ScriptReference/Jobs.TransformAccessArray.html) to update the transform of sprite presenters in parallel?

## Changelog

### Version 3.0.0

Add more functionality

- Spawn multiple characters
- Randomly set the animation for each character
- Automatically move characters
- Can change global move speed for all characters in real-time
- Destroy entities

![spritesheet animations](docs~/preview-2.gif)


## Credits

- GandalfHardcore - Pixel Art Character Pack: https://gandalfhardcore.itch.io/free-pixel-art
