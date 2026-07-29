# Detection Service for Unity

> Spatial-grid detection with field-of-view and obstacle checks.

`DetectionService` tracks `IDetectable` objects in a two-dimensional XZ grid.
It can query visible objects by position, direction, angle and distance while
using a raycast to exclude targets hidden behind obstacles.

## Features

- Grid-based spatial partitioning.
- Register, unregister and move detectable objects.
- Field-of-view and distance filtering.
- Obstacle check through `Physics.Raycast` and a `LayerMask`.
- Optional Gizmo and vision debugger helpers.

## Installation

```bash
git submodule add https://github.com/Azzazelloqq/Detection-service.git Assets/DetectionService
```

Or add to `Packages/manifest.json`:

```json
"com.azzazello.detectionservice": "https://github.com/Azzazelloqq/Detection-service.git"
```

## Make an object detectable

```csharp
using Azzazelloqq.DetectionService.Source;
using UnityEngine;

public sealed class Enemy : MonoBehaviour, IDetectable
{
    public Vector3 Position => transform.position;
    public bool IsDead { get; private set; }
}
```

## Register and query

```csharp
var detection = new DetectionService(cellSize: 5f);
detection.RegisterObject(enemy);

var visible = detection.DetectObjectsInView(
    observerPosition: transform.position,
    observerForward: transform.forward,
    viewAngle: 90f,
    viewDistance: 15f,
    obstacleLayer: obstacleMask);
```

When an object crosses a grid cell boundary, notify the service with its prior
position:

```csharp
var previousPosition = enemy.Position;
// Move the enemy.
detection.UpdateObjectPosition(enemy, previousPosition);
```

Call `UnregisterObject` when the object leaves the world.

## Notes

- The service uses the XZ plane; Y does not affect grid placement.
- `IDetectable.IsDead` objects are excluded from results.
- The result collection is reused between queries. Consume it immediately and
  do not retain it across another `DetectObjectsInView` call.
- `obstacleLayer` controls which colliders block line of sight.

## API

`IDetectionService` exposes object registration, position updates,
`DetectObjectsInView`, `GetCellSize` and `GetGrid` for diagnostics.
