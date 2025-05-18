using System.Collections;
using System.Collections.Generic;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : SingleInstance<PlayerController>
{
	public enum PlayerState
	{
		MOVEMENT,
		PUZZLE,
	}
	public ThirdPersonController controller;

	public PlayerState state = PlayerState.MOVEMENT;
	

	InputAction ringAction;
	bool moveThePuzzle;

	public List<Rune> runes = new List<Rune>();

	void Start()
	{
		ringAction = controller.PlayerInput.currentActionMap.FindAction("Ring");

	}

	void Update()
	{
		switch (state)
		{
			case PlayerState.MOVEMENT:
				
				break;
			case PlayerState.PUZZLE:

				

				if (ringAction.WasReleasedThisFrame())
				{
					moveThePuzzle = false;
				}

				break;	
		}
	}

	public void OnRing(InputValue value)
	{
		if (state == PlayerState.PUZZLE)
		{
			if (value.isPressed)
			{
				moveThePuzzle = true;
			}
			return;
		}

		if (value.isPressed)
		{
			if (RunePuzzle.Current != null)
			{
				RunePuzzle.Current.RunThisPuzzle();
				state = PlayerState.PUZZLE;
			}

			Debug.Log("Play ring");

			foreach (var citizen in Citizen.AllNear)
			{
				citizen.FollowFor(Random.Range(3f, 5f));
			}
		}
	}

	public Coroutine TryToMoveHere(Vector3 pos, Vector3 forward)
	{
		return StartCoroutine(MoveHereRoutine(pos, forward));
	}

	IEnumerator MoveHereRoutine(Vector3 pos, Vector3 forward)
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
		controller.SetDirection(pos);
	}
}
