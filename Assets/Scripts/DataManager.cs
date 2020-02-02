using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;

public class DataManager : MonoBehaviour
{

    public static DataManager instance;
    GameManager gameManager;

    Firebase firebase = Firebase.CreateNew("https://lal-ggj2020.firebaseio.com");

    private FirebaseObserver observer;  // this is the observer that has events attached to it and watches the database for changes. we can turn it on and off

    FirebaseQueue firebaseQueue;

    private void Awake()
    {
        instance = this;
        firebase.OnGetFailed += GetFailHandler;
        firebaseQueue = new FirebaseQueue(true, 3, 1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
    }

     void OnDestroy(){
        StopListening();
    }


    void GetFailHandler(Firebase sender, FirebaseError err)
    {
        Debug.LogError("[ERR] get from key <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    public void WriteGameDataToFirebase()
    {
        Dictionary<string, object> fbGame = new Dictionary<string, object>();

//        fbGame.Add("code", game.code);
        fbGame.Add("playerPosition", gameManager.game.playerPosition);
        fbGame.Add("gateLevel", gameManager.game.gateLevel);
        fbGame.Add("foundMatchingArtifact", gameManager.game.foundArtifact);
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(fbGame);
        firebaseQueue.AddQueueUpdate(firebase.Child(gameManager.game.code, true), json);
    }

    public void GetPlayerPositionFromFB()
    {
        firebase.Child(gameManager.game.code + "/playerPosition", true).GetValue();
//        game.playerPosition = 
    }

    public void UpdatePlayerPosition()
    {

    }

    
    public void StartListening()
    {
        observer = new FirebaseObserver(firebase.Child(gameManager.game.code), 1f);
        observer.OnChange += (Firebase sender, DataSnapshot snapshot) =>
        {
            Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        /*    System.Enum.TryParse<Player.Role>((string)dict["artifact1"],false,out Player.Role role);*/
            GameManager.instance.game.gateLevel = System.Convert.ToSingle(dict["gateLevel"]);
            GameManager.instance.player.SetArtifactToFind((string)dict["artifact1"]);

            //Debug.Log("playerposx: " + game.playerPosition);


/*
                        // are all the bets in?
                        if (answerCount == dict.Count)
                        {
                            observer.Stop();
                        }
            */
        };

        observer.Start();
    }

    public void StopListening()
    {
        observer.Stop();
    }
}
