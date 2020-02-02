using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public string gameID = "Cthulhu";
    public float gateIncrease = 5f;
    public Game game;

    DataManager dataManager;
    public GameObject playerPrefab;
    public Player player;
    public GameObject endGame;
    //public TextMeshProUGUI text;
    public GameObject win;
    public GameObject lose;
    public enum State {ChooseRole, Playing, End}
    public State gamePhase = State.ChooseRole;

    private void Awake()
    {
        instance = this;
        game = new Game();
    }

    // Start is called before the first frame update
    void Start()
    {
        dataManager = DataManager.instance;
        player = FindObjectOfType<Player>();

        // if there is a gameID in the manager than that will be used. Otherwise it will create a random one
//        dataManager.CreateNewGame(gameID);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateNewGame(string id)
    {
        // create a new game
        if (id == "")
        {
            game.code = Random.Range(1000, 9999).ToString();
        }
        else game.code = id;

        dataManager.WriteInitialGameDataToFirebase();
        gamePhase = State.Playing;

        // instantiate the player
    //    GameObject playerObj = GameObject.Instantiate(playerPrefab, game.playerPosition, Quaternion.identity);
    //    player = playerObj.GetComponent<Player>();
    }

    public void EndGame(){
        gamePhase = State.End;
        endGame.SetActive(true);
        if(GameManager.instance.player.won){
            win.SetActive(true);
        }
        else{
            lose.SetActive(true);
        }
        DataManager.instance.StopListening();
    }

    public void MovePlayer()
    {
        player.transform.position = game.playerPosition;
    }

    public void UpdateGateLevel(Player.Role role)
    {
        switch(role){
            case Player.Role.Insane:
                game.gateLevel += gateIncrease;
                break;
            case Player.Role.Sane:
                game.gateLevel -= gateIncrease;
                break;
        }
    }

}
