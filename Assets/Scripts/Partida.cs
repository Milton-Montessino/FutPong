using UnityEngine;

public class Partida : MonoBehaviour
{
    private enum LadoPontuador
    {
        Esquerda,
        Direita
    }

    [Header("Referencias")]
    [SerializeField] private Bola bola;
    [SerializeField] private Goleiro goleiroEsquerdo;
    [SerializeField] private Goleiro goleiroDireito;

    [Header("Placar")]
    [SerializeField] private int golsEsquerda;
    [SerializeField] private int golsDireita;

    [Header("Tempo da Partida")]
    [SerializeField] private float duracaoPartidaEmSegundos = 180f;

    [Header("Fluxo da Rodada")]
    [SerializeField] private float tempoAntesDoReinicio = 1.5f;

    [Header("Posicoes Iniciais dos Goleiros")]
    [SerializeField] private Vector2 posicaoInicialGoleiroEsquerdo = new Vector2(-25.75f, 0f);
    [SerializeField] private Vector2 posicaoInicialGoleiroDireito = new Vector2(25.75f, 0f);

    private float contadorReinicio;
    private float tempoRestante;
    private bool aguardandoReinicio;
    private int direcaoReinicioBola = 1;

    public int GolsEsquerda => golsEsquerda;
    public int GolsDireita => golsDireita;
    public float TempoRestante => tempoRestante;

    private void Start()
    {
        if (bola == null)
        {
            Debug.LogError("A Partida precisa de uma referencia para a Bola.", this);
            enabled = false;
            return;
        }

        if (goleiroEsquerdo == null || goleiroDireito == null)
        {
            Debug.LogError("A Partida precisa de referencias para os dois goleiros.", this);
            enabled = false;
            return;
        }

        tempoRestante = duracaoPartidaEmSegundos;
        ReiniciarPosicaoDosGoleiros();
    }

    private void Update()
    {
        AtualizarTempoDaPartida();

        if (!aguardandoReinicio)
        {
            return;
        }

        contadorReinicio -= Time.deltaTime;

        if (contadorReinicio > 0f)
        {
            return;
        }

        aguardandoReinicio = false;
        ReiniciarPosicaoDosGoleiros();
        bola.ReiniciarNoCentro(direcaoReinicioBola);
    }

    private void AtualizarTempoDaPartida()
    {
        if (tempoRestante <= 0f)
        {
            tempoRestante = 0f;
            return;
        }

        tempoRestante -= Time.deltaTime;

        if (tempoRestante < 0f)
        {
            tempoRestante = 0f;
        }
    }

    public void RegistrarGolEsquerda()
    {
        RegistrarGol(LadoPontuador.Esquerda);
    }

    public void RegistrarGolDireita()
    {
        RegistrarGol(LadoPontuador.Direita);
    }

    private void RegistrarGol(LadoPontuador ladoPontuador)
    {
        if (ladoPontuador == LadoPontuador.Esquerda)
        {
            golsEsquerda++;
            direcaoReinicioBola = -1;
            Debug.Log($"Gol da esquerda! Placar: {golsEsquerda} x {golsDireita}");
        }
        else
        {
            golsDireita++;
            direcaoReinicioBola = 1;
            Debug.Log($"Gol da direita! Placar: {golsEsquerda} x {golsDireita}");
        }

        contadorReinicio = tempoAntesDoReinicio;
        aguardandoReinicio = true;
    }

    private void ReiniciarPosicaoDosGoleiros()
    {
        goleiroEsquerdo.DefinirPosicao(posicaoInicialGoleiroEsquerdo);
        goleiroDireito.DefinirPosicao(posicaoInicialGoleiroDireito);
    }
}
