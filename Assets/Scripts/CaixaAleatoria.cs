using UnityEngine;

public class CaixaAleatoria : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Bola bola;
    [SerializeField] private Partida partida;
    [SerializeField] private SpriteRenderer spriteRendererCaixa;

    [Header("Area de Geracao")]
    [SerializeField] private float areaMinX = -10f;
    [SerializeField] private float areaMaxX = 10f;
    [SerializeField] private float areaMinY = -10f;
    [SerializeField] private float areaMaxY = 10f;

    [Header("Temporizadores")]
    [SerializeField] private float intervaloEntreAparicoes = 30f;
    [SerializeField] private float duracaoAtiva = 15f;

    [Header("Tamanho da Caixa")]
    [SerializeField] private float extensaoHorizontal = 0.5f;
    [SerializeField] private float extensaoVertical = 0.5f;

    private float contadorAparicao;
    private float contadorAtiva;
    private bool caixaAtiva;

    private void Start()
    {
        if (spriteRendererCaixa == null)
        {
            spriteRendererCaixa = GetComponent<SpriteRenderer>();
        }

        if (bola == null || partida == null)
        {
            Debug.LogError("A CaixaAleatoria precisa de referencias para Bola e Partida.", this);
            enabled = false;
            return;
        }

        contadorAparicao = intervaloEntreAparicoes;
        DefinirCaixaAtiva(false);
    }

    private void Update()
    {
        if (partida.PartidaEncerrada)
        {
            DefinirCaixaAtiva(false);
            return;
        }

        if (!caixaAtiva)
        {
            AtualizarContadorDeAparicao();
            return;
        }

        AtualizarCaixaAtiva();
    }

    private void AtualizarContadorDeAparicao()
    {
        contadorAparicao -= Time.deltaTime;

        if (contadorAparicao > 0f)
        {
            return;
        }

        MostrarCaixa();
    }

    private void AtualizarCaixaAtiva()
    {
        contadorAtiva -= Time.deltaTime;

        if (BolaAcertouCaixa())
        {
            AplicarModificadorAleatorio();
            EsconderCaixa();
            return;
        }

        if (contadorAtiva > 0f)
        {
            return;
        }

        EsconderCaixa();
    }

    private bool BolaAcertouCaixa()
    {
        if (!bola.EstaAtiva)
        {
            return false;
        }

        Vector2 posicaoBola = bola.transform.position;
        float bolaMinX = posicaoBola.x - bola.ExtensaoHorizontal;
        float bolaMaxX = posicaoBola.x + bola.ExtensaoHorizontal;
        float bolaMinY = posicaoBola.y - bola.ExtensaoVertical;
        float bolaMaxY = posicaoBola.y + bola.ExtensaoVertical;

        float caixaMinX = transform.position.x - extensaoHorizontal;
        float caixaMaxX = transform.position.x + extensaoHorizontal;
        float caixaMinY = transform.position.y - extensaoVertical;
        float caixaMaxY = transform.position.y + extensaoVertical;

        bool sobrepoeHorizontalmente = bolaMaxX >= caixaMinX && bolaMinX <= caixaMaxX;
        bool sobrepoeVerticalmente = bolaMaxY >= caixaMinY && bolaMinY <= caixaMaxY;

        return sobrepoeHorizontalmente && sobrepoeVerticalmente;
    }

    private void AplicarModificadorAleatorio()
    {
        Goleiro ultimoGoleiro = bola.UltimoGoleiroQueTocou;

        if (ultimoGoleiro == null)
        {
            Debug.Log("A caixa foi atingida, mas nenhum goleiro havia tocado na bola ainda.");
            return;
        }

        Partida.TipoModificadorCaixa tipoModificador = Random.value < 0.5f
            ? Partida.TipoModificadorCaixa.DiminuirGol
            : Partida.TipoModificadorCaixa.AumentarGoleiro;

        partida.AplicarModificadorDaCaixa(ultimoGoleiro, tipoModificador);
    }

    private void MostrarCaixa()
    {
        Vector3 novaPosicao = new Vector3(
            Random.Range(areaMinX, areaMaxX),
            Random.Range(areaMinY, areaMaxY),
            transform.position.z
        );

        transform.position = novaPosicao;
        contadorAtiva = duracaoAtiva;
        DefinirCaixaAtiva(true);
    }

    private void EsconderCaixa()
    {
        contadorAparicao = intervaloEntreAparicoes;
        DefinirCaixaAtiva(false);
    }

    private void DefinirCaixaAtiva(bool ativa)
    {
        caixaAtiva = ativa;

        if (spriteRendererCaixa != null)
        {
            spriteRendererCaixa.enabled = ativa;
        }
    }

    private void OnValidate()
    {
        if (areaMinX > areaMaxX)
        {
            areaMaxX = areaMinX;
        }

        if (areaMinY > areaMaxY)
        {
            areaMaxY = areaMinY;
        }

        extensaoHorizontal = Mathf.Max(0.1f, extensaoHorizontal);
        extensaoVertical = Mathf.Max(0.1f, extensaoVertical);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 centroArea = new Vector3(
            (areaMinX + areaMaxX) * 0.5f,
            (areaMinY + areaMaxY) * 0.5f,
            0f
        );
        Vector3 tamanhoArea = new Vector3(
            areaMaxX - areaMinX,
            areaMaxY - areaMinY,
            0f
        );
        Gizmos.DrawWireCube(centroArea, tamanhoArea);

        if (!caixaAtiva)
        {
            return;
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(extensaoHorizontal * 2f, extensaoVertical * 2f, 0f)
        );
    }
}
