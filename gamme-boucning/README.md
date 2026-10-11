# Park Physics Environment

A complete Unity 6 project for the photographed park and physics assignment.

Open this folder in Unity 6000.6.1f1, then open `Assets/Scenes/ParkPhysicsEnvironment.unity` and press **Play**. Both balls fall under gravity and bounce. Use **Drop balls again** to repeat the experiment and **Pause** to inspect it.

## Assignment requirements

| Requirement | Implementation |
| --- | --- |
| Ground Plane | 18 × 14 grass plane with a Mesh Collider |
| At least three cubes | Two benches built from four cubes each, plus walking paths |
| Two spheres | Orange and blue dynamic balls (additional spheres form tree canopies) |
| One cylinder | Four cylinder tree trunks |
| Move, Rotate, Scale | Positioned trees and balls, rotated benches/backrests, scaled seats, paths and trunks |
| Different materials | Separate grass, stone, wood, iron, bark, leaves and ball materials |
| Two Rigidbody components | Orange sphere: 1 kg; blue sphere: 3 kg; Use Gravity enabled on both |
| Appropriate colliders | Mesh Collider on ground, Box Colliders on cubes, Capsule Colliders on trunks, Sphere Colliders on spheres |
| Bouncing effect | Orange Physics Material: 0.86 bounciness; blue: 0.45; Maximum bounce combine |
| Ground collision | Both balls land directly on the grass plane; continuous collision detection enabled |
| Clear camera view | Orthographic camera frames the entire park |
| Organized hierarchy | Named groups for environment, benches, trees, physics objects and controls |
| Saved scene | `Assets/Scenes/ParkPhysicsEnvironment.unity` |

The orange sphere starts at Y = 5; the blue sphere at Y = 6. Their different starting heights and bounciness make the two experiments easy to see. The physics engine supplies the bounce; the scripts only show controls and count impacts.

Use **Park Physics → Validate Saved Scene** outside Play mode to repeat the 12-second physics check. The result is written to `Validation/physics-check.txt`. **Park Physics → Rebuild Assignment Scene** recreates the supplied scene and materials, replacing edits to that scene.
