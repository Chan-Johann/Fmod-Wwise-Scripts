Script made and tested in Unity 2022.2.8f1
It's made for creating 3D sound sources based on large object or areas like forest, river etc, things that might not sound proper by just single 3D event placed in the world as it would be 'too directional' in a site that would feel not correct.



Script makes use of three elements:
- FMOD Event - it has to be FMOD 3D event, and as such it works fine for all channel configuration 
- Collider (or if "useChildColliders" is active - set of child objects with colliders and a rigidbody on parent) - set as a trigger
- FMOD listener (Player) - or more precisely it's position, here it's the same as player's as it's FPP game - it's also detected by 'Player' tag, 
      For TPP game:
      - modify the names of variables: player, distanceToPlayer, playerPosition; for ones that are more suitable for your game
      - Awake function in its first part checks for 'player' object if it's assigned by user, if not it search for it by "Player" tag - modify it according to how you position listener on objects
      - OnTrigger Enter/Exit functions checks if object has "Player" tag - change it for specific tag of your listener



How it works:
- Basic functionality of AreaEmitter is updating the position of FMOD event based on the player position.
- Point for updated position is based on the closest point of Collider or Player's position if he is inside the area (tho closest point of collider can work in both situation, using Player is simpler and don't require unnecessary calculations)
- While Player enter/exit the area it will start OnTriggerEnter/Exit fuctions, which will change how position is gonna be updated.
- When Player is inside Area the distance of attenuation should be below minimum one (position can be bit delayed - keep that in mind while setting up attenuation curve) Event will be played as it would be 2D - preserving the original channel configuration and spatialization.
- Script keeps attention to the distance between Player and Emitter - if its above the maximum distance set in FMOD it will not update the position of event 
- Script visualize the emitter point in gizmos by a red sphere moving in the collider
- Basic version uses single Collider component for the emitter point, tho with "useChildColliders" it allows to create more complex structure with multiple colliders placed on the child objects (keep in mind narrow corners, as one wall on the left, might be as close to the wall on right and make sound jump between them) in this situation the Trigger function that keeps track of player being inside/outside won't work without Rigidbody on the main gameobject.
