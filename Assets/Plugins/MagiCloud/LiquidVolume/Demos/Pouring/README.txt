Pouring demo scene
------------------

This demo scene uses a special prefab "PotionOpen" located in DemoResources folder.
It also uses a particle system and a custom particle shader located in the same folder that distorts the background simulating water effect.

The way the pouring works in this demo is by using a special cube with a collider and rigidbody just under the surface level.
Take a look at the cube children of the prefab. This cube only has a collider and rigidbody so the particles can collide.
It also has a script attached than contains the logic to position the cube just under the surface level and increase the liquid level when the particles hit the collider.
