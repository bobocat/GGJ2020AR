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
    [System.Serializable]
    public enum State {ChooseRole, FirstPhase, SecondPhase, End}
    public State gamePhase = State.ChooseRole;

    public Material monsterMat;
    float totalGateLevel = 100;

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
        gamePhase = State.FirstPhase;
        // instantiate the player
    //    GameObject playerObj = GameObject.Instantiate(playerPrefab, game.playerPosition, Quaternion.identity);
    //    player = playerObj.GetComponent<Player>();
    }

    public void EndGame(){
        gamePhase = State.End;
        endGame.SetActive(true);
        if(GameManager.instance.player.won){
            win.SetActive(true);

            // bring Cthulhu to full opacity! Or seal him away!
            if(GameManager.instance.player.role == Player.Role.Insane){
                UpdateGateLevel(100);
            }
            else{
                UpdateGateLevel(0);
            }
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

    public void IncrementGateLevel(Player.Role role)
    {
        switch(role){
            case Player.Role.Insane:
                UpdateGateLevel(game.gateLevel + gateIncrease);
                break;
            case Player.Role.Sane:
                UpdateGateLevel(game.gateLevel - gateIncrease);
                break;
        }
    }

    public void UpdateGateLevel(float value){
        game.gateLevel = value;
        float frac = game.gateLevel / totalGateLevel;
        monsterMat.color = new Color(monsterMat.color.r, monsterMat.color.b, monsterMat.color.g, (int)Mathf.Lerp(0, 255, frac));
    }

}
