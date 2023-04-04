Script makes use of three elements:
- FMOD Event - it has to be FMOD 3D event, and as such it works fine for all channel configuration 
- Collider (or if "use of child colliders is active - set of child objects with colliders and a rigidbody on parent) - set as a trigger
- FMOD listener (Player) - or more precisely it's position, here it's the same as player's as it's FPP game - it's also detected by 'Player' tag, 
      For TPP game:
      - modify the names of variables: player, distanceToPlayer, playerPosition; for ones that are more suitable for your game
      - Awake function in its first part checks for 'player' object if it's assigned by user, if not it search for it by "Player" tag - modify it according to how you position listener on objects
      - OnTrigger Enter/Exit functions checks if object has "Player" tag - change it for specific tag of your listener


How it works:
- Basic functionality of AreaEmitter is updating the position of FMOD event based on the player position
- Point for updated position can be either the closest point of Collider, or Player's position if he is inside the area (tho closest point of collider can work in both situation, using Player is simpler and don't require unnecessary calculations)
- When Player is inside Area, as the distance of attenuation will be 0 (or below minimum distance as position can be bit delayed - keep that in mind while setting up attenuation curve) Event will be played as it would be 2D - preserving the original channel configuration and spatialization (quad ambiences will play in surround etc)
- Script keeps attention to the distance between Player and Emitter - if its above the maximum distance set in FMOD then it will not update the position of event 
- For visualisation of the emitter point script uses gizmos with white sphere representing that point
