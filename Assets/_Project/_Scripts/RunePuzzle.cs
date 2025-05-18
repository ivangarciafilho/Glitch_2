using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class RunePuzzle : MonoBehaviour
{
	public Rune rune;
	public Animator animator;
	public static RunePuzzle Current;
	public CinemachineCamera vCam;
	public Transform playerShouldBeHere;
	public float minDistance = 3f;


	public void RunThisPuzzle()
	{
		StartCoroutine(RunThisPizzleRoutine());
	}

	IEnumerator RunThisPizzleRoutine()
	{
		vCam.Priority = 50;
		yield return PlayerController.Instance.TryToMoveHere(playerShouldBeHere.position, playerShouldBeHere.forward);

		yield return new WaitForSeconds(0.5f);

		if (PlayerController.Instance.runes.Contains(rune))
		{
			PlayerController.Instance.runes.Remove(rune);
			animator.SetTrigger("Perform");
			// construction logic here
		}
		else
		{
			animator.SetTrigger("Block");
		}
	}

	void Update()
	{
		if ((PlayerController.Instance.transform.position - transform.position).magnitude < minDistance)
		{
			Current = this;
		}
		else
		{
			if (Current == this)
			{
				Current = null;
			}
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, minDistance);	
	}
}
