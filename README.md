# Systems to use in my other games

## Flow field pathfinding

Flow field pathfinding is an optimal pathfinding algorithm when you have many agents moving towards the same object.  It uses a grid of (imaginary) arrows pointing to the next square.

Usage: copy the whole Pathfinding directory in, or just FlowField.cs and FlowFieldRegistry.cs.  Usage:

```
FlowFieldRegistry.Get(Transform key, Vector3 follower)
```

Returns a float [0, 360) indicating the direction (counterclockwise from right) to move.  It calculates a new FlowField if there's not one yet for the given key; otherwise uses the old one.

Used in my game [Essence](https://github.com/uncannyforest/Essence).

## Algorithmic creature generation

Flexible dynamic creature generation.  Connects body parts with tubes for legs/arms/neck.  Torso uses a rig to control BWH shape.

Not ready for primetime. Currently only two-legged beings are implemented, and reverse kinematic movement is still in progress.
