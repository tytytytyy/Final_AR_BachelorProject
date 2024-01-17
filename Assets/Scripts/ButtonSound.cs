using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{

    // Audio Clip der abgespielt wird
    [SerializeField]
    public AudioClip clickSound;  

    private Button button;  // Referenz auf das Button-Komponente
    private AudioSource audioSource;  

    void Start()
    {
        // Hole Referenzen auf die Komponenten
        button = GetComponent<Button>();
        audioSource = gameObject.AddComponent<AudioSource>();  // F�ge eine AudioSource-Komponente hinzu

        // F�ge einen Listener f�r das Button-Klick-Ereignis hinzu
        button.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        // Spiele den Klick-Sound ab
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
