using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnParticleOnReflection : MonoBehaviour
{
	public ParticleSystem mainParticleSystem;
	public ParticleSystem subParticleSystem;
	public ParticleSystem subParticleSystem_1;

	public List<ParticleCollisionEvent> collisions = new List<ParticleCollisionEvent>();


	private void OnParticleCollision ( GameObject other )
	{
		mainParticleSystem.GetCollisionEvents ( other, collisions);
		var amountOfCollisions = collisions.Count;

		for ( int i = 0; i < amountOfCollisions; i++ )
		{
			var mainParticleSpawnPosition = mainParticleSystem.transform.position;

			var spawnPosition = collisions [ i ].intersection;
			var normal = collisions [ i ].normal;

			var direction = (spawnPosition - mainParticleSpawnPosition).normalized;
			var directionRecflected = Vector3.Reflect ( direction , normal );

			subParticleSystem.transform.position = spawnPosition;
			subParticleSystem.transform.forward = directionRecflected;
			subParticleSystem_1.transform.position = spawnPosition;
			subParticleSystem_1.transform.forward = directionRecflected;

			subParticleSystem.Emit ( 30 );
			subParticleSystem_1.Emit ( 30);
		}
	}
}
