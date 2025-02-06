using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
    public GameObject scrollbar;
    private float scrollPos = 0;
    private float[] positions;
    private int currentIndex = 0;

    // Adjustable speed for smooth scrolling
    public float scrollSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the positions array
        positions = new float[transform.childCount];
        float distance = 1f / (positions.Length - 1f);

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = distance * i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for swipe gestures
        if (Input.GetMouseButtonDown(0))
        {
            // Store the initial touch position
            Vector2 startTouch = Input.mousePosition;
            StartCoroutine(SwipeCoroutine(startTouch));
        }
    }

    private IEnumerator SwipeCoroutine(Vector2 startTouch)
    {
        while (Input.GetMouseButton(0))
        {
            yield return null; // Wait until the next frame
        }

        // Calculate the swipe direction
        Vector2 endTouch = Input.mousePosition;
        float swipeDistance = endTouch.x - startTouch.x;

        if (swipeDistance > 50) // Swipe right
        {
            currentIndex = Mathf.Clamp(currentIndex - 1, 0, positions.Length - 1);
        }
        else if (swipeDistance < -50) // Swipe left
        {
            currentIndex = Mathf.Clamp(currentIndex + 1, 0, positions.Length - 1);
        }

        // Smoothly scroll to the new position
        StartCoroutine(SmoothScrollTo(positions[currentIndex]));
    }

    private IEnumerator SmoothScrollTo(float targetValue)
    {
        float startValue = scrollbar.GetComponent<Scrollbar>().value;
        float elapsedTime = 0;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime / scrollSpeed; // Adjust the speed here
            scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(startValue, targetValue, elapsedTime);
            yield return null; // Wait until the next frame
        }

        scrollbar.GetComponent<Scrollbar>().value = targetValue; // Ensure it reaches the target
    }
}
