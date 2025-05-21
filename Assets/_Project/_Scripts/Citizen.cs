using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class Citizen : MonoBehaviour
{
	public static Citizen CurrentNear;
	public static List<Citizen> AllNear = new List<Citizen>();
	[RuntimeInitializeOnLoadMethod]
	static void ClearStatic()
	{
		CurrentNear = null;
		AllNear.Clear();
	}

	public NavMeshAgent agent;
	public float minDistance = 3f;

	public TextMeshProUGUI followCueExclamationTemp;

	public Vector2 minMaxSpeed = new Vector2(2f, 4f);
	public Rune runeReward;

	public bool canBeAttracted = true;
	

	void Start()
	{
		followCueExclamationTemp.DOFade(0.0f, 0.0f);
		agent.speed = Random.Range(minMaxSpeed.x, minMaxSpeed.y);	
	}

	void Update()
	{
		if (!canBeAttracted) return; 

		if ((PlayerController.Instance.transform.position - transform.position).magnitude < minDistance)
		{
			//InteractionManager.Instance.Show();
			if (CurrentNear != null && CurrentNear != this)
			{
				InteractionManager.Instance.Hide();
			}

			CurrentNear = this;
			InteractionManager.Instance.Show();

			AllNear.Add(this);
		}
		else
		{
			if (CurrentNear == this)
			{
				InteractionManager.Instance.Hide();
			}

			AllNear.Remove(this);
		}
	}

	public void FollowFor(float time)
	{
		if (!canBeAttracted) return; 

		followCueExclamationTemp.DOFade(1.0f, 0.1f);

		agent.isStopped = false;
		StartCoroutine(FollowRoutine(time));
	}

	IEnumerator FollowRoutine(float time)
	{
		yield return null;
		followCueExclamationTemp.DOFade(0.0f, 1.5f);

		float t = 0;

		agent.SetDestination(PlayerController.Instance.transform.position);
		agent.isStopped = false;
		while (canBeAttracted && t < time)
		{
			t += Time.deltaTime;
			agent.SetDestination(PlayerController.Instance.transform.position);
			yield return null;
		}

		//agent.isStopped = true;
	}

	public void GiveReward()
	{
		if (runeReward != null)
		{
			StopCoroutine(FollowRoutine(0f));

			PlayerHUD.Instance.AddRune(runeReward);
			PlayerController.Instance.runes.Add(runeReward);
			canBeAttracted = false;
		}
		else
		{
			Debug.Log("No rune to give");
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, minDistance);	
	}
}
