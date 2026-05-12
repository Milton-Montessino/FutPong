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

        AtualizarTextos();
    }

    private void Update()
    {
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
}
