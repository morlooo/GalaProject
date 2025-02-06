using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using System.Collections;

public class LoadMedia : MonoBehaviour
{
    public string folderPath = @"F:/media/"; // Path to media folder

    public GameObject imagePrefab; // Prefab containing RawImage
    public GameObject videoPrefab; // Prefab containing VideoPlayer
    public Transform contentPanel; // ScrollView's Content panel

    public GameObject scrollbar; // Reference to the Scrollbar for swiping
    private float scrollPos = 0;
    private float[] positions;
    private int totalItems;
    private int currentIndex = 0;
    public float scrollSpeed = 0.1f; // Adjustable speed for smooth scrolling

    private RectTransform contentRect; // Reference to the Content panel's RectTransform
    private float screenWidth; // Screen width in pixels

    void Start()
    {
        // Get the screen width
        screenWidth = Screen.width;

        // Get the RectTransform of the content panel
        contentRect = contentPanel.GetComponent<RectTransform>();
        LoadMediaFromDisk();

        InitializeSwipePositions();
    }

    void LoadMediaFromDisk()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("Folder does not exist: " + folderPath);
            return;
        }

        string[] mediaFiles = Directory.GetFiles(folderPath); // Get all files

        foreach (string file in mediaFiles)
        {
            string extension = Path.GetExtension(file).ToLower();

            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                LoadImage(file);
            }
            else if (extension == ".mp4" || extension == ".avi" || extension == ".mov")
            {
                LoadVideo(file);
            }
        }

        // Adjust the content panel width to fit all media items
        contentRect.sizeDelta = new Vector2(screenWidth * contentPanel.childCount, contentRect.sizeDelta.y);
    }

    void LoadImage(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);

        if (texture.LoadImage(fileData))
        {
            GameObject newImage = Instantiate(imagePrefab, contentPanel);
            RawImage rawImage = newImage.GetComponent<RawImage>();
            rawImage.texture = texture;

            // Set the size of the image to match the screen width and height
            RectTransform imageRect = newImage.GetComponent<RectTransform>();
            imageRect.sizeDelta = new Vector2(screenWidth, Screen.height);

            Debug.Log("Loaded image: " + filePath);
        }
        else
        {
            Debug.LogError("Failed to load image: " + filePath);
        }
    }

    void LoadVideo(string filePath)
    {
        GameObject newVideo = Instantiate(videoPrefab, contentPanel);
        VideoPlayer videoPlayer = newVideo.GetComponent<VideoPlayer>();
        RawImage videoDisplay = newVideo.GetComponent<RawImage>();

        string formattedPath = "file:///" + filePath.Replace("\\", "/");

        videoPlayer.url = formattedPath;
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = new RenderTexture(1920, 1080, 16);
        videoDisplay.texture = videoPlayer.targetTexture;

        // Set the size of the video to match the screen width and height
        RectTransform videoRect = newVideo.GetComponent<RectTransform>();
        videoRect.sizeDelta = new Vector2(screenWidth, Screen.height);

        newVideo.AddComponent<Button>().onClick.AddListener(() => ToggleVideoPlayback(videoPlayer));

        videoPlayer.Play();
        Debug.Log("Loaded video: " + filePath);
    }

    void ToggleVideoPlayback(VideoPlayer videoPlayer)
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
    }

    void InitializeSwipePositions()
    {

        totalItems = contentPanel.childCount; // Count number of items dynamically

        if (totalItems <= 1) return; // No need for scrolling if only one item

        positions = new float[totalItems];

        for (int i = 0; i < totalItems; i++)
        {
            positions[i] = (float)i / (totalItems - 1); // Normalize positions (0 to 1)
        }
    }

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
        Debug.Log("Swipe started at: " + startTouch.x);
        while (Input.GetMouseButton(0))
        {
            yield return null; // Wait until the next frame
        }

        // Calculate the swipe direction
        Vector2 endTouch = Input.mousePosition;
        float swipeDistance = endTouch.x - startTouch.x;

        Debug.Log("Swipe ended at: " + endTouch.x + " | Distance: " + swipeDistance);
        
        if (swipeDistance > 50) // Swipe right
        {
            currentIndex = Mathf.Clamp(currentIndex - 1, 0, positions.Length - 1);
            Debug.Log("Swiped Right. New Index: " + currentIndex);
        }
        else if (swipeDistance < -50) // Swipe left
        {
            currentIndex = Mathf.Clamp(currentIndex + 1, 0, positions.Length - 1);
            Debug.Log("Swiped Left. New Index: " + currentIndex);
        }
        else
        {
            Debug.Log("Swipe too short, ignoring.");
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
