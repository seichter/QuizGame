using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public SimpleObjectPool answerButtonObjectPool;
	public Text questionText;
	public Text scoreDisplay;
	public Text timeRemainingDisplay;
	public Transform answerButtonParent;

	public GameObject questionDisplay;
	public GameObject roundEndDisplay;
	public Text highScoreDisplay;

	private DataController dataController;
	private RoundData currentRoundData;
	private QuestionData[] questionPool;

	private bool isRoundActive = false;
	private float timeRemaining;
	private int playerScore;
	private int questionIndex;
	private List<GameObject> answerButtonGameObjects = new List<GameObject>();

	void Start()
	{
		dataController = FindObjectOfType<DataController>(); 

		currentRoundData = dataController.GetCurrentRoundData();
        // Take a copy of the questions so we could shuffle the
        //pool or drop questions from it without affecting the original RoundData object
        questionPool = currentRoundData.questions;		

		timeRemaining = currentRoundData.timeLimitInSeconds;	
		UpdateTimeRemainingDisplay();
		playerScore = 0;
		questionIndex = 0;

		ShowQuestion();
		isRoundActive = true;
	}

	void Update()
	{
		if (isRoundActive)
		{
			timeRemaining -= Time.deltaTime;												
			UpdateTimeRemainingDisplay();

			if (timeRemaining <= 0f)														
			{
				EndRound();
			}
		}
	}

	void ShowQuestion()
	{
		RemoveAnswerButtons();

		QuestionData questionData = questionPool[questionIndex];							
		questionText.text = questionData.questionText;										

		for (int i = 0; i < questionData.answers.Length; i ++)								
		{
			GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();			
			answerButtonGameObjects.Add(answerButtonGameObject);
			answerButtonGameObject.transform.SetParent(answerButtonParent);
			answerButtonGameObject.transform.localScale = Vector3.one;

			AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
			answerButton.SetUp(questionData.answers[i]);									
		}
	}

	void RemoveAnswerButtons()
	{
		while (answerButtonGameObjects.Count > 0)											
		{
			answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
			answerButtonGameObjects.RemoveAt(0);
		}
	}

	public void AnswerButtonClicked(bool isCorrect)
	{
		if (isCorrect)
		{
			playerScore += currentRoundData.pointsAddedForCorrectAnswer;					
			scoreDisplay.text = playerScore.ToString();
		}

		if(questionPool.Length > questionIndex + 1)											
		{
			questionIndex++;
			ShowQuestion();
		}
		else																			
		{
			EndRound();
		}
	}

	private void UpdateTimeRemainingDisplay()
	{
		timeRemainingDisplay.text = Mathf.Round(timeRemaining).ToString();
	}

	public void EndRound()
	{
		isRoundActive = false;
        dataController.SubmitNewPlayerScore(playerScore);
        highScoreDisplay.text = dataController.GethighestScore().ToString();

		questionDisplay.SetActive(false);
		roundEndDisplay.SetActive(true);
	}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene("MenuScreen");
	}

 
}