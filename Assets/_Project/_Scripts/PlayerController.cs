using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : SingleInstance<PlayerController>
{
	public ThirdPersonController controller;

	void Start()
	{
		
	}

	public void OnRing(InputValue value)
	{
		if (value.isPressed)
		{
			Debug.Log("Play ring");

			foreach (var citizen in Citizen.AllNear)
			{
				citizen.FollowFor(Random.Range(5f, 8.5f));
			}
		}
	}

	public Coroutine TryToMoveHere(Vector3 pos)
	{
		return StartCoroutine(MoveHereRoutine(pos));
	}

	IEnumerator MoveHereRoutine(Vector3 pos)
	{
		yield return null;
		Vector3 dir = (pos - transform.position).normalized;

		controller.SetRawDirection(true);
		while ((pos - transform.position).magnitude > 0.5f)
		{
			controller.SetInput(new Vector2(dir.x, dir.z));
			//controller.Move(true);

			yield return null;
		}

		controller.SetInput(Vector2.zero);
		controller.SetRawDirection(false);
	}
}
