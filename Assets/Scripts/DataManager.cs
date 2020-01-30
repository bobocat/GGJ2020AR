using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;

public class DataManager : MonoBehaviour
{

    public Game game;

    Firebase firebase = Firebase.CreateNew("https://lal-ggj2020.firebaseio.com");

    private FirebaseObserver observer;  // this is the observer that has events attached to it and watches the database for changes. we can turn it on and off

    FirebaseQueue firebaseQueue;

    // Start is called before the first frame update
    void Start()
    {
        game = new Game();

        firebase.OnGetFailed += GetFailHandler;
        firebaseQueue = new FirebaseQueue(true, 3, 1f);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateNewGame()
    {
        game.code = Random.Range(1000, 9999).ToString();
        WriteGameDataToFirebase();
    }

    void GetFailHandler(Firebase sender, FirebaseError err)
    {
        Debug.LogError("[ERR] get from key <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    public void WriteGameDataToFirebase()
    {
        Dictionary<string, object> fbGame = new Dictionary<string, object>();

        fbGame.Add("code", game.code);
        fbGame.Add("playerPosition", game.playerPosition);

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(fbGame);
        firebaseQueue.AddQueueUpdate(firebase.Child(game.code, true), json);
    }

    public void GetPlayerPositionFromFB()
    {
        firebase.Child(game.code + "playerPosition", true).GetValue();
//        game.playerPosition = 
    }

    public void UpdatePlayerPosition()
    {

    }


    public void ListenForPlayerPosition()
    {
        observer = new FirebaseObserver(firebase.Child(game.code + "/playerPosition"), 1f);
        observer.OnChange += (Firebase sender, DataSnapshot snapshot) =>
        {
            Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();

//            int answerCount = 0;   // count how many bets are in

/*
            // set all the teams with bets in to checked mode
            string answer;
            for (int i = 0; i < dict.Count; i++)
            {
                answer = dict["answer" + (i + 1)].ToString();
                if (answer != "")
                {
                    gameManager.teamList[i].SetTeamStatus(Team.statusType.boxOnlyChecked);
                    game.team[i].answer = answer;
                    answerCount++;
                }
            }

            // are all the bets in?
            if (answerCount == dict.Count)
            {
                observer.Stop();
            }
*/
        };

        observer.Start();
    }

}
