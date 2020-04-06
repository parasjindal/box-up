using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject newBox;
    private GameObject currentlyActiveBox;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject pauseButton;
    public Text winText;
    public Text easyScore;
    public Text mediumScore;
    public Text hardScore;
    private bool isCoroutineActive = false;
    public Text currentBoxText;
    private int currentBoxCount = 0;
    private bool hasWon = false;
    private int level = 1;
    private bool isPaused = false;
    private bool isHighScoreOpen = false;

    int score = 0;
    private Vector3 originalPosition = new Vector3(0, 3, 2);
    private List<double> positions = new List<double>();

    // Start is called before the first frame update
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "Game") {
            level = PlayerPrefs.GetInt("level");
            currentlyActiveBox = GameObject.Find("BoxSprite");
            if(level > 1) {
                Collider2D collider2D = currentlyActiveBox.GetComponent<Collider2D>();
                collider2D.sharedMaterial.bounciness = 0.1f;
                collider2D.enabled = false;
                collider2D.enabled = true;
            }
            if(level == 3) {
                currentlyActiveBox.transform.localScale = new Vector3(0.125f, 0.125f, 1);
            }
        }
        
        if(scene.buildIndex == 3) {
            print(PlayerPrefs.GetInt("level_easy").ToString());
            string easyEfficiency = PlayerPrefs.GetInt("level_easy").ToString() + "%";
            easyScore.text = easyEfficiency;

            string mediumEfficiency = PlayerPrefs.GetInt("level_medium").ToString() + "%";
            mediumScore.text = mediumEfficiency;

            string hardEfficiency = PlayerPrefs.GetInt("level_hard").ToString() + "%";
            hardScore.text = hardEfficiency;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Win() {
        hasWon = true;
        winMenu.SetActive(true);
        pauseButton.SetActive(false);
        int efficiency = CalculateScore();
        winText.text = "Efficiency: " + efficiency.ToString() + "%";
        if(level == 1) {
            if(efficiency > PlayerPrefs.GetInt("level_easy", 0)) {
                PlayerPrefs.SetInt("level_easy", efficiency);
            }
        } if(level == 2) {
            if(efficiency > PlayerPrefs.GetInt("level_medium", 0)) {
                PlayerPrefs.SetInt("level_medium", efficiency);
            }
        } if(level == 3) {
            if(efficiency > PlayerPrefs.GetInt("level_hard", 0)) {
                PlayerPrefs.SetInt("level_hard", efficiency);
            }
        }
    }

    public void Restart() {
        SceneManager.LoadScene("Game");
        currentlyActiveBox = GameObject.Find("BoxSprite");
        if(level > 1) {
            Collider2D collider2D = currentlyActiveBox.GetComponent<Collider2D>();
            collider2D.sharedMaterial.bounciness = 0.1f;
            collider2D.enabled = false;
            collider2D.enabled = true;
        }
        if(level == 3) {
            currentlyActiveBox.transform.localScale = new Vector3(0.125f, 0.125f, 1);
        }
    }

    public void Exit() {
        SceneManager.LoadScene(0);
    }

    public void Pause() {
        isPaused = true;
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void Resume() {
        pauseButton.SetActive(true);
        isPaused = false;
        pauseMenu.SetActive(false);
    }

    public void LevelScreen() {
        SceneManager.LoadScene(2);
    }

    public void HighScores() {
        SceneManager.LoadScene(3);
    }

    public void OnGameAreaClicked() {
        if(hasWon || isPaused) {
            return;
        }
        if( currentlyActiveBox != null) {
            currentlyActiveBox.GetComponent<Box>().Release();
            positions.Add(Math.Round(currentlyActiveBox.transform.position.x, 2));
            currentlyActiveBox = null;
            currentBoxCount++;
            currentBoxText.text = currentBoxCount.ToString();
            StartCoroutine(ExampleCoroutine());
        }
    }

    private IEnumerator ExampleCoroutine()
    {
        if(isCoroutineActive) {
            yield break;
        }
        isCoroutineActive = true;
        yield return new WaitForSeconds(2);
        currentlyActiveBox = Instantiate(newBox, originalPosition, Quaternion.identity) as GameObject;
        isCoroutineActive = false;
    }

    public void SetLevel(int _level) {
        PlayerPrefs.SetInt("level", _level);
        SceneManager.LoadScene("Game");
    }

    public int GetLevel() {
        return level;
    }

    private int CalculateScore() {
        int minBoxReq = 9;
        int maxScore = 68;
        double precision = 0.02;
        if(level != 0) {
            minBoxReq = 11;
            maxScore = 60;
        }
        int extraBoxes = currentBoxCount - minBoxReq;
        int score = maxScore - 5*extraBoxes;
        int precisionPoints = 0;
        for(int i = 1; i < minBoxReq; i++) {
            if(positions[i-1] <= positions[i] + precision && positions[i-1] >= positions[i] - precision) {
                precisionPoints = precisionPoints + 4;
            }
        }
        score = score + precisionPoints;
        if(score > 100) {
            score = 100;
        }
        if(score < 0) {
            score = 0;
        }
        return score;
    }
}
