using UnityEngine;
using UnityEngine.UI;

public class PlacarUi : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Partida partida;

    [Header("Textos")]
    [SerializeField] private Text textoGolsEsquerda;
    [SerializeField] private Text textoTempo;
    [SerializeField] private Text textoGolsDireita;
    [SerializeField] private Text textoMensagemGol;
    [SerializeField] private Text textoMensagemFinal;

    [Header("Mensagem de Gol")]
    [SerializeField] private float duracaoMensagemGol = 1.8f;
    [SerializeField] private string mensagemGolEsquerda = "GOL! DA ESQUERDA!";
    [SerializeField] private string mensagemGolDireita = "GOL! DA DIREITA!";

    [Header("Mensagem Final")]
    [SerializeField] private string mensagemVitoriaEsquerda = "VITORIA DOS PETRALHA!";
    [SerializeField] private string mensagemVitoriaDireita = "VITORIA DOS BOLSOMINIONS!";
    [SerializeField] private string mensagemEmpate = "EMPATE!";

    private int ultimoGolsEsquerda;
    private int ultimoGolsDireita;
    private float contadorMensagemGol;
    private bool mensagemFinalMostrada;

    private void Start()
    {
        if (partida == null)
        {
            Debug.LogError("O PlacarUi precisa de uma referencia para a Partida.", this);
            enabled = false;
            return;
        }

        if (textoGolsEsquerda == null || textoTempo == null || textoGolsDireita == null)
        {
            Debug.LogError("O PlacarUi precisa de referencias para todos os textos.", this);
            enabled = false;
            return;
        }

        ultimoGolsEsquerda = partida.GolsEsquerda;
        ultimoGolsDireita = partida.GolsDireita;
        DefinirMensagemGolAtiva(false);
        DefinirMensagemFinalAtiva(false);
        AtualizarTextos();
    }

    private void Update()
    {
        VerificarFimDaPartida();
        VerificarNovoGol();
        AtualizarMensagemGol();
        AtualizarTextos();
    }

    private void AtualizarTextos()
    {
        textoGolsEsquerda.text = FormatarGols(partida.GolsEsquerda);
        textoGolsDireita.text = FormatarGols(partida.GolsDireita);
        textoTempo.text = FormatarTempo(partida.TempoRestante);
    }

    private string FormatarGols(int gols)
    {
        return gols.ToString("00");
    }

    private string FormatarTempo(float tempoEmSegundos)
    {
        int tempoInteiro = Mathf.CeilToInt(tempoEmSegundos);
        int minutos = tempoInteiro / 60;
        int segundos = tempoInteiro % 60;
        return $"{minutos:00}:{segundos:00}";
    }

    private void VerificarNovoGol()
    {
        if (partida.PartidaEncerrada)
        {
            return;
        }

        if (partida.GolsEsquerda > ultimoGolsEsquerda)
        {
            MostrarMensagemGol(mensagemGolEsquerda);
        }
        else if (partida.GolsDireita > ultimoGolsDireita)
        {
            MostrarMensagemGol(mensagemGolDireita);
        }

        ultimoGolsEsquerda = partida.GolsEsquerda;
        ultimoGolsDireita = partida.GolsDireita;
    }

    private void MostrarMensagemGol(string mensagem)
    {
        if (textoMensagemGol == null)
        {
            return;
        }

        textoMensagemGol.text = mensagem;
        contadorMensagemGol = duracaoMensagemGol;
        DefinirMensagemGolAtiva(true);
    }

    private void AtualizarMensagemGol()
    {
        if (textoMensagemGol == null)
        {
            return;
        }

        if (contadorMensagemGol <= 0f)
        {
            return;
        }

        contadorMensagemGol -= Time.deltaTime;

        if (contadorMensagemGol > 0f)
        {
            return;
        }

        contadorMensagemGol = 0f;
        DefinirMensagemGolAtiva(false);
    }

    private void DefinirMensagemGolAtiva(bool ativa)
    {
        if (textoMensagemGol == null)
        {
            return;
        }

        textoMensagemGol.enabled = ativa;
    }

    private void VerificarFimDaPartida()
    {
        if (!partida.PartidaEncerrada || mensagemFinalMostrada)
        {
            return;
        }

        mensagemFinalMostrada = true;
        DefinirMensagemGolAtiva(false);
        MostrarMensagemFinal(ObterMensagemFinalPorResultado());
    }

    private string ObterMensagemFinalPorResultado()
    {
        if (partida.GolsEsquerda > partida.GolsDireita)
        {
            return mensagemVitoriaEsquerda;
        }

        if (partida.GolsDireita > partida.GolsEsquerda)
        {
            return mensagemVitoriaDireita;
        }

        return mensagemEmpate;
    }

    private void MostrarMensagemFinal(string mensagem)
    {
        if (textoMensagemFinal == null)
        {
            return;
        }

        textoMensagemFinal.text = mensagem;
        DefinirMensagemFinalAtiva(true);
    }

    private void DefinirMensagemFinalAtiva(bool ativa)
    {
        if (textoMensagemFinal == null)
        {
            return;
        }

        textoMensagemFinal.enabled = ativa;
    }
}
