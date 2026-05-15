using UnityEngine;

public class Partida : MonoBehaviour
{
    public enum TipoModificadorCaixa
    {
        DiminuirGol,
        AumentarGoleiro
    }

    private enum LadoPontuador
    {
        Esquerda,
        Direita
    }

    [Header("Referencias")]
    [SerializeField] private Bola bola;
    [SerializeField] private Goleiro goleiroEsquerdo;
    [SerializeField] private Goleiro goleiroDireito;
    [SerializeField] private Gols golEsquerdo;
    [SerializeField] private Gols golDireito;

    [Header("Placar")]
    [SerializeField] private int golsEsquerda;
    [SerializeField] private int golsDireita;

    [Header("Tempo da Partida")]
    [SerializeField] private float duracaoPartidaEmSegundos = 180f;

    [Header("Fluxo da Rodada")]
    [SerializeField] private float tempoAntesDoReinicio = 1.5f;

    [Header("Modificadores Temporarios")]
    [SerializeField] private float duracaoDosModificadores = 30f;
    [SerializeField] private float multiplicadorGoleiroGrande = 1.5f;

    [Header("Indicadores Visuais dos Modificadores")]
    [SerializeField] private GameObject modificadorGolEsquerdo;
    [SerializeField] private GameObject modificadorGoleiroEsquerdo;
    [SerializeField] private GameObject modificadorGolDireito;
    [SerializeField] private GameObject modificadorGoleiroDireito;

    [Header("Posicoes Iniciais dos Goleiros")]
    [SerializeField] private Vector2 posicaoInicialGoleiroEsquerdo = new Vector2(-25.75f, 0f);
    [SerializeField] private Vector2 posicaoInicialGoleiroDireito = new Vector2(25.75f, 0f);

    private float contadorReinicio;
    private float tempoRestante;
    private bool aguardandoReinicio;
    private bool partidaEncerrada;
    private int direcaoReinicioBola = 1;
    private float tempoGolPequenoEsquerda;
    private float tempoGolPequenoDireita;
    private float tempoGoleiroGrandeEsquerda;
    private float tempoGoleiroGrandeDireita;

    public int GolsEsquerda => golsEsquerda;
    public int GolsDireita => golsDireita;
    public float TempoRestante => tempoRestante;
    public bool PartidaEncerrada => partidaEncerrada;

    private void Start()
    {
        if (bola == null)
        {
            Debug.LogError("A Partida precisa de uma referencia para a Bola.", this);
            enabled = false;
            return;
        }

        if (goleiroEsquerdo == null || goleiroDireito == null || golEsquerdo == null || golDireito == null)
        {
            Debug.LogError("A Partida precisa de referencias para os goleiros e gols.", this);
            enabled = false;
            return;
        }

        tempoRestante = duracaoPartidaEmSegundos;
        ReiniciarPosicaoDosGoleiros();
        AtualizarIndicadoresVisuaisDosModificadores();
    }

