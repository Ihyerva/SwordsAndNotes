using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
    public GameObject dialogueTextObject; // Assign the TMP text GameObject in Inspector
    public string[] dialogueLines;
    public float typeSpeed = 0.05f;

    [SerializeField] private GameObject objectA;
    [SerializeField] private GameObject objectB;

    private Image imageA;
    private Image imageB;
    private TMP_Text dialogueText;
    private int currentLine = 0;
    private bool isTyping = false;
    private bool cancelTyping = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueText = dialogueTextObject.GetComponent<TMP_Text>();
        if(objectA != null && objectA.GetComponent<Image>() != null){    
            imageA = objectA.GetComponent<Image>();
        }
        if(objectB != null && objectB.GetComponent<Image>() != null){
            imageB = objectB.GetComponent<Image>();
        }
        SetAlphas(currentLine);
        StartCoroutine(TypeLine());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                cancelTyping = true;
            }
            else
            {
                currentLine++;
                if (currentLine < dialogueLines.Length)
                {
                    SetAlphas(currentLine);
                    StartCoroutine(TypeLine());
                }
                else
                {
                    dialogueText.text = ""; // End of dialogue
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Load next scene
                }
            }
        }
    }

    void SetAlphas(int index)
    {
        if (index % 2 == 0 && imageA != null && imageB != null) 
        {   //Even
            if (imageA != null) SetImageAlpha(imageA, 1f);
            if (imageB != null) SetImageAlpha(imageB, 0.3f);
        }
        else if (imageA != null && imageB != null)
        {
            //Odd
            if (imageA != null) SetImageAlpha(imageA, 0.3f);
            if (imageB != null) SetImageAlpha(imageB, 1f);
        }
    }

    void SetImageAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        cancelTyping = false;
        dialogueText.text = "";
        string line = dialogueLines[currentLine];
        for (int i = 0; i < line.Length; i++)
        {
            if (cancelTyping)
            {
                dialogueText.text = line;
                break;
            }
            dialogueText.text += line[i];
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }
}
