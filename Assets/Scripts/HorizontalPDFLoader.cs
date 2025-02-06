using UnityEngine;
using UnityEngine.UI;
using Paroxe.PdfRenderer;  // Import Paroxe PDF Renderer
using System.IO;
using System.Collections.Generic;

public class HorizontalPDFLoader : MonoBehaviour
{
    public Transform contentPanel;  // Assign the Content panel inside ScrollView
    public GameObject buttonPrefab; // Assign a UI Button Prefab
    public PDFViewer pdfViewer;     // Assign PDF Viewer component

    private string pdfFolderPath = "F:/media/pdf/"; // Change to your local folder path
    private List<string> pdfFiles = new List<string>(); // Stores PDF file paths

    void Start()
    {
        LoadPDFs();
    }

    void LoadPDFs()
    {
        if (!Directory.Exists(pdfFolderPath))
        {
            Debug.LogError("Folder does not exist: " + pdfFolderPath);
            return;
        }

        string[] files = Directory.GetFiles(pdfFolderPath, "*.pdf"); // Get all PDFs

        foreach (string file in files)
        {
            pdfFiles.Add(file);
            CreateButton(file); // Create a button for each PDF
        }
    }

    void CreateButton(string filePath)
    {
        GameObject newButton = Instantiate(buttonPrefab, contentPanel); // Create Button
        newButton.GetComponentInChildren<Text>().text = Path.GetFileName(filePath); // Set button text
        newButton.GetComponent<Button>().onClick.AddListener(() => LoadSelectedPDF(filePath)); // Add Click Event
    }

    public void LoadSelectedPDF(string filePath)
    {
        PDFDocument document = new PDFDocument(filePath); // Load PDF

        if (document != null)
        {
            pdfViewer.LoadDocument(document); // Show in Viewer
        }
        else
        {
            Debug.LogError("Failed to load PDF: " + filePath);
        }
    }
}
