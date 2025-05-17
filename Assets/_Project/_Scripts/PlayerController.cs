using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : SingleInstance<PlayerController>
{
	public void OnRing(InputValue value)
	{
		if (value.isPressed)
		{
			Debug.Log("Play ring");		

			foreach(var citizen in Citizen.AllNear)
			{
				citizen.FollowFor(Random.Range(5f, 8.5f));	
			}
		}
	}
}
