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
- How to use Companion GameObject from Entities Hybrid?

## Changelog

### Version 4.0.1

- Correct subscene
- Remove the code to add `Prefab` tag to prefab entities at runtime

### Version 4.0.0

- Upgrade to Unity 6000.0.27
- Fix a compilation error caused by missing TMPro
- Add Entities Graphics and URP
- Add "Version 2" that use Companion GameObject from Entities Hybrid instead of GameObjectPool
- Make some changes to subscene and authoring to accomodate the Companion GameObject approach

### Version 3.0.0

Add more functionality

- Spawn multiple characters
- Randomly set the animation for each character
- Automatically move characters
- Can change global move speed for all characters in real-time
- Destroy entities

![spritesheet animations](docs~/preview-3.0.0.gif)

### Version 3.1.0

- Refactor some code
- Teleport characters to the opposite side of the screen if the went out of bounds

![spritesheet animations](docs~/preview-3.1.0.gif)

## Credits

- GandalfHardcore - Pixel Art Character Pack: https://gandalfhardcore.itch.io/free-pixel-art
