using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI loopText;
    [SerializeField] TextMeshProUGUI tempoText;
    int loop = 0;

    public void UpdateLoop(float newTempo)
    {
        loop++;
        loopText.text = "Loop: " + loop;
        tempoText.text = "Tempo: " + (int) newTempo;
    }

    public void RestartGame()
    {
        StartCoroutine(WaitToRestart());
    }

    IEnumerator WaitToRestart()
    {
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
