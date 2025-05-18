using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Autoplay : MonoBehaviour
{
	public GameObject [ ] particles;
	public Text display;
	public float delay = 7f;

	public void Awake ( )
	{
		var amount = particles.Length;
		for ( int i = 0; i < amount; i++ )
		{
			particles [ i ].GetComponent<ParticleSystem> ( ).Stop ( true, ParticleSystemStopBehavior.StopEmittingAndClear );
			particles [ i].SetActive ( false );
		}
	}

	public void OnEnable ( )
	{
		StopAllCoroutines ( );
		StartCoroutine ( CycleThroughParticles() );
	}

	private IEnumerator CycleThroughParticles ( )
	{
		var nextItem = 0;
		yield return new WaitForSeconds ( 2f );


		while ( true )
		{
			if ( display ) display.text = particles [ nextItem ].name;

			particles [ nextItem ].SetActive ( true );
			particles [ nextItem ].GetComponent<ParticleSystem> ( ).Play ( );

			yield return new WaitForSeconds ( delay );

			particles [ nextItem ].GetComponent<ParticleSystem> ( ).Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
			particles [ nextItem ].SetActive ( false );

			nextItem++;
			nextItem = (nextItem >= particles.Length) ? 0 : nextItem;
		}
	}


#if UNITY_EDITOR
	private void OnValidate ( )
	{
		particles = particles.Where ( _item => _item != null).ToArray();
	}
#endif
}
