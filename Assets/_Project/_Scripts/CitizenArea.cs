using UnityEngine;

public class CitizenArea : MonoBehaviour
{
	public float radius = 10f;
	void Start()
	{

	}

	void Update()
	{
		//loop through all citizens and check if they are in the area
		foreach (var citizen in Citizen.AllNear)
		{
			if ((citizen.transform.position - transform.position).magnitude < radius)
			{
				if (citizen.canBeAttracted)
				{
					citizen.GiveReward();
					citizen.agent.SetDestination(transform.position);
				}
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;	
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
