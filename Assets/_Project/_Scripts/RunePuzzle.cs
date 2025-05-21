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
	public MeshRenderer runeSymbolRenderer;

	void Start()
	{
		// set the rune icon based on the sprite rect of the sprite
		runeSymbolRenderer.material.SetTextureScale("_BaseMap", new Vector2(rune.icon.rect.width / rune.icon.texture.width, rune.icon.rect.height / rune.icon.texture.height));
		runeSymbolRenderer.material.SetTextureOffset("_BaseMap", new Vector2(rune.icon.rect.x / rune.icon.texture.width, rune.icon.rect.y / rune.icon.texture.height));
		runeSymbolRenderer.material.SetTexture("_BaseMap", rune.icon.texture);
	}

	public void RunThisPuzzle()
	{
		StartCoroutine(RunThisPizzleRoutine());
	}

	IEnumerator RunThisPizzleRoutine()
	{
		vCam.Priority = 50;
		yield return PlayerController.Instance.TryToMoveHere(playerShouldBeHere.position, playerShouldBeHere.forward);
		PlayerController.Instance.controller.canMove = false;

		yield return new WaitForSeconds(0.5f);

		if (PlayerController.Instance.runes.Contains(rune))
		{
			PlayerHUD.Instance.RemoveRune(rune);
			PlayerController.Instance.runes.Remove(rune);
			animator.SetTrigger("Perform");
			// construction logic here
		}
		else
		{
			animator.SetTrigger("Block");
		}
	}

	public void EndBlock()
	{
		vCam.Priority = 0;
		PlayerController.Instance.controller.canMove = true;
	}

	public void EndPerform()
	{
		vCam.Priority = 0;
		PlayerController.Instance.controller.canMove = true;
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
