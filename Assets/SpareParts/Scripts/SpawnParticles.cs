using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticles : MonoBehaviour
{
	public ParticleSystem main;
	public ParticleSystem.Particle[] particles = new ParticleSystem.Particle[90];
	public Transform attractor;



	public void Update ( )
	{
		var amountOfParticles = main.GetParticles ( particles );

		for ( int i = 0; i < amountOfParticles ; i++ )
		{
			var attractorDireciton = attractor.position - particles [ i ].position;
			particles [ i ].velocity = attractorDireciton;
		}

		main.SetParticles ( particles );
	}
}