    private void Update()
    {
        AtualizarTempoDaPartida();
        AtualizarModificadoresTemporarios();

        if (partidaEncerrada)
        {
            return;
        }

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
        if (partidaEncerrada)
        {
            tempoRestante = 0f;
            return;
        }

        if (tempoRestante <= 0f)
        {
            tempoRestante = 0f;
            EncerrarPartida();
            return;
        }

        tempoRestante -= Time.deltaTime;

        if (tempoRestante < 0f)
        {
            tempoRestante = 0f;
            EncerrarPartida();
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
        if (partidaEncerrada)
        {
            return;
        }

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

    private void AtualizarModificadoresTemporarios()
    {
        AtualizarTempoModificador(ref tempoGolPequenoEsquerda, () => golEsquerdo.DesativarGolPequeno());
        AtualizarTempoModificador(ref tempoGolPequenoDireita, () => golDireito.DesativarGolPequeno());
        AtualizarTempoModificador(ref tempoGoleiroGrandeEsquerda, () => goleiroEsquerdo.RestaurarAlturaPadrao());
        AtualizarTempoModificador(ref tempoGoleiroGrandeDireita, () => goleiroDireito.RestaurarAlturaPadrao());
        AtualizarIndicadoresVisuaisDosModificadores();
    }

    private void AtualizarTempoModificador(ref float tempoModificador, System.Action aoExpirar)
    {
        if (tempoModificador <= 0f)
        {
            return;
        }

        tempoModificador -= Time.deltaTime;

        if (tempoModificador > 0f)
        {
            return;
        }

        tempoModificador = 0f;
        aoExpirar.Invoke();
    }

    private void EncerrarPartida()
    {
        if (partidaEncerrada)
        {
            return;
        }

        partidaEncerrada = true;
        aguardandoReinicio = false;
        contadorReinicio = 0f;
        bola.PararEEsconder();
        LimparModificadoresAtivos();
        AtualizarIndicadoresVisuaisDosModificadores();

        if (golsEsquerda > golsDireita)
        {
            Debug.Log($"Fim de partida! Vitoria da esquerda por {golsEsquerda} x {golsDireita}");
        }
        else if (golsDireita > golsEsquerda)
        {
            Debug.Log($"Fim de partida! Vitoria da direita por {golsEsquerda} x {golsDireita}");
        }
        else
        {
            Debug.Log($"Fim de partida! Empate em {golsEsquerda} x {golsDireita}");
        }
    }

    public void AplicarModificadorDaCaixa(Goleiro goleiroAlvo, TipoModificadorCaixa tipoModificador)
    {
        if (partidaEncerrada || goleiroAlvo == null)
        {
            return;
        }

        bool alvoEsquerda = goleiroAlvo == goleiroEsquerdo;
        bool alvoDireita = goleiroAlvo == goleiroDireito;

        if (!alvoEsquerda && !alvoDireita)
        {
            return;
        }

        if (tipoModificador == TipoModificadorCaixa.DiminuirGol)
        {
            if (alvoEsquerda)
            {
                tempoGolPequenoEsquerda = duracaoDosModificadores;
                golEsquerdo.AtivarGolPequeno();
            }
            else
            {
                tempoGolPequenoDireita = duracaoDosModificadores;
                golDireito.AtivarGolPequeno();
            }
        }
        else
        {
            if (alvoEsquerda)
            {
                tempoGoleiroGrandeEsquerda = duracaoDosModificadores;
                goleiroEsquerdo.DefinirMultiplicadorAltura(multiplicadorGoleiroGrande);
            }
            else
            {
                tempoGoleiroGrandeDireita = duracaoDosModificadores;
                goleiroDireito.DefinirMultiplicadorAltura(multiplicadorGoleiroGrande);
            }
        }

        AtualizarIndicadoresVisuaisDosModificadores();
    }

    private void LimparModificadoresAtivos()
    {
        tempoGolPequenoEsquerda = 0f;
        tempoGolPequenoDireita = 0f;
        tempoGoleiroGrandeEsquerda = 0f;
        tempoGoleiroGrandeDireita = 0f;

        golEsquerdo.DesativarGolPequeno();
        golDireito.DesativarGolPequeno();
        goleiroEsquerdo.RestaurarAlturaPadrao();
        goleiroDireito.RestaurarAlturaPadrao();
    }

    private void AtualizarIndicadoresVisuaisDosModificadores()
    {
        DefinirObjetoAtivo(modificadorGolEsquerdo, tempoGolPequenoEsquerda > 0f);
        DefinirObjetoAtivo(modificadorGoleiroEsquerdo, tempoGoleiroGrandeEsquerda > 0f);
        DefinirObjetoAtivo(modificadorGolDireito, tempoGolPequenoDireita > 0f);
        DefinirObjetoAtivo(modificadorGoleiroDireito, tempoGoleiroGrandeDireita > 0f);
    }

    private void DefinirObjetoAtivo(GameObject objeto, bool ativo)
    {
        if (objeto == null)
        {
            return;
        }

        if (objeto.activeSelf == ativo)
        {
            return;
        }

        objeto.SetActive(ativo);
    }
}
