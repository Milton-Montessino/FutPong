using UnityEngine;

public class Campo : MonoBehaviour
{
    [Header("Limites Externos do Campo")]
    [SerializeField] private float limiteEsquerdo = -8f;
    [SerializeField] private float limiteDireito = 8f;
    [SerializeField] private float limiteSuperior = 4f;
    [SerializeField] private float limiteInferior = -4f;

    [Header("Divisao do Campo")]
    [SerializeField] private float linhaCentralX = 0f;

    [Header("Area Proibida do Gol Esquerdo")]
    [SerializeField] private float golEsquerdoMinX = -8f;
    [SerializeField] private float golEsquerdoMaxX = -7f;
    [SerializeField] private float golEsquerdoMinY = -0.8f;
    [SerializeField] private float golEsquerdoMaxY = 0.8f;

    [Header("Area Proibida do Gol Direito")]
    [SerializeField] private float golDireitoMinX = 7f;
    [SerializeField] private float golDireitoMaxX = 8f;
    [SerializeField] private float golDireitoMinY = -0.8f;
    [SerializeField] private float golDireitoMaxY = 0.8f;

    public float LimiteEsquerdo => limiteEsquerdo;
    public float LimiteDireito => limiteDireito;
    public float LimiteSuperior => limiteSuperior;
    public float LimiteInferior => limiteInferior;
    public float LinhaCentralX => linhaCentralX;

    public Vector2 LimitarGoleiroEsquerdo(Vector2 posicao)
    {
        return LimitarGoleiroEsquerdo(posicao, Vector2.zero);
    }

    public Vector2 LimitarGoleiroEsquerdo(Vector2 posicao, Vector2 meiaExtensao)
    {
        return LimitarGoleiroEsquerdo(posicao, meiaExtensao.x, meiaExtensao.x, meiaExtensao.y, meiaExtensao.y);
    }

    public Vector2 LimitarGoleiroEsquerdo(
        Vector2 posicao,
        float extensaoEsquerda,
        float extensaoDireita,
        float extensaoBaixo,
        float extensaoCima
    )
    {
        Vector2 posicaoLimitada = new Vector2(
            Mathf.Clamp(posicao.x, limiteEsquerdo + extensaoEsquerda, linhaCentralX - extensaoDireita),
            Mathf.Clamp(posicao.y, limiteInferior + extensaoBaixo, limiteSuperior - extensaoCima)
        );

        if (EstaNaAreaDoGolEsquerdo(posicaoLimitada, extensaoEsquerda, extensaoDireita, extensaoBaixo, extensaoCima))
        {
            posicaoLimitada.x = golEsquerdoMaxX + extensaoEsquerda;
        }

        return posicaoLimitada;
    }

    public Vector2 LimitarGoleiroDireito(Vector2 posicao)
    {
        return LimitarGoleiroDireito(posicao, Vector2.zero);
    }

    public Vector2 LimitarGoleiroDireito(Vector2 posicao, Vector2 meiaExtensao)
    {
        return LimitarGoleiroDireito(posicao, meiaExtensao.x, meiaExtensao.x, meiaExtensao.y, meiaExtensao.y);
    }

    public Vector2 LimitarGoleiroDireito(
        Vector2 posicao,
        float extensaoEsquerda,
        float extensaoDireita,
        float extensaoBaixo,
        float extensaoCima
    )
    {
        Vector2 posicaoLimitada = new Vector2(
            Mathf.Clamp(posicao.x, linhaCentralX + extensaoEsquerda, limiteDireito - extensaoDireita),
            Mathf.Clamp(posicao.y, limiteInferior + extensaoBaixo, limiteSuperior - extensaoCima)
        );

        if (EstaNaAreaDoGolDireito(posicaoLimitada, extensaoEsquerda, extensaoDireita, extensaoBaixo, extensaoCima))
        {
            posicaoLimitada.x = golDireitoMinX - extensaoDireita;
        }

        return posicaoLimitada;
    }

    public Vector2 LimitarPosicaoNoCampo(Vector2 posicao)
    {
        return new Vector2(
            Mathf.Clamp(posicao.x, limiteEsquerdo, limiteDireito),
            Mathf.Clamp(posicao.y, limiteInferior, limiteSuperior)
        );
    }

    public bool EstaNaAreaDoGolEsquerdo(Vector2 posicao)
    {
        return EstaNaAreaDoGolEsquerdo(posicao, Vector2.zero);
    }

    public bool EstaNaAreaDoGolEsquerdo(Vector2 posicao, Vector2 meiaExtensao)
    {
        return EstaNaAreaDoGolEsquerdo(posicao, meiaExtensao.x, meiaExtensao.x, meiaExtensao.y, meiaExtensao.y);
    }

    public bool EstaNaAreaDoGolEsquerdo(
        Vector2 posicao,
        float extensaoEsquerda,
        float extensaoDireita,
        float extensaoBaixo,
        float extensaoCima
    )
    {
        float esquerda = posicao.x - extensaoEsquerda;
        float direita = posicao.x + extensaoDireita;
        float inferior = posicao.y - extensaoBaixo;
        float superior = posicao.y + extensaoCima;

        return direita >= golEsquerdoMinX &&
            esquerda <= golEsquerdoMaxX &&
            superior >= golEsquerdoMinY &&
            inferior <= golEsquerdoMaxY;
    }

    public bool EstaNaAreaDoGolDireito(Vector2 posicao)
    {
        return EstaNaAreaDoGolDireito(posicao, Vector2.zero);
    }

    public bool EstaNaAreaDoGolDireito(Vector2 posicao, Vector2 meiaExtensao)
    {
        return EstaNaAreaDoGolDireito(posicao, meiaExtensao.x, meiaExtensao.x, meiaExtensao.y, meiaExtensao.y);
    }

    public bool EstaNaAreaDoGolDireito(
        Vector2 posicao,
        float extensaoEsquerda,
        float extensaoDireita,
        float extensaoBaixo,
        float extensaoCima
    )
    {
        float esquerda = posicao.x - extensaoEsquerda;
        float direita = posicao.x + extensaoDireita;
        float inferior = posicao.y - extensaoBaixo;
        float superior = posicao.y + extensaoCima;

        return direita >= golDireitoMinX &&
            esquerda <= golDireitoMaxX &&
            superior >= golDireitoMinY &&
            inferior <= golDireitoMaxY;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 centroCampo = new Vector3(
            (limiteEsquerdo + limiteDireito) * 0.5f,
            (limiteInferior + limiteSuperior) * 0.5f,
            0f
        );
        Vector3 tamanhoCampo = new Vector3(
            limiteDireito - limiteEsquerdo,
            limiteSuperior - limiteInferior,
            0f
        );
        Gizmos.DrawWireCube(centroCampo, tamanhoCampo);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            new Vector3(linhaCentralX, limiteInferior, 0f),
            new Vector3(linhaCentralX, limiteSuperior, 0f)
        );

        Gizmos.color = Color.red;
        DesenharAreaGol(golEsquerdoMinX, golEsquerdoMaxX, golEsquerdoMinY, golEsquerdoMaxY);
        DesenharAreaGol(golDireitoMinX, golDireitoMaxX, golDireitoMinY, golDireitoMaxY);
    }

    private void DesenharAreaGol(float minX, float maxX, float minY, float maxY)
    {
        Vector3 centro = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
        Vector3 tamanho = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(centro, tamanho);
    }
}
