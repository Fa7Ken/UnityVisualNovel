using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    // Elementos presentes no objeto de armazenamento do dialogos
    public string characterName;  // Nome do personagem
    public Sprite characterPortrait; // Imagem do personagem no texto de dialogo
    public Sprite backGround; // Imagem do fundo
    public Sprite characterLeft; // Imagem do personagem a direita da tela
    public Sprite characterRight; // Imagem do personagem a esquerda da tela
    [TextArea(3, 10)] public string text; // Texto do diálogo
    public string eventName;  // Nome do evento a ser chamado nessa fala específica
    public DialogueChoice[] choices; // botão de escolhas vertical
    public bool useVerticalPanel; // nova variável que indica se é vertical
}

[System.Serializable]
public class DialogueChoice
{
    // Elementos para preencher o botão quando usado no dialogo
    public string text;
    public UnityEngine.Events.UnityEvent action;
}