using UnityEngine;
using UltEvents;

public class FireEventOnDisable : MonoBehaviour
{
	public UltEvent triggeredEvents;
	
	private void OnDisable()	
	{
		if ( triggeredEvents !=null && triggeredEvents.HasCalls )
			triggeredEvents.InvokeSafe();
		
	}
}
