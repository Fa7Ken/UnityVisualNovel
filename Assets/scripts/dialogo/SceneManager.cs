using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public DialogueManager dialogueManager; // O controle do dialogo 
    public Dialogue dialogo0;
    public Dialogue dialogo1;
    public Dialogue dialogo2;
    public bool active = false;


    public void Start()
    {
        if (dialogueManager == null)
        {
            dialogueManager = GameObject.Find("DialogueManager")?.GetComponent<DialogueManager>();
        }
    }

    public void Update()
    {
        if (!active) // Se o activate estiver negativo, acessa aqui para iniciar o dialogo, com o proposito de iniciar no momento que iniciar a cena
        {
            dialogueManager.StartDialogue(dialogo0);
            active = true;
        }

    }

    public void Dialogo1()
    {
        if (dialogueManager == null)
        {
            dialogueManager = GameObject.Find("DialogueManager")?.GetComponent<DialogueManager>();
            dialogueManager.StartDialogue(dialogo1);
        }
    }

    public void Dialogo2()
    {
        if (dialogueManager == null)
        {
            dialogueManager = GameObject.Find("DialogueManager")?.GetComponent<DialogueManager>();
            dialogueManager.StartDialogue(dialogo2);
        }
    }
}
