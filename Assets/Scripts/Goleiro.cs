using UnityEngine;
using UnityEngine.InputSystem;

public class Goleiro : MonoBehaviour
{
    private enum LadoCampo
    {
        Esquerdo,
        Direito
    }

    [Header("Referencias")]
    [SerializeField] private Campo campo;

    [Header("Configuracao do Goleiro")]
    [SerializeField] private LadoCampo lado = LadoCampo.Esquerdo;
    [SerializeField] private float velocidadeMovimento = 10f;

    [Header("Extensoes da Colisao")]
    [SerializeField] private float extensaoEsquerda = 0.5f;
    [SerializeField] private float extensaoDireita = 0.5f;
    [SerializeField] private float extensaoBaixo = 3f;
    [SerializeField] private float extensaoCima = 3f;

    [Header("Teclas")]
    [SerializeField] private KeyCode teclaCima = KeyCode.W;
    [SerializeField] private KeyCode teclaBaixo = KeyCode.S;
    [SerializeField] private KeyCode teclaEsquerda = KeyCode.A;
    [SerializeField] private KeyCode teclaDireita = KeyCode.D;

    private InputAction acaoMovimento;
    private Vector2 velocidadeAtual;

    public float LimiteColisaoEsquerda => transform.position.x - extensaoEsquerda;
    public float LimiteColisaoDireita => transform.position.x + extensaoDireita;
    public float LimiteColisaoInferior => transform.position.y - extensaoBaixo;
    public float LimiteColisaoSuperior => transform.position.y + extensaoCima;
    public Vector2 VelocidadeAtual => velocidadeAtual;

    private void Awake()
    {
        CriarAcaoDeMovimento();
    }

    private void OnEnable()
    {
        acaoMovimento?.Enable();
    }

    private void OnDisable()
    {
        acaoMovimento?.Disable();
    }

    private void OnDestroy()
    {
        acaoMovimento?.Dispose();
    }

    private void Start()
    {
        if (campo == null)
        {
            Debug.LogError("O Goleiro precisa de uma referencia para o Campo.", this);
            enabled = false;
            return;
        }

        ValidarExtensoes();
    }

    private void Update()
    {
        Vector2 posicaoAnterior = transform.position;
        Vector2 direcaoMovimento = LerDirecaoMovimento();
        Vector2 novaPosicao = (Vector2)transform.position + (direcaoMovimento * velocidadeMovimento * Time.deltaTime);

        if (lado == LadoCampo.Esquerdo)
        {
            novaPosicao = campo.LimitarGoleiroEsquerdo(
                novaPosicao,
                extensaoEsquerda,
                extensaoDireita,
                extensaoBaixo,
                extensaoCima
            );
        }
        else
        {
            novaPosicao = campo.LimitarGoleiroDireito(
                novaPosicao,
                extensaoEsquerda,
                extensaoDireita,
                extensaoBaixo,
                extensaoCima
            );
        }

        transform.position = novaPosicao;

        if (Time.deltaTime > 0f)
        {
            velocidadeAtual = (novaPosicao - posicaoAnterior) / Time.deltaTime;
        }
    }

    private Vector2 LerDirecaoMovimento()
    {
        if (acaoMovimento == null)
        {
            return Vector2.zero;
        }

        return acaoMovimento.ReadValue<Vector2>().normalized;
    }

    private void CriarAcaoDeMovimento()
    {
        if (acaoMovimento != null)
        {
            acaoMovimento.Dispose();
        }

        acaoMovimento = new InputAction(name: $"Mover{gameObject.name}", type: InputActionType.Value);
        var composite = acaoMovimento.AddCompositeBinding("2DVector");

        composite.With("Up", ConverterParaCaminhoInputSystem(teclaCima));
        composite.With("Down", ConverterParaCaminhoInputSystem(teclaBaixo));
        composite.With("Left", ConverterParaCaminhoInputSystem(teclaEsquerda));
        composite.With("Right", ConverterParaCaminhoInputSystem(teclaDireita));
    }

    private string ConverterParaCaminhoInputSystem(KeyCode tecla)
    {
        switch (tecla)
        {
            case KeyCode.W:
                return "<Keyboard>/w";
            case KeyCode.A:
                return "<Keyboard>/a";
            case KeyCode.S:
                return "<Keyboard>/s";
            case KeyCode.D:
                return "<Keyboard>/d";
            case KeyCode.I:
                return "<Keyboard>/i";
            case KeyCode.J:
                return "<Keyboard>/j";
            case KeyCode.K:
                return "<Keyboard>/k";
            case KeyCode.L:
                return "<Keyboard>/l";
            case KeyCode.UpArrow:
                return "<Keyboard>/upArrow";
            case KeyCode.DownArrow:
                return "<Keyboard>/downArrow";
            case KeyCode.LeftArrow:
                return "<Keyboard>/leftArrow";
            case KeyCode.RightArrow:
                return "<Keyboard>/rightArrow";
            case KeyCode.Keypad8:
                return "<Keyboard>/numpad8";
            case KeyCode.Keypad2:
                return "<Keyboard>/numpad2";
            case KeyCode.Keypad5:
                return "<Keyboard>/numpad5";
            case KeyCode.Keypad4:
                return "<Keyboard>/numpad4";
            case KeyCode.Keypad6:
                return "<Keyboard>/numpad6";
            default:
                Debug.LogWarning($"Tecla nao suportada para InputAction: {tecla}.", this);
                return "<Keyboard>/f1";
        }
    }

    private void ValidarExtensoes()
    {
        extensaoEsquerda = Mathf.Max(0f, extensaoEsquerda);
        extensaoDireita = Mathf.Max(0f, extensaoDireita);
        extensaoBaixo = Mathf.Max(0f, extensaoBaixo);
        extensaoCima = Mathf.Max(0f, extensaoCima);
    }

    private void OnValidate()
    {
        ValidarExtensoes();

        if (!Application.isPlaying)
        {
            return;
        }

        CriarAcaoDeMovimento();
        acaoMovimento.Enable();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 centro = new Vector3(
            transform.position.x + (extensaoDireita - extensaoEsquerda) * 0.5f,
            transform.position.y + (extensaoCima - extensaoBaixo) * 0.5f,
            transform.position.z
        );

        Vector3 tamanho = new Vector3(
            extensaoEsquerda + extensaoDireita,
            extensaoBaixo + extensaoCima,
            0f
        );

        Gizmos.DrawWireCube(centro, tamanho);
    }

    public Vector2 ObterCentroColisao()
    {
        return new Vector2(
            transform.position.x + (extensaoDireita - extensaoEsquerda) * 0.5f,
            transform.position.y + (extensaoCima - extensaoBaixo) * 0.5f
        );
    }

    public Vector2 ObterTamanhoColisao()
    {
        return new Vector2(
            extensaoEsquerda + extensaoDireita,
            extensaoBaixo + extensaoCima
        );
    }

    public void DefinirPosicao(Vector2 novaPosicao)
    {
        transform.position = novaPosicao;
        velocidadeAtual = Vector2.zero;
    }
}
