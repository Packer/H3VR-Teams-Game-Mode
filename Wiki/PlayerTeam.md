# Player Team

## Player Team
| Variable    | Setting |
| -------- | ------- |
| name  | Player Team name |
| description | A Brief description of the player team |
| sosigTeam    | Name of the complimentary Sosig Team that goes with this player team |
| playerClasses  | List of Classes the player can choose from |

### Player Class
| Variable    | Setting |
| -------- | ------- |
| name  | Class's Display Name |
| spriteName | `Example.png` name of the image that will be used as a thumbnail |
| playerHealth    |  How much health this class has, default `5000`   |
| minKills  | The minimum amount of kills required before this class becomes available |
| maxKills  | The maximum amount of kills before it becomes unavailable |

#### Sub Class
| Variable    | Setting |
| -------- | ------- |
| name  | Internal name for this sub class |
| Items | List of all items that are spawned with this class |

##### Item Set
| Variable    | Setting |
| -------- | ------- |
| name  | Internal name for this item set |
| objectCount | How many of ObjectIDs are spawned |
| uniformObjects | If `true`, only one `objectID` is selected and spawned `objectCount` amount of times, if `false` randomly select from the `objectID` list per spawned object |
| requiredSecondaryPieces  | if `objectID` has a required Secondary Pieces, spawn them |
| objectID  | A list of spawnable objectIDs |
| ammoCount  | The amount of rounds/magazines/speed loaders/clips spawned |
| ammoUniform  | If `true`, will spawn all ammo as the same type |
| ammoContainerID  | Specific ObjectID for the ammo container used |
| minCapacity  | The minimum capacity the ammo container will be |
| maxCapacity  | The maximum capacity the ammo container will be |
| ammoFireArmRoundClass  | The exact type of ammo thats loaded into the container, see `FireArmRoundClass` enum, default to `-1` to use `ammoType` instead |
| ammoType  | The generic type of ammo thats loaded into the container, see `AmmoEnum` |