using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    [Header("Variaveis do Dialogo")]
    public RectTransform panel; // Arraste o painel de dialogo no Inspector
    public TextMeshProUGUI nameText;  // Nome do personagem
    public TextMeshProUGUI dialogueText; // Texto do dialogo
    public Image portraitImage; // Imagem do personagem
    public Image backGround; //fundo da tela
    public Image charLeft; //personagem que fica do lado esquerdo da tela
    public Image charRigth; //personagem que fica do lado direito da tela
    public Image nextImage; // Imagem para avancar o texto
    public GameObject horizontalChoicePanel; // Painel das escolhas horizontais
    public Button choiceBtnPrefabH; // Botao base
    public float typingSpeed = 0.05f; // Velocidade do efeito de digitacao
    public GameObject verticalChoicePanel; // Painel das escolhas verticais
    public Button choiceBtnPrefabV; // Botao base vertical




    [Space(5)]
    [Header("Variaveis De Controle")]
    private Dialogue currentDialogue; //grupo de falas atual
    private int currentLineIndex; // indice da linha atual
    private bool isTyping = false; //se esta digitando a linha ainda
    private Coroutine typingCoroutine; //iniciar a co-rotina de digitacao
    private bool waitingForChoice = false; // Bloqueia avanco quando ha escolhas
    private Dictionary<string, System.Action> dialogueEvents; //dicionario de eventos para chamar nos dialogos
    Animator anim;



    void Awake()
    {
        // iniciando todas as variaveis de forma forcada para evitar que elas nao sejam chamadas,
        // nao e obrigatorio, mas e bom quando for chamar o dialogue manager para outros codigos
        if (panel == null)
        {
            panel = GameObject.Find("Painel de Dialogo")?.GetComponent<RectTransform>();
        }
        if (nameText == null)
        {
            nameText = GameObject.Find("Name")?.GetComponent<TextMeshProUGUI>();
        }
        if (dialogueText == null)
        {
            dialogueText = GameObject.Find("Dialogue")?.GetComponent<TextMeshProUGUI>();
        }
        if (portraitImage == null)
        {
            portraitImage = GameObject.Find("Portrait")?.GetComponent<Image>();
        }
    }

    void Start()
    {
        panel.localScale = Vector3.zero; // Comeca invisivel
        horizontalChoicePanel.SetActive(false); //deixa inativo o painel de botoes
        verticalChoicePanel.SetActive(false); //deixa inativo o painel de botoes
        nextImage.gameObject.SetActive(false); //desativa o botao de avancar
        dialogueEvents = new Dictionary<string, System.Action> //biblioteca de eventos para chamar durante
                                                               //os dialogos (para trocar background e sprite
                                                               //dos personagens na cena principalmente)
        {
            { "Evento1", Evento1 },
            { "Evento2", Evento2 },
        };
    }

    void Update()
    {
        if (panel.localScale != Vector3.zero) //so ativa se o painel estiver ativo, para evitar bugs de
                                              //chamar a acao da tecla mesmo nao estando em dialogo
        {
            // Verifica se qualquer tecla foi pressionada
            if (Input.anyKeyDown)
            {
                if (isTyping) //se estiver digitando completa o texto
                {
                    CompleteTextInstantly();
                }
                else if (!waitingForChoice) // Se nao estiver esperando escolha e terminou de digitar o texto, avanca
                {
                    nextImage.gameObject.SetActive(false);
                    DisplayNextLine();
                }
            }
        }
    }

    public void StartDialogue(Dialogue dialogue) //funcao usada principalmente por outros scripts
    {
        //caso nao tenha dialogo anexado no script que chamou essa funcao, aparece este erro no console
        if (dialogue == null)
        {
            Debug.LogError("StartDialogue recebeu um dialogo nulo!");
            return;
        }
        currentDialogue = dialogue; //comeca alimentar as variaveis no DialogueManager
        currentLineIndex = 0; //zera o index para comecar o dialogo desde o inicio
        panel.localScale = Vector3.one; //ativa o painel do dialogo que foi desativado no inicio do script
        DisplayNextLine(); //funcao de avancar o dialogo
    }

    public void DisplayNextLine()
    {
        if (currentDialogue == null || currentDialogue.lines == null || currentDialogue.lines.Length == 0)
        {
            Debug.LogWarning("Nenhum dialogo ativo. Ignorando o clique.");
            return; //Impede que o codigo continue e evita o erro caso nao tenha dialogo ativa
        }
        if (isTyping || waitingForChoice) return; //Se ainda estiver digitando ou tenha alguma escolha
                                                  //para ser deicidida, impede do texto avancar

        if (currentLineIndex < currentDialogue.lines.Length) //verifica se o tamanho das linha atual do
                                                             //dialogo e menor que o numero maximo de dialogos
        {
            //puxa todas as informacoes da linha de dialogo, nome, texto, quadro da imagem falando e comeca digitar
            DialogueLine line = currentDialogue.lines[currentLineIndex];

            nameText.text = line.characterName;
            portraitImage.sprite = line.characterPortrait;
            backGround.sprite = line.backGround;
            charLeft.sprite = line.characterLeft;
            charRigth.sprite = line.characterRight;
            nextImage.gameObject.SetActive(false);

            typingCoroutine = StartCoroutine(TypeText(line.text, line.choices));

            currentLineIndex++;

            // Se essa fala tem um evento, executa ele
            if (!string.IsNullOrEmpty(line.eventName) && dialogueEvents.ContainsKey(line.eventName))
            {
                dialogueEvents[line.eventName]?.Invoke();
            }

        }
        else //caso o numero de linhas seja igual ou maior (nao provavel) ele chama a funcao para encerrar o dialogo
        {
            EndDialogue();
        }
    }

    IEnumerator TypeText(string text, DialogueChoice[] choices) // co-rotina de digitacao da fala, caso tenha botoes ativa eles
    {
        isTyping = true; // coloca a variavel booleana em verdade para informa que esta digitando
        dialogueText.text = ""; // inicia a variavel para escrever texto

        foreach (char letter in text)// verifica a quantidade de letras no texto da linha do dialogo
        {
            dialogueText.text += letter; // adiciona uma letra por vez na variavel iniciada
            yield return new WaitForSeconds(typingSpeed); // aguarda o tempo da variavel para chamar a proxima letra
        }

        isTyping = false; // quando termina o loop de repeticao transforma a booleana em falsa

        if (choices.Length > 0) // se existir botao criado no dialogo ativa o painel de escolhas
        {
            ShowChoices(choices);
        }
        else //caso nao exista escolhas ativa a imagem de avancar texto
        {
            nextImage.gameObject.SetActive(true);
        }
    }

    void CompleteTextInstantly()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Para a corrotina da digitacao
        }

        dialogueText.text = currentDialogue.lines[currentLineIndex - 1].text; // Mostra o texto inteiro
        isTyping = false;

        if (currentDialogue.lines[currentLineIndex - 1].choices.Length > 0) //verifica se a linha do dialogo
                                                                            //tem escolhas igual na corrotina TypeText
        {
            ShowChoices(currentDialogue.lines[currentLineIndex - 1].choices);
        }
        else //caso nao exista escolhas ativa a imagem de avancar texto
        {
            nextImage.gameObject.SetActive(true);
        }
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        waitingForChoice = true;

        // Pega a linha atual
        DialogueLine line = currentDialogue.lines[currentLineIndex - 1];

        // Define painel e prefab com base no tipo
        GameObject choicePanelAtual = line.useVerticalPanel ? verticalChoicePanel : horizontalChoicePanel;
        Button prefab = line.useVerticalPanel ? choiceBtnPrefabV : choiceBtnPrefabH;

        // Ativa o painel correto
        choicePanelAtual.SetActive(true);

        // Remove escolhas anteriores
        foreach (Transform child in choicePanelAtual.transform)
        {
            Destroy(child.gameObject);
        }

        // Cria os bot�es
        foreach (DialogueChoice choice in choices)
        {
            Button newButton = Instantiate(prefab, choicePanelAtual.transform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            newButton.onClick.AddListener(() => SelectChoice(choice, choicePanelAtual));
        }
    }

    void SelectChoice(DialogueChoice choice, GameObject painel)
    {
        choice.action?.Invoke(); // Executa a a��o do bot�o
        painel.SetActive(false); // Desativa o painel correto
        waitingForChoice = false;
        DisplayNextLine();
    }

    public void EndDialogue()
    {
        panel.localScale = Vector3.zero; //desativa o painel de dialogo
        horizontalChoicePanel.SetActive(false); //deixa inativo o painel de botoes
        verticalChoicePanel.SetActive(false); //deixa inativo o painel de botoes
        nextImage.gameObject.SetActive(false); //desativa a imagem  de avancar texto
    }


    public void Evento1()
    {
    }

    public void Evento2()
    {
    }
}