using UnityEngine;
using UnityEngine.Events;
using UltEvents;

public class FireEventOnEnable : MonoBehaviour
{
	public UltEvent triggeredEvents;

	private void OnEnable ( )
		=> triggeredEvents.Invoke ( );
}
