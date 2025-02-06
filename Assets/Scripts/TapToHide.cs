using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapToHide : MonoBehaviour, IPointerClickHandler
{
    public GameObject objectToHide;  // The first object to hide
    public GameObject objectToShow;  // The second object to show

    private bool isSwitched = false; // Track state

    public void OnPointerClick(PointerEventData eventData)
    {
        isSwitched = !isSwitched; // Toggle state

        objectToHide.SetActive(!isSwitched); // Hide the first object
        objectToShow.SetActive(isSwitched);  // Show the second object
    }
}
