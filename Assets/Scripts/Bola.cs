using UnityEngine;

public class Bola : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Campo campo;
    [SerializeField] private Goleiro goleiroEsquerdo;
    [SerializeField] private Goleiro goleiroDireito;

    [Header("Movimento")]
    [SerializeField] private float velocidadeInicial = 5f;
    [SerializeField] private float velocidadeAoBaterNoGoleiro = 6f;
    [SerializeField] private float velocidadeAoBaterNaLateral = 4.5f;
    [SerializeField] private Vector2 direcaoInicial = new Vector2(1f, 0.35f);
    [SerializeField] private float intensidadeDesvioVertical = 0.75f;
    [SerializeField] private float influenciaMovimentoVerticalGoleiro = 0.035f;
    [SerializeField] private float componenteHorizontalMinimaNoRebote = 0.65f;

    [Header("Tamanho")]
    [SerializeField] private float extensaoHorizontal = 0.25f;
    [SerializeField] private float extensaoVertical = 0.25f;

    private Vector3 posicaoInicial;
    private Vector2 direcaoAtual;
    private float velocidadeAtual;

    public float ExtensaoHorizontal => extensaoHorizontal;
    public float ExtensaoVertical => extensaoVertical;
    public bool EstaAtiva => gameObject.activeSelf;

    private void Start()
    {
        if (campo == null)
        {
            Debug.LogError("A Bola precisa de uma referencia para o Campo.", this);
            enabled = false;
            return;
        }

        posicaoInicial = transform.position;
        velocidadeAtual = velocidadeInicial;
        direcaoAtual = SortearDirecaoInicialAleatoria();
    }

    private void Update()
    {
        Vector2 proximaPosicao = (Vector2)transform.position + (direcaoAtual * velocidadeAtual * Time.deltaTime);

        VerificarColisaoVertical(ref proximaPosicao);
        VerificarColisaoComGoleiro(goleiroEsquerdo, ref proximaPosicao);
        VerificarColisaoComGoleiro(goleiroDireito, ref proximaPosicao);

        VerificarColisaoHorizontal(ref proximaPosicao);

        transform.position = proximaPosicao;
    }

    private void VerificarColisaoComGoleiro(Goleiro goleiro, ref Vector2 posicao)
    {
        if (goleiro == null)
        {
            return;
        }

        bool sobrepoeHorizontalmente =
            posicao.x + extensaoHorizontal >= goleiro.LimiteColisaoEsquerda &&
            posicao.x - extensaoHorizontal <= goleiro.LimiteColisaoDireita;
        bool sobrepoeVerticalmente =
            posicao.y + extensaoVertical >= goleiro.LimiteColisaoInferior &&
            posicao.y - extensaoVertical <= goleiro.LimiteColisaoSuperior;

        if (!sobrepoeHorizontalmente || !sobrepoeVerticalmente)
        {
            return;
        }

        bool veioDaEsquerda = transform.position.x <= goleiro.ObterCentroColisao().x;
        float direcaoHorizontal = veioDaEsquerda ? -1f : 1f;

        if (veioDaEsquerda)
        {
            posicao.x = goleiro.LimiteColisaoEsquerda - extensaoHorizontal;
        }
        else
        {
            posicao.x = goleiro.LimiteColisaoDireita + extensaoHorizontal;
        }

        float diferencaVertical = posicao.y - goleiro.ObterCentroColisao().y;
        float metadeAlturaGoleiro = goleiro.ObterTamanhoColisao().y * 0.5f;
        float componenteVertical = 0f;

        if (metadeAlturaGoleiro > 0f)
        {
            float fatorVertical = Mathf.Clamp(diferencaVertical / metadeAlturaGoleiro, -1f, 1f);
            componenteVertical += fatorVertical * intensidadeDesvioVertical;
        }

        componenteVertical += goleiro.VelocidadeAtual.y * influenciaMovimentoVerticalGoleiro;

        float componenteHorizontalMinimaClamped = Mathf.Clamp(componenteHorizontalMinimaNoRebote, 0.01f, 1f);
        float componenteVerticalMaxima = Mathf.Sqrt(1f - (componenteHorizontalMinimaClamped * componenteHorizontalMinimaClamped));
        componenteVertical = Mathf.Clamp(componenteVertical, -componenteVerticalMaxima, componenteVerticalMaxima);
        float componenteHorizontal = Mathf.Sqrt(1f - (componenteVertical * componenteVertical));

        direcaoAtual = new Vector2(direcaoHorizontal * componenteHorizontal, componenteVertical).normalized;
        velocidadeAtual = velocidadeAoBaterNoGoleiro;
    }

    private void VerificarColisaoVertical(ref Vector2 posicao)
    {
        if (posicao.y + extensaoVertical > campo.LimiteSuperior)
        {
            posicao.y = campo.LimiteSuperior - extensaoVertical;
            direcaoAtual.y *= -1f;
            velocidadeAtual = velocidadeAoBaterNaLateral;
        }
        else if (posicao.y - extensaoVertical < campo.LimiteInferior)
        {
            posicao.y = campo.LimiteInferior + extensaoVertical;
            direcaoAtual.y *= -1f;
            velocidadeAtual = velocidadeAoBaterNaLateral;
        }
    }

    private void VerificarColisaoHorizontal(ref Vector2 posicao)
    {
        bool bateuNaParedeEsquerda = posicao.x - extensaoHorizontal < campo.LimiteEsquerdo;
        bool bateuNaParedeDireita = posicao.x + extensaoHorizontal > campo.LimiteDireito;

        if (bateuNaParedeEsquerda)
        {
            posicao.x = campo.LimiteEsquerdo + extensaoHorizontal;
            direcaoAtual.x *= -1f;
            velocidadeAtual = velocidadeAoBaterNaLateral;
        }
        else if (bateuNaParedeDireita)
        {
            posicao.x = campo.LimiteDireito - extensaoHorizontal;
            direcaoAtual.x *= -1f;
            velocidadeAtual = velocidadeAoBaterNaLateral;
        }
    }

    public void PararEEsconder()
    {
        gameObject.SetActive(false);
    }

    public void ReiniciarNoCentro(int direcaoHorizontal)
    {
        transform.position = posicaoInicial;
        velocidadeAtual = velocidadeInicial;

        float eixoX = direcaoHorizontal >= 0 ? 1f : -1f;
        float eixoY = Mathf.Abs(direcaoInicial.y) > 0f ? Mathf.Sign(direcaoInicial.y) : 1f;
        direcaoAtual = new Vector2(eixoX, eixoY).normalized;

        gameObject.SetActive(true);
    }

    private Vector2 SortearDirecaoInicialAleatoria()
    {
        float eixoX = Random.value < 0.5f ? -1f : 1f;
        float eixoY = Random.value < 0.5f ? -1f : 1f;
        return new Vector2(eixoX, eixoY).normalized;
    }
}
