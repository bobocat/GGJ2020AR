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

    public void WriteInitialGameDataToFirebase(){
                Dictionary<string, object> fbGame = new Dictionary<string, object>();

//        fbGame.Add("code", game.code);
        fbGame.Add("playerPosition", gameManager.game.playerPosition);
        fbGame.Add("gateLevel", gameManager.game.gateLevel);
        fbGame.Add("foundMatchingArtifact", false);
        fbGame.Add("artifact1", "");
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(fbGame);
        firebaseQueue.AddQueueUpdate(firebase.Child(gameManager.game.code, true), json);

    }

        public void WriteValueToFirebase(string key, object value){
                Dictionary<string, object> fbGame = new Dictionary<string, object>();

//        fbGame.Add("code", game.code);
        fbGame.Add(key,value);

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

    void OnChange(Firebase sender, DataSnapshot snapshot) {
        if(GameManager.instance.gamePhase != GameManager.State.End){
            Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        /*    System.Enum.TryParse<Player.Role>((string)dict["artifact1"],false,out Player.Role role);*/
           GameManager.instance.UpdateGateLevel( System.Convert.ToSingle(dict["gateLevel"]));

            // if it is time to find a matching artifact, set it and enter second phase
            if(((string)dict["artifact1"]).ToLower() == "found"){
                GameManager.instance.gamePhase = GameManager.State.SecondPhase;
                GameManager.instance.player.SetArtifactToFind();
            }
            bool isEnd = System.Convert.ToBoolean(dict["foundMatchingArtifact"]);
            if(isEnd){
                GameManager.instance.EndGame();
                observer.OnChange -= OnChange;
            }
        }
        }
    public void StartListening()
    {
        observer = new FirebaseObserver(firebase.Child(gameManager.game.code), 1f);
        observer.OnChange += OnChange;

            //Debug.Log("playerposx: " + game.playerPosition);


/*
                        // are all the bets in?
                        if (answerCount == dict.Count)
                        {
                            observer.Stop();
                        }
            */

        observer.Start();
    }

    public void StopListening()
    {
        observer.Stop();
    }

    public void AskForValue(){
        firebase.Child(gameManager.game.code, true).GetValue();
        firebase.OnGetSuccess += UpdateGateLevel;
    }

    void UpdateGateLevel(Firebase sender, DataSnapshot snapshot){
            firebase.OnGetSuccess -= UpdateGateLevel;
            Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
            GameManager.instance.UpdateGateLevel(System.Convert.ToSingle(dict["gateLevel"]));
            GameManager.instance.IncrementGateLevel(GameManager.instance.player.role);
            DataManager.instance.WriteGameDataToFirebase();
    }

}
