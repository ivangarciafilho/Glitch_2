using UnityEngine;
using UnityEngine.UI;

public class RotateTroughSlider : MonoBehaviour
{
    public Slider slider;
    
    public Vector3 minRotation;
    public Vector3 maxRotation;

    public bool normalized = false;

    private Vector3 eulerRotation;
    private Vector3 targetRotation;


    private float sliderValue;

    private void Update ( )
    {
        sliderValue = slider.value;
        eulerRotation = transform.localEulerAngles;

        if ( normalized )
        {
            targetRotation.x = Mathf.Lerp ( minRotation.x, maxRotation.x, sliderValue );
            targetRotation.y = Mathf.Lerp ( minRotation.y, maxRotation.y, sliderValue );
            targetRotation.z = Mathf.Lerp ( minRotation.z, maxRotation.z, sliderValue );
        }
        else
        {
            targetRotation.y = sliderValue;
        }

        transform.localEulerAngles = targetRotation;
    }
}
