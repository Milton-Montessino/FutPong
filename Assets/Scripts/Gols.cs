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
    [SerializeField] private SpriteRenderer spriteRendererGol;
    [SerializeField] private Sprite spriteGolNormal;
    [SerializeField] private Sprite spriteGolPequeno;

    [Header("Configuracao do Gol")]
    [SerializeField] private LadoGol ladoGol = LadoGol.Esquerdo;

    [Header("Limites Normais")]
    [SerializeField] private float limiteMinX = -30f;
    [SerializeField] private float limiteMaxX = -28f;
    [SerializeField] private float limiteMinY = -15f;
    [SerializeField] private float limiteMaxY = 15f;

    [Header("Limites do Gol Pequeno")]
    [SerializeField] private float limitePequenoMinX = -30f;
    [SerializeField] private float limitePequenoMaxX = -28f;
    [SerializeField] private float limitePequenoMinY = -8f;
    [SerializeField] private float limitePequenoMaxY = 8f;

    private bool golPequenoAtivo;

    private void Awake()
    {
        if (spriteRendererGol == null)
        {
            spriteRendererGol = GetComponent<SpriteRenderer>();
        }
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

        AtualizarVisualDoGol();
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

        ObterLimitesAtuais(out float minX, out float maxX, out float minY, out float maxY);

        bool sobrepoeHorizontalmente = bolaMaxX >= minX && bolaMinX <= maxX;
        bool sobrepoeVerticalmente = bolaMaxY >= minY && bolaMinY <= maxY;

        return sobrepoeHorizontalmente && sobrepoeVerticalmente;
    }

    private void ObterLimitesAtuais(out float minX, out float maxX, out float minY, out float maxY)
    {
        if (golPequenoAtivo)
        {
            minX = limitePequenoMinX;
            maxX = limitePequenoMaxX;
            minY = limitePequenoMinY;
            maxY = limitePequenoMaxY;
            return;
        }

        minX = limiteMinX;
        maxX = limiteMaxX;
        minY = limiteMinY;
        maxY = limiteMaxY;
    }

    private void AtualizarVisualDoGol()
    {
        if (spriteRendererGol == null)
        {
            return;
        }

        if (golPequenoAtivo && spriteGolPequeno != null)
        {
            spriteRendererGol.sprite = spriteGolPequeno;
            return;
        }

        if (spriteGolNormal != null)
        {
            spriteRendererGol.sprite = spriteGolNormal;
        }
    }

    public void AtivarGolPequeno()
    {
        golPequenoAtivo = true;
        AtualizarVisualDoGol();
    }

    public void DesativarGolPequeno()
    {
        golPequenoAtivo = false;
        AtualizarVisualDoGol();
    }

    private void OnValidate()
    {
        CorrigirMinMax(ref limiteMinX, ref limiteMaxX);
        CorrigirMinMax(ref limiteMinY, ref limiteMaxY);
        CorrigirMinMax(ref limitePequenoMinX, ref limitePequenoMaxX);
        CorrigirMinMax(ref limitePequenoMinY, ref limitePequenoMaxY);
    }

    private void CorrigirMinMax(ref float min, ref float max)
    {
        if (min > max)
        {
            max = min;
        }
    }

    private void OnDrawGizmosSelected()
    {
        DesenharGizmoArea(limiteMinX, limiteMaxX, limiteMinY, limiteMaxY, Color.magenta);
        DesenharGizmoArea(limitePequenoMinX, limitePequenoMaxX, limitePequenoMinY, limitePequenoMaxY, Color.yellow);
    }

    private void DesenharGizmoArea(float minX, float maxX, float minY, float maxY, Color cor)
    {
        Gizmos.color = cor;
        Vector3 centro = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
        Vector3 tamanho = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(centro, tamanho);
    }
}
