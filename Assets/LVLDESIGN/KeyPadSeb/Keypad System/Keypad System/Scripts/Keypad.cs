using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeypadManager : MonoBehaviour
{
    private AudioSource audioSource;

    public GameObject door;
    private bool doorOpen;

    [Header("Screen Settings")]
    public TextMeshPro screenText;

    [Header("Keypad Settings")]
    public string correctCode = "1234";
    public int maxCodeLength = 4;

    public AudioClip FailSound;
    public AudioClip WinSound;

    private string currentCode = "";

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ButtonPressed(string value)
    {
        if (currentCode.Length >= maxCodeLength)
        {
            currentCode = "";
        }

        currentCode += value;
        UpdateScreen();

        if (currentCode.Length == maxCodeLength)
        {
            StartCoroutine(CheckCodeCoroutine());
        }
    }

    private void UpdateScreen()
    {
        screenText.text = currentCode;
    }

    private IEnumerator CheckCodeCoroutine()
    {
        if (currentCode == correctCode)
        {
            Debug.Log("Code Correct! Unlock!");
            audioSource.PlayOneShot(WinSound);
            screenText.color = Color.green; // Change text to green
            doorOpen = !doorOpen;
            door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
        }
        else
        {
            Debug.Log("Incorrect Code!");
            audioSource.PlayOneShot(FailSound);
            screenText.color = Color.red; // Change text to red
        }

        // Wait for 1 second so user can see full code
        yield return new WaitForSeconds(1f);

        // Reset input and screen
        currentCode = "";
        screenText.text = "";
        screenText.color = Color.white; // Reset to default color
    }
}
