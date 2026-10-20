# The Dreamweaver’s Maze

A virtual reality adventure developed with Unity and C# for EPFL’s CS-444 Virtual Reality course, Spring 2024 (Group 10, Professor Ronan Boulic).

Play as a wizard trapped in enchanted worlds: escape a dungeon, gather ingredients in a magical garden, brew a teleportation potion and navigate a minecart through a crystal mine. The experience combines physical object interactions, puzzles, magic and locomotion.

**This repository is a gameplay code showcase.** It does not contain the complete Unity project, scenes, models, textures, audio or Oculus SDK. It cannot be opened and played as a standalone Unity project.

## Gameplay and code

- Grab, release and throw objects: `HandController`, `GrabbableObject`, `ThrowableObject`.
- Move through the environment using teleportation: `ControllerTeleport`.
- Collect ingredients and brew a potion: `BasketController`, `CauldronController`, `PotionManager`.
- Use a magic wand and fireballs: `WandController`, `WandObject`.
- Interact with puzzles and environmental hazards: `PuzzleController`, `PendulumController`, `LavaCollision`.
- Provide audio and haptic feedback: `SoundManager`, `HapticsController`.

The original build scene order is: tutorial → prison cell → magic garden → final cave → final scene.

## Technology

- Unity **2020.2.2f1**
- C#
- Oculus integration / `OVRInput`
- Unity XR Management **4.4.1** and Oculus XR Plugin **1.6.1**

Versions are recorded from the delivered project. Compatibility with current Unity or headset software has not been tested.

## Source version

The scripts come from the May 22, 2024 submission folder, alongside the original report, Android APK and demonstration video. Script contents are preserved unchanged; Unity `.meta` files are retained.

The later timestamp on `VR final/VR/Project/Assets/Scripts/CauldronController.cs` corresponds only to removal of an unused field and formatting changes. No behavior change was found, so the submitted version is retained.

## Dependencies and reconstruction

These scripts depend on the original scene setup, serialized Inspector references, prefabs, colliders, layers, tags, Oculus integration and third-party components. `Outline.cs` (QuickOutline, Chris Nolet) is deliberately excluded as third-party code; scripts using `Outline` require that dependency.

See [third-party inventory](docs/THIRD_PARTY.md) for omitted resources. To run the complete game, use the original delivered Unity project and its licensed dependencies. This repository alone is insufficient to reconstruct it.

## My contribution — Mylène Berruyer

As documented in the final project report, I worked on:

- **Object throwing:** grabbing, releasing and throwing objects, with audio and haptic feedback.
- **Container interactions:** collecting and transporting objects in a basket, including feedback.
- **Magic Garden:** designing the scene and its magical atmosphere.
- **Potion crafting:** displaying the recipe when the player approaches, handling correct and incorrect ingredients in the cauldron, and transitioning to the next scene when the potion is consumed.
- **Object respawning.**

Relevant scripts include `HandController.cs`, `ThrowableObject.cs`, `BasketController.cs`, `NoteAppear.cs`, `CauldronController.cs` and `PotionManager.cs`. This is a guide to the related systems, not a claim of exclusive authorship of each file.

## Team

- **Mylène Berruyer:** throwing and container interactions; Magic Garden and potion mechanics.
- **Nicolas Francio:** door joint interaction; Final Cave, minecart course and hazards; final transition and debugging support.
- **Solène Noize:** locomotion methods; tutorial and final scene; scene integration and interaction fixes.
- **Pierre Stoeber:** dungeon, puzzle and magic wand.

Credits and contribution summaries are based on the final report, section 6. This repository contains group code; individual work is identified above.

## Demo

Watch the gameplay demonstration: [The Dreamweaver’s Maze — VR Demo](https://youtu.be/7fKR0kQXZP4).

## Rights

No open-source license has been selected for the group code. Third-party resources retain their respective terms and are not included here.
