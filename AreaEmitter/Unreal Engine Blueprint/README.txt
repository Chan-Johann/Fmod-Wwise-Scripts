Blueprint made in Unreal Engine 4.27
It's made for creating 3D sound sources based on large object or areas like forest, river etc, things that might not sound proper by just single 3D event placed in the world as it would be 'too directional' in a site that would feel not correct.



Blueprint makes use of three elements:
- FMOD Event - it has to be FMOD 3D event, and as such it works fine for all channel configuration 
- FMOD listener (Player) - or more precisely it's position, here it's the same as player's as it's script for FPP game
- Collision component



How it works:
- Basic functionality of AreaEmitter is updating the position of FMOD event based on the player position.
- Point for updated position is based on the closest point of Collision or Player's position if he is inside the area.
- While Player enter/exit the area it will start Actor Begin/End Overlap events which will change how position is gonna be updated.
- When Player is inside Area the distance of attenuation should be below minimum one (position can be bit delayed - keep that in mind while setting up attenuation curve) Event will be played as it would be 2D - preserving the original channel configuration and spatialization.
- Blueprint keeps attention to the distance between Player and Emitter - if its above the maximum distance set in FMOD it will not update the position of event 
- in Editor script visualize the emitter point by a red sphere moving in the collision
