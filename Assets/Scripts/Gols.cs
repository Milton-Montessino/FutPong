using UnityEngine;

public class Gols : MonoBehaviour
{
    private enum LadoGol
    {
        Esquerdo,
        Direito
    }

    [Header("Referencias")]
    [SerializeField] private Bola bola;
    [SerializeField] private Partida partida;

    [Header("Configuracao do Gol")]
    [SerializeField] private LadoGol ladoGol = LadoGol.Esquerdo;
    [SerializeField] private float limiteMinX = -30f;
    [SerializeField] private float limiteMaxX = -28f;
    [SerializeField] private float limiteMinY = -15f;
    [SerializeField] private float limiteMaxY = 15f;

    private Vector3 escalaInicial;
    private float limiteMinYInicial;
    private float limiteMaxYInicial;

    private void Awake()
    {
        escalaInicial = transform.localScale;
        limiteMinYInicial = limiteMinY;
        limiteMaxYInicial = limiteMaxY;
    }

    private void Start()
    {
        if (bola == null)
        {
            Debug.LogError("O Gol precisa de uma referencia para a Bola.", this);
            enabled = false;
            return;
        }

        if (partida == null)
        {
            Debug.LogError("O Gol precisa de uma referencia para a Partida.", this);
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (partida == null || partida.PartidaEncerrada)
        {
            return;
        }

        if (bola == null || !bola.EstaAtiva)
        {
            return;
        }

        if (!BolaEntrouNoGol())
        {
            return;
        }

        bola.PararEEsconder();

        if (ladoGol == LadoGol.Esquerdo)
        {
            partida.RegistrarGolDireita();
        }
        else
        {
            partida.RegistrarGolEsquerda();
        }
    }

    private bool BolaEntrouNoGol()
    {
        Vector2 posicaoBola = bola.transform.position;

        float bolaMinX = posicaoBola.x - bola.ExtensaoHorizontal;
        float bolaMaxX = posicaoBola.x + bola.ExtensaoHorizontal;
        float bolaMinY = posicaoBola.y - bola.ExtensaoVertical;
        float bolaMaxY = posicaoBola.y + bola.ExtensaoVertical;

        bool sobrepoeHorizontalmente = bolaMaxX >= limiteMinX && bolaMinX <= limiteMaxX;
        bool sobrepoeVerticalmente = bolaMaxY >= limiteMinY && bolaMinY <= limiteMaxY;

        return sobrepoeHorizontalmente && sobrepoeVerticalmente;
    }

    private void OnValidate()
    {
        if (limiteMinX > limiteMaxX)
        {
            limiteMaxX = limiteMinX;
        }

        if (limiteMinY > limiteMaxY)
        {
            limiteMaxY = limiteMinY;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Vector3 centro = new Vector3(
            (limiteMinX + limiteMaxX) * 0.5f,
            (limiteMinY + limiteMaxY) * 0.5f,
            0f
        );

        Vector3 tamanho = new Vector3(
            limiteMaxX - limiteMinX,
            limiteMaxY - limiteMinY,
            0f
        );

        Gizmos.DrawWireCube(centro, tamanho);
    }

    public void DefinirMultiplicadorAltura(float multiplicador)
    {
        float multiplicadorValido = Mathf.Max(0.1f, multiplicador);
        float centroY = (limiteMinYInicial + limiteMaxYInicial) * 0.5f;
        float meiaAlturaBase = (limiteMaxYInicial - limiteMinYInicial) * 0.5f;
        float meiaAlturaAtual = meiaAlturaBase * multiplicadorValido;

        limiteMinY = centroY - meiaAlturaAtual;
        limiteMaxY = centroY + meiaAlturaAtual;

        Vector3 novaEscala = escalaInicial;
        novaEscala.y = escalaInicial.y * multiplicadorValido;
        transform.localScale = novaEscala;
    }

    public void RestaurarAlturaPadrao()
    {
        DefinirMultiplicadorAltura(1f);
    }
}
