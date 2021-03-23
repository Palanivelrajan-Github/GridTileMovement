using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountDownTest : MonoBehaviour
{
    public int timeLeftForTake10;

    public Text timertext;

    private float _currentTime;

    private void Start()
    {
        StartCoroutine(Timer());
    }


    private IEnumerator Timer()
    {
        _currentTime = timeLeftForTake10;

        while (_currentTime > 0)
        {
            _currentTime -= Time.deltaTime;
            timerDisplay();
            yield return null;
        }
    }

    private void timerDisplay()
    {
        var timeInMinutes = ((int) (_currentTime / 60) % 60).ToString("00");
        var timeInSeconds = ((int) _currentTime % 60).ToString("00");
        timertext.text = $"{timeInMinutes}:{timeInSeconds}";
    }
}