using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Linq;

public class GameLoop : MonoBehaviour
{
    public TMP_Text instructions;
    public GameObject buttonPrefab;
    public GameObject tile;
    public GameObject InGameUI;
    public Image playerTurn;
    public TMP_Text playerName;
    public TMP_Text locationResources;
    public TMP_Text shipStats;
    void Start()
    {
        instructions.text = "";
        StartCoroutine(GameProcess());
        InGameUI.SetActive(false);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        int x = Mathf.FloorToInt(mousePos.x) + 4;
        int y = Mathf.FloorToInt(mousePos.y) + 4;
        if (universe != null && x >= 0 && y >= 0 && x < universe.universeGrid.GetLength(0) && y < universe.universeGrid.GetLength(1))
        {
            Debug.Log("Tile: (" + x + ", " + y + ") SystemType: " + universe.universeGrid[x, y].systemType);

            string control= "Unocupied";
            if (universe.universeGrid[x, y].PlayerInControl != null)
            {
                control = universe.universeGrid[x, y].PlayerInControl.name;
            }
            locationResources.text =
                
                "Control: " + control + "<br>" +
                "System Type " + universe.universeGrid[x, y].systemType +"<br>"+
                "Common: " + universe.universeGrid[x, y].common + "<br>" +
                "Rare: " + universe.universeGrid[x, y].rare + "<br>" +
                "Very Rare: " + universe.universeGrid[x, y].veryRare + "<br>" +
                "Engines: " + universe.universeGrid[x, y].engines + "<br>" +
                "Sheilds: " + universe.universeGrid[x, y].sheilds + "<br>" +
                "Armor: " + universe.universeGrid[x, y].armor + "<br>" +
                "BeamWeapon: " + universe.universeGrid[x, y].beamWeapon+ "<br>" +
                "MissileWeapon: " + universe.universeGrid[x, y].missileWeapon + "<br>" +
                "AntiMissileSystems: " + universe.universeGrid[x, y].antiMissileSystems + "<br>" +
                "Marines: " + universe.universeGrid[x, y].marines+ "<br>" +

                ""
                ;
            locationResources.text += "Facilities: <br>";
            if(universe.universeGrid[x, y].Counters.Count == 0)
            {
                locationResources.text += "None <br>";
            }
            foreach(Counter c in universe.universeGrid[x, y].Counters)
            {
                locationResources.text += c + "<br>";
            }

            List<Ship> ships = universe.universeGrid[x, y].Ships.OrderByDescending(x => (x.PlayerInControl.name)).ToList();
            shipStats.text = "";
            foreach (Ship s in ships)
            {

                shipStats.text +=
                    "Name: " + s.Name + "<br>" +
                    "Control: " + s.PlayerInControl.name + "<br>" +
                    "Common: " + s.common + "<br>" +
                    "Rare: " + s.rare + "<br>" +
                    "Very Rare: " + s.veryRare + "<br>" +
                    "Engines: " + s.engines + "<br>" +
                    "Sheilds: " + s.sheilds + "<br>" +
                    "Armor: " + s.armor + "<br>" +
                    "BeamWeapon: " + s.beamWeapon + "<br>" +
                    "MissileWeapon: " + s.missileWeapon + "<br>" +
                    "AntiMissileSystems: " + s.antiMissileSystems + "<br>" +
                    "Marines: " + s.marines + "<br>" +

                "<br>"
                ;
            }





        }


    }





    Universe universe;
    UnityEvent destroyButtons = new();
    List<Player> players = new();
    bool doesShipHaveFreeSpace(Ship ship)
    {
        int inventorySlotsFull = (ship.common + 9) / 10 + (ship.rare + 9) / 10 + (ship.veryRare + 9) / 10
            + ship.antiMissileSystems + ship.armor + ship.beamWeapon + ship.engines
            + ship.marines + ship.missileWeapon + ship.sheilds;
        return 6 > inventorySlotsFull;
    }
    IEnumerator GameProcess() 
    {
        
        instructions.text = "How many players?";
        int playerCount = 0;
        CreateNewButton("1", new Vector3(-3, 0, 0), () => { playerCount = 1; });
        CreateNewButton("2", new Vector3(-1, 0, 0), () => { playerCount = 2; });
        CreateNewButton("3", new Vector3(1, 0, 0), () => { playerCount = 3; });
        CreateNewButton("4", new Vector3(3, 0, 0), () => { playerCount = 4; });
        yield return new WaitUntil(() => playerCount != 0);
        instructions.text = "Behold, the universe";
        destroyButtons.Invoke();
        universe = new();
        for (int i = 0; i<4; i++)
        {
            Player newPlayer = new();
            if(i<playerCount) newPlayer.isHuman = true;
            players.Add(newPlayer);
        }
        for (int i = 0; i < players.Count; i++)
        {
            players[i].color = new Color(Random.value, Random.value, Random.value);
            Debug.Log("This character is human? " + players[i].isHuman +" Color:" + players[i].color);
        }
        players[0].name = "Collective";
        players[1].name = "Federation";
        players[2].name = "Progenetors";
        players[3].name = "Vespiary";
        Debug.Log("There are " + playerCount + " humans");




        InGameUI.SetActive(true);
        
        universe.DisplayUniverse(tile);

        //Game Start. Choose a home system. There place markers for Home system, Space Dock, Mine, Barracks, Static Defense System.
        yield return new WaitForSeconds(1.645f);

        instructions.text = "Choose a home system";


        // loop through each player once and choose a home system
        players.Sort((a, b) => 1 - 2 * Random.Range(0, 2));
        foreach (Player p in players)
        {
            SetTurnMarker(p);

            bool chooseHome = false;

            for (int x = 0; x < universe.universeGrid.GetLength(0); x++) {
                for (int y = 0; y < universe.universeGrid.GetLength(1); y++) {

                    if (universe.universeGrid[x,y].systemType != SystemType.Empty&& universe.universeGrid[x, y].systemType != SystemType.Home)
                    {
                        int bx = x;
                        int by = y;
                        CreateNewButton("Home?", new Vector3(x-3.5f, y-3.5f, 0), () => 
                        {
                            Debug.Log("You chose " +bx+ ", " +by+ " as your home system");
                            chooseHome = true;
                            universe.universeGrid[bx, by].PlayerInControl = p;
                            universe.universeGrid[bx, by].systemType = SystemType.Home;
                            universe.universeGrid[bx, by].Counters.Add(Counter.HomeSystem);
                            universe.universeGrid[bx, by].Counters.Add(Counter.Barracks);
                            universe.universeGrid[bx, by].Counters.Add(Counter.Mine);
                            universe.universeGrid[bx, by].Counters.Add(Counter.SpaceDock);
                            universe.universeGrid[bx, by].Counters.Add(Counter.StaticDefenceSystem);
                        });
                    } 
                }
            }
        yield return new WaitUntil(() => chooseHome == true);
            universe.DestroyUniverse();
            universe.DisplayUniverse(tile);
            destroyButtons.Invoke();
            yield return new WaitForSeconds(0.15f);
        }
        bool victoryCondition = false;

        while (victoryCondition == false)
        {
            //Phase 1
            
            players.Sort((a, b) => 1 - 2 * Random.Range(0, 2));
            //collect resources
            //go through all of the locations and generate resources for each location.
            for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
            {
                for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                {
                    PlaySpace p = universe.universeGrid[x, y];
                    if (p.PlayerInControl != null && p.Counters.Contains(Counter.Mine))
                    {
                        instructions.text = "Mining resources in sector ("+x+", "+y+")";
                        yield return new WaitForSeconds(.4f);
                        instructions.text = "";
                        switch (p.systemType)
                        {
                            case SystemType.Empty:
                                break;
                            case SystemType.Green:
                                p.common += 30;
                                p.rare += 20;
                                p.veryRare += 10;
                                break;
                            case SystemType.Yellow:
                                p.common += 20;
                                p.rare += 20;
                                p.veryRare += 20;
                                break;
                            case SystemType.Blue:
                                p.common += 0;
                                p.rare += 20;
                                p.veryRare += 0;
                                break;
                            case SystemType.Red:
                                p.common += 0;
                                p.rare += 0;
                                p.veryRare += 20;
                                break;
                            case SystemType.Home:
                                p.common += 5000;
                                p.rare += 3000;
                                p.veryRare += 2000;
                                break;
                        }
                        yield return new WaitForSeconds(0.14f);

                    }
                }
            }
            
            //completeTurn = false;
            foreach (Player activePlayer in players)
            {
                SetTurnMarker(activePlayer);
                //spend resources, resources must be available at that location

                bool finishStep;


                #region SHIP CONSTRUCTION
                //Building Ships
                finishStep = false;
                while (finishStep == false)
                {

                    instructions.text = "Constructing Ships";
                    bool buttonClicked = false;

                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed Ship Building");
                        instructions.text = "Completed Ship Construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });
                    
                    //Generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                if (playSpace.common >= 150 && playSpace.rare >= 90 && playSpace.veryRare >= 60)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        Debug.Log("Constructing ship in sector" + bx + ", " + by);
                                        instructions.text = "Constructing Ship in sector (" + x + ", " + y + ")";
                                        Ship ship = new();
                                        ship.Name = GenerateNewName();
                                        ship.PlayerInControl = activePlayer;
                                        playSpace.Ships.Add(ship);
                                        playSpace.common -= 150;
                                        playSpace.rare -= 90;
                                        playSpace.veryRare -= 60;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }





                    // make sure total ships is less than 6
                    int totalShips = 0;
                    foreach (PlaySpace playSpace in universe.universeGrid)
                    {
                        foreach (Ship ship in playSpace.Ships)
                        {
                            if (ship.PlayerInControl == activePlayer)
                            {
                                totalShips++;
                            }
                        }
                    }
                    if (totalShips >= 6)
                    {
                        buttonClicked = true;
                        finishStep = true;
                        destroyButtons.Invoke();
                        instructions.text= activePlayer.name + " is at maximum ship count.";
                    }

                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region ENGINE CONSTRUCTION
                //Building Engines
                finishStep = false;
                while (finishStep == false)
                {
                    instructions.text = "Constructing Engines";
                    int cCost = 30;
                    int rCost = 10;
                    int vCost = 0;
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.engines++;
                    }
                    string constructName = "engine";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed " + constructName + " Building");
                        instructions.text = "Completed " + constructName + " Construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= rCost && playSpace.veryRare >= vCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing " + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }

                    
                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region SHEILD GENERATOR CONSTRUCTION

                finishStep = false;
                while (finishStep == false)
                {
                    
                    int cCost = 10;
                    int rCost = 20;
                    int vCost = 10;
                    string constructName = "sheild generator";
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.sheilds++;
                    }

                    instructions.text = "Constructing " + constructName + "";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed"+constructName+"Building");
                        instructions.text = "Completed " + constructName + " construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= vCost && playSpace.veryRare >= rCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing" + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region ARMOR CONSTRUCTION

                finishStep = false;
                while (finishStep == false)
                {

                    int cCost = 10;
                    int rCost = 30;
                    int vCost = 10;
                    string constructName = "armor";
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.armor++;
                    }

                    instructions.text = "Constructing " + constructName + "";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + constructName + "Building");
                        instructions.text = "Completed " + constructName + " construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= vCost && playSpace.veryRare >= rCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing" + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region BEAM WEAPON CONSTRUCTION

                finishStep = false;
                while (finishStep == false)
                {

                    int cCost = 30;
                    int rCost = 30;
                    int vCost = 10;
                    string constructName = "beam weapon";
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.beamWeapon++;
                    }

                    instructions.text = "Constructing " + constructName + "";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + constructName + "Building");
                        instructions.text = "Completed " + constructName + " construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= vCost && playSpace.veryRare >= rCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing" + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region MISSILE SYSTEM CONSTRUCTION

                finishStep = false;
                while (finishStep == false)
                {

                    int cCost = 20;
                    int rCost = 10;
                    int vCost = 0;
                    string constructName = "missile system";
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.missileWeapon++;
                    }

                    instructions.text = "Constructing " + constructName + "";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + constructName + "Building");
                        instructions.text = "Completed " + constructName + " construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= vCost && playSpace.veryRare >= rCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing" + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region ANTI MISSILE SYSTEM CONSTRUCTION

                finishStep = false;
                while (finishStep == false)
                {

                    int cCost = 30;
                    int rCost = 20;
                    int vCost = 10;
                    string constructName = "anti-missile system";
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.antiMissileSystems++;
                    }

                    instructions.text = "Constructing " + constructName + "";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + constructName + "Building");
                        instructions.text = "Completed " + constructName + " construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= vCost && playSpace.veryRare >= rCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing" + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region SPACE MARINE CONSTRUCTION

                finishStep = false;
                while (finishStep == false)
                {

                    int cCost = 10;
                    int rCost = 10;
                    int vCost = 0;
                    string constructName = "space marine";
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.marines++;
                    }

                    instructions.text = "Constructing " + constructName + "";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + constructName + "Building");
                        instructions.text = "Completed " + constructName + " construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer&& playSpace.marines<3 && playSpace.Counters.Contains(Counter.Barracks))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= vCost && playSpace.veryRare >= rCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing" + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region MINE CONSTRUCTION

                finishStep = false;
                while (finishStep == false)
                {

                    int cCost = 150;
                    int rCost = 20;
                    int vCost = 10;
                    string constructName = "mine";
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.Counters.Add(Counter.Mine);
                    }

                    instructions.text = "Constructing " + constructName + "";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + constructName + "Building");
                        instructions.text = "Completed " + constructName + " construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer&&!playSpace.Counters.Contains(Counter.Mine))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= vCost && playSpace.veryRare >= rCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing" + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region SPACE DOCK CONSTRUCTION

                finishStep = false;
                while (finishStep == false)
                {

                    int cCost = 150;
                    int rCost = 50;
                    int vCost = 20;
                    string constructName = "space dock";
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.Counters.Add(Counter.SpaceDock);
                    }

                    instructions.text = "Constructing " + constructName + "";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + constructName + "Building");
                        instructions.text = "Completed " + constructName + " construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer && !playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= vCost && playSpace.veryRare >= rCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing" + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region STATIC DEFENSE SYSTEM CONSTRUCTION

                finishStep = false;
                while (finishStep == false)
                {

                    int cCost = 30;
                    int rCost = 10;
                    int vCost = 0;
                    string constructName = "static defense system";
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.Counters.Add(Counter.StaticDefenceSystem);
                    }

                    instructions.text = "Constructing " + constructName + "";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + constructName + "Building");
                        instructions.text = "Completed " + constructName + " construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer && !playSpace.Counters.Contains(Counter.StaticDefenceSystem))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= vCost && playSpace.veryRare >= rCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing" + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region BARRACKS CONSTRUCTION

                finishStep = false;
                while (finishStep == false)
                {

                    int cCost = 30;
                    int rCost = 10;
                    int vCost = 0;
                    string constructName = "barracks";
                    void customButtonEffect(PlaySpace playSpace)
                    {
                        playSpace.Counters.Add(Counter.Barracks);
                    }

                    instructions.text = "Constructing " + constructName + "";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + constructName + "Building");
                        instructions.text = "Completed " + constructName + " construction";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer && !playSpace.Counters.Contains(Counter.Barracks))
                            {
                                if (playSpace.common >= cCost && playSpace.rare >= vCost && playSpace.veryRare >= rCost)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                        instructions.text = "Constructing" + constructName + " in sector (" + x + ", " + y + ")";
                                        customButtonEffect(playSpace);
                                        playSpace.common -= cCost;
                                        playSpace.rare -= vCost;
                                        playSpace.veryRare -= rCost;

                                        buttonClicked = true;
                                        destroyButtons.Invoke();
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion
                //add parts to ships



                #region SHIP ENGINE INSTALATION
                finishStep = false;
                while (finishStep == false)
                {


                    string shipMod = "engines";
                    bool doesPlaySpaceHaveMod(PlaySpace playSpace)
                    {
                        return 0 < playSpace.engines;
                    }

                    void customButtonEffect(PlaySpace playSpace, Ship ship)
                    {
                        playSpace.engines -= 1;
                        ship.engines++;
                    }

                    instructions.text = "Add " + shipMod + " to ships";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + shipMod + "installation");
                        instructions.text = "Finished installing " + shipMod + " to ships";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (doesPlaySpaceHaveMod(playSpace)&&playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                bool anyViableShips = false;
                                foreach(Ship ship in playSpace.Ships)
                                {
                                  
                                    if(ship.PlayerInControl== activePlayer&&doesShipHaveFreeSpace(ship))
                                    {
                                        anyViableShips = true;
                                    }
                                }
                                if (anyViableShips)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                    {
                                            instructions.text = "Installing" + shipMod + " in sector (" + x + ", " + y + ")";
                                            destroyButtons.Invoke();
                                            int xOffset = 0;
                                            foreach (Ship ship in playSpace.Ships)
                                            {
                                                if (ship.PlayerInControl == activePlayer)
                                                {
                                                    if (doesShipHaveFreeSpace(ship))
                                                    {
                                                        xOffset++;
                                                        string name = ship.Name;
                                                        Ship localVarShip = ship;
                                                        CreateNewButton(name, new Vector3(-5.1f + xOffset * 1.5f, -4.6f, 0), () =>
                                                      {
                                                            instructions.text = "Installing " + shipMod + "to " + ship.Name;
                                                            buttonClicked = true;
                                                            customButtonEffect(playSpace, localVarShip);
                                                            destroyButtons.Invoke();
                                                        });
                                                    }
                                                }
                                            }
                                    });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region SHIP SHEILD GENERATOR INSTALATION
                finishStep = false;
                while (finishStep == false)
                {


                    string shipMod = "sheild generator";
                    bool doesPlaySpaceHaveMod(PlaySpace playSpace)
                    {
                        return 0 < playSpace.sheilds;
                    }
                    bool doesShipHaveFreeSpace(Ship ship)
                    {
                        int inventorySlotsFull = (ship.common + 9) / 10 + (ship.rare + 9) / 10 + (ship.veryRare + 9) / 10
                            + ship.antiMissileSystems + ship.armor + ship.beamWeapon + ship.engines
                            + ship.marines + ship.missileWeapon + ship.sheilds;
                        return 6 > inventorySlotsFull;
                    }
                    void customButtonEffect(PlaySpace playSpace, Ship ship)
                    {
                        playSpace.sheilds -= 1;
                        ship.sheilds++;
                    }

                    instructions.text = "Add " + shipMod + " to ships";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + shipMod + "installation");
                        instructions.text = "Finished installing " + shipMod + " to ships";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (doesPlaySpaceHaveMod(playSpace) && playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                bool anyViableShips = false;
                                foreach (Ship ship in playSpace.Ships)
                                {

                                    if (ship.PlayerInControl == activePlayer && doesShipHaveFreeSpace(ship))
                                    {
                                        anyViableShips = true;
                                    }
                                }
                                if (anyViableShips)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                {
                                    instructions.text = "Installing" + shipMod + " in sector (" + x + ", " + y + ")";
                                    destroyButtons.Invoke();
                                    int xOffset = 0;
                                    foreach (Ship ship in playSpace.Ships)
                                    {
                                        if (ship.PlayerInControl == activePlayer)
                                        {
                                            if (doesShipHaveFreeSpace(ship))
                                            {
                                                xOffset++;
                                                string name = ship.Name;
                                                Ship localVarShip = ship;
                                                CreateNewButton(name, new Vector3(-5.1f + xOffset * 1.5f, -4.6f, 0), () =>
                                                {
                                                    instructions.text = "Installing " + shipMod + "to " + ship.Name;
                                                    buttonClicked = true;
                                                    customButtonEffect(playSpace, localVarShip);
                                                    destroyButtons.Invoke();
                                                });
                                            }
                                        }
                                    }
                                });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region SHIP ARMOR INSTALATION
                finishStep = false;
                while (finishStep == false)
                {


                    string shipMod = "armor";
                    bool doesPlaySpaceHaveMod(PlaySpace playSpace)
                    {
                        return 0 < playSpace.armor;
                    }
                    bool doesShipHaveFreeSpace(Ship ship)
                    {
                        int inventorySlotsFull = (ship.common + 9) / 10 + (ship.rare + 9) / 10 + (ship.veryRare + 9) / 10
                            + ship.antiMissileSystems + ship.armor + ship.beamWeapon + ship.engines
                            + ship.marines + ship.missileWeapon + ship.sheilds;
                        return 6 > inventorySlotsFull;
                    }
                    void customButtonEffect(PlaySpace playSpace, Ship ship)
                    {
                        playSpace.armor -= 1;
                        ship.armor++;
                    }

                    instructions.text = "Add " + shipMod + " to ships";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + shipMod + "installation");
                        instructions.text = "Finished installing " + shipMod + " to ships";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (doesPlaySpaceHaveMod(playSpace) && playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                bool anyViableShips = false;
                                foreach (Ship ship in playSpace.Ships)
                                {

                                    if (ship.PlayerInControl == activePlayer && doesShipHaveFreeSpace(ship))
                                    {
                                        anyViableShips = true;
                                    }
                                }
                                if (anyViableShips)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                {
                                    instructions.text = "Installing" + shipMod + " in sector (" + x + ", " + y + ")";
                                    destroyButtons.Invoke();
                                    int xOffset = 0;
                                    foreach (Ship ship in playSpace.Ships)
                                    {
                                        if (ship.PlayerInControl == activePlayer)
                                        {
                                            if (doesShipHaveFreeSpace(ship))
                                            {
                                                xOffset++;
                                                string name = ship.Name;
                                                Ship localVarShip = ship;
                                                CreateNewButton(name, new Vector3(-5.1f + xOffset * 1.5f, -4.6f, 0), () =>
                                                {
                                                    instructions.text = "Installing " + shipMod + "to " + ship.Name;
                                                    buttonClicked = true;
                                                    customButtonEffect(playSpace, localVarShip);
                                                    destroyButtons.Invoke();
                                                });
                                            }
                                        }
                                    }
                                });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region SHIP BEAM WEAPON INSTALATION
                finishStep = false;
                while (finishStep == false)
                {


                    string shipMod = "beam weapon";
                    bool doesPlaySpaceHaveMod(PlaySpace playSpace)
                    {
                        return 0 < playSpace.beamWeapon;
                    }
                    bool doesShipHaveFreeSpace(Ship ship)
                    {
                        int inventorySlotsFull = (ship.common + 9) / 10 + (ship.rare + 9) / 10 + (ship.veryRare + 9) / 10
                            + ship.antiMissileSystems + ship.armor + ship.beamWeapon + ship.engines
                            + ship.marines + ship.missileWeapon + ship.sheilds;
                        return 6 > inventorySlotsFull;
                    }
                    void customButtonEffect(PlaySpace playSpace, Ship ship)
                    {
                        playSpace.beamWeapon -= 1;
                        ship.beamWeapon++;
                    }

                    instructions.text = "Add " + shipMod + " to ships";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + shipMod + "installation");
                        instructions.text = "Finished installing " + shipMod + " to ships";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (doesPlaySpaceHaveMod(playSpace) && playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                bool anyViableShips = false;
                                foreach (Ship ship in playSpace.Ships)
                                {

                                    if (ship.PlayerInControl == activePlayer && doesShipHaveFreeSpace(ship))
                                    {
                                        anyViableShips = true;
                                    }
                                }
                                if (anyViableShips)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                {
                                    instructions.text = "Installing" + shipMod + " in sector (" + x + ", " + y + ")";
                                    destroyButtons.Invoke();
                                    int xOffset = 0;
                                    foreach (Ship ship in playSpace.Ships)
                                    {
                                        if (ship.PlayerInControl == activePlayer)
                                        {
                                            if (doesShipHaveFreeSpace(ship))
                                            {
                                                xOffset++;
                                                string name = ship.Name;
                                                Ship localVarShip = ship;
                                                CreateNewButton(name, new Vector3(-5.1f + xOffset * 1.5f, -4.6f, 0), () =>
                                                {
                                                    instructions.text = "Installing " + shipMod + "to " + ship.Name;
                                                    buttonClicked = true;
                                                    customButtonEffect(playSpace, localVarShip);
                                                    destroyButtons.Invoke();
                                                });
                                            }
                                        }
                                    }
                                });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region SHIP MISSILE SYSTEM INSTALATION
                finishStep = false;
                while (finishStep == false)
                {


                    string shipMod = "missile System";
                    bool doesPlaySpaceHaveMod(PlaySpace playSpace)
                    {
                        return 0 < playSpace.missileWeapon;
                    }
                    bool doesShipHaveFreeSpace(Ship ship)
                    {
                        int inventorySlotsFull = (ship.common + 9) / 10 + (ship.rare + 9) / 10 + (ship.veryRare + 9) / 10
                            + ship.antiMissileSystems + ship.armor + ship.beamWeapon + ship.engines
                            + ship.marines + ship.missileWeapon + ship.sheilds;
                        return 6 > inventorySlotsFull;
                    }
                    void customButtonEffect(PlaySpace playSpace, Ship ship)
                    {
                        playSpace.missileWeapon -= 1;
                        ship.missileWeapon++;
                    }

                    instructions.text = "Add " + shipMod + " to ships";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + shipMod + "installation");
                        instructions.text = "Finished installing " + shipMod + " to ships";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (doesPlaySpaceHaveMod(playSpace) && playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                bool anyViableShips = false;
                                foreach (Ship ship in playSpace.Ships)
                                {

                                    if (ship.PlayerInControl == activePlayer && doesShipHaveFreeSpace(ship))
                                    {
                                        anyViableShips = true;
                                    }
                                }
                                if (anyViableShips)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                {
                                    instructions.text = "Installing" + shipMod + " in sector (" + x + ", " + y + ")";
                                    destroyButtons.Invoke();
                                    int xOffset = 0;
                                    foreach (Ship ship in playSpace.Ships)
                                    {
                                        if (ship.PlayerInControl == activePlayer)
                                        {
                                            if (doesShipHaveFreeSpace(ship))
                                            {
                                                xOffset++;
                                                string name = ship.Name;
                                                Ship localVarShip = ship;
                                                CreateNewButton(name, new Vector3(-5.1f + xOffset * 1.5f, -4.6f, 0), () =>
                                                {
                                                    instructions.text = "Installing " + shipMod + "to " + ship.Name;
                                                    buttonClicked = true;
                                                    customButtonEffect(playSpace, localVarShip);
                                                    destroyButtons.Invoke();
                                                });
                                            }
                                        }
                                    }
                                });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                #region SHIP ANTI-MISSILE SYSTEM INSTALATION
                finishStep = false;
                while (finishStep == false)
                {


                    string shipMod = "anti-missile systems";
                    bool doesPlaySpaceHaveMod(PlaySpace playSpace)
                    {
                        return 0 < playSpace.antiMissileSystems;
                    }
                    bool doesShipHaveFreeSpace(Ship ship)
                    {
                        int inventorySlotsFull = (ship.common + 9) / 10 + (ship.rare + 9) / 10 + (ship.veryRare + 9) / 10
                            + ship.antiMissileSystems + ship.armor + ship.beamWeapon + ship.engines
                            + ship.marines + ship.missileWeapon + ship.sheilds;
                        return 6 > inventorySlotsFull;
                    }
                    void customButtonEffect(PlaySpace playSpace, Ship ship)
                    {
                        playSpace.antiMissileSystems -= 1;
                        ship.antiMissileSystems++;
                    }

                    instructions.text = "Add " + shipMod + " to ships";
                    bool buttonClicked = false;
                    CreateNewButton("Finish", new Vector3(7, 4, 0), () =>
                    {
                        Debug.Log("Completed" + shipMod + "installation");
                        instructions.text = "Finished installing " + shipMod + " to ships";
                        finishStep = true;
                        buttonClicked = true;
                        destroyButtons.Invoke();
                    });

                    //generate buttons
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (doesPlaySpaceHaveMod(playSpace) && playSpace.PlayerInControl == activePlayer && playSpace.Counters.Contains(Counter.SpaceDock))
                            {
                                bool anyViableShips = false;
                                foreach (Ship ship in playSpace.Ships)
                                {

                                    if (ship.PlayerInControl == activePlayer && doesShipHaveFreeSpace(ship))
                                    {
                                        anyViableShips = true;
                                    }
                                }
                                if (anyViableShips)
                                {
                                    CreateNewButton("Build", new Vector3(x - 3.5f, y - 3.5f, 0), () =>
                                {
                                    instructions.text = "Installing" + shipMod + " in sector (" + x + ", " + y + ")";
                                    destroyButtons.Invoke();
                                    int xOffset = 0;
                                    foreach (Ship ship in playSpace.Ships)
                                    {
                                        if (ship.PlayerInControl == activePlayer)
                                        {
                                            if (doesShipHaveFreeSpace(ship))
                                            {
                                                xOffset++;
                                                string name = ship.Name;
                                                Ship localVarShip = ship;
                                                CreateNewButton(name, new Vector3(-5.1f + xOffset * 1.5f, -4.6f, 0), () =>
                                                {
                                                    instructions.text = "Installing " + shipMod + "to " + ship.Name;
                                                    buttonClicked = true;
                                                    customButtonEffect(playSpace, localVarShip);
                                                    destroyButtons.Invoke();
                                                });
                                            }
                                        }
                                    }
                                });
                                }
                            }
                        }
                    }


                    yield return new WaitUntil(() => buttonClicked == true);
                    yield return new WaitForSeconds(.1f);
                    instructions.text = "";
                    yield return new WaitForSeconds(0.14f);

                }
                #endregion

                //Trading goes here if i feel like it




                
                universe.DestroyUniverse();
                universe.DisplayUniverse(tile);
                destroyButtons.Invoke();
                instructions.text = activePlayer.name + "has completed their phase 1";
                yield return new WaitForSeconds(0.5f);
            }

            //phase 2


            foreach (Player activePlayer in players)
            {
                SetTurnMarker(activePlayer);
                //spend resources, resources must be available at that location

                #region SHIP MOVEMENT
                
                    instructions.text = "Move all ships";
                for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                    {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];

                        List<Ship> ships = new(playSpace.Ships);
                        foreach (Ship ship in ships)
                        {
                            
                            if (ship.PlayerInControl == activePlayer&&ship.alreadMoved== false)
                            {
                                int shipX = bx;
                                int shipY = by;
                                ship.alreadMoved = true;// due to looping through positions instead of spaces ships can show up multiple times depending on were they move.
                                instructions.text = "Moving ship "+ship.Name+" in sector (" + x + ", " + y + ")";
                                
                                
                                for (int i = 0; i < ship.engines; i++)
                                {
                                    bool skipMovement = false;
                                    bool movedSpace = false;
                                        Ship localVarShip = ship;
                                        CreateNewButton("Skip", new Vector3(7, 4, 0), () =>
                                        {
                                            Debug.Log("Skipping movement of ship " +localVarShip.Name);
                                            instructions.text = "Skipping movement of ship " +localVarShip.Name;
                                            skipMovement = true;
                                            movedSpace = true;
                                            destroyButtons.Invoke();
                                        });

                            
                                        int wrapXoffset(int v)
                                        {
                                            return (shipX + 8 + v) % 8;
                                        }
                                        int wrapYoffset(int v)
                                        {
                                            return (shipY + 8 + v) % 8;
                                        }


                                    CreateNewButton("N", new Vector3(wrapXoffset(0) - 3.5f, wrapYoffset(1) - 3.5f, 0), () =>
                                    {
                                        universe.universeGrid[wrapXoffset(0), wrapYoffset(0)].Ships.Remove(ship);
                                        universe.universeGrid[wrapXoffset(0), wrapYoffset(1)].Ships.Add(ship);
                                        shipX = wrapXoffset(0);
                                        shipY = wrapYoffset(1);
                                        destroyButtons.Invoke();
                                        movedSpace = true;
                                    });
                                    CreateNewButton("S", new Vector3(wrapXoffset(0) - 3.5f, wrapYoffset(-1) - 3.5f, 0), () =>
                                    {
                                        universe.universeGrid[wrapXoffset(0), wrapYoffset(0)].Ships.Remove(ship);
                                        universe.universeGrid[wrapXoffset(0), wrapYoffset(-1)].Ships.Add(ship);
                                        shipX = wrapXoffset(0);
                                        shipY = wrapYoffset(-1);
                                        destroyButtons.Invoke();
                                        movedSpace = true;
                                    });
                                    CreateNewButton("E", new Vector3(wrapXoffset(1) - 3.5f, wrapYoffset(0) - 3.5f, 0), () =>
                                    {
                                        universe.universeGrid[wrapXoffset(0), wrapYoffset(0)].Ships.Remove(ship);
                                        universe.universeGrid[wrapXoffset(1), wrapYoffset(0)].Ships.Add(ship);
                                        shipX = wrapXoffset(1);
                                        shipY = wrapYoffset(0);
                                        destroyButtons.Invoke();
                                        movedSpace = true;
                                    });
                                    CreateNewButton("W", new Vector3(wrapXoffset(-1) - 3.5f, wrapYoffset(0) - 3.5f, 0), () =>
                                    {
                                        universe.universeGrid[wrapXoffset(0), wrapYoffset(0)].Ships.Remove(ship);
                                        universe.universeGrid[wrapXoffset(-1), wrapYoffset(0)].Ships.Add(ship);
                                        shipX = wrapXoffset(-1);
                                        shipY = wrapYoffset(0);
                                        destroyButtons.Invoke();
                                        movedSpace = true;
                                    });
                                    CreateNewButton("NE", new Vector3(wrapXoffset(1) - 3.5f, wrapYoffset(1) - 3.5f, 0), () =>
                                    {
                                        universe.universeGrid[wrapXoffset(0), wrapYoffset(0)].Ships.Remove(ship);
                                        universe.universeGrid[wrapXoffset(1), wrapYoffset(1)].Ships.Add(ship);
                                        shipX = wrapXoffset(1);
                                        shipY = wrapYoffset(1);
                                        destroyButtons.Invoke();
                                        movedSpace = true;
                                    });
                                    CreateNewButton("NW", new Vector3(wrapXoffset(-1) - 3.5f, wrapYoffset(1) - 3.5f, 0), () =>
                                    {
                                        universe.universeGrid[wrapXoffset(0), wrapYoffset(0)].Ships.Remove(ship);
                                        universe.universeGrid[wrapXoffset(-1), wrapYoffset(1)].Ships.Add(ship);
                                        shipX = wrapXoffset(-1);
                                        shipY = wrapYoffset(1);
                                        destroyButtons.Invoke();
                                        movedSpace = true;
                                    });
                                    CreateNewButton("SE", new Vector3(wrapXoffset(1) - 3.5f, wrapYoffset(-1) - 3.5f, 0), () =>
                                    {
                                        universe.universeGrid[wrapXoffset(0), wrapYoffset(0)].Ships.Remove(ship);
                                        universe.universeGrid[wrapXoffset(1), wrapYoffset(-1)].Ships.Add(ship);
                                        shipX = wrapXoffset(1);
                                        shipY = wrapYoffset(-1);
                                        destroyButtons.Invoke();
                                        movedSpace = true;
                                    });
                                    CreateNewButton("SW", new Vector3(wrapXoffset(-1) - 3.5f, wrapYoffset(-1) - 3.5f, 0), () =>
                                    {
                                        universe.universeGrid[wrapXoffset(0), wrapYoffset(0)].Ships.Remove(ship);
                                        universe.universeGrid[wrapXoffset(-1), wrapYoffset(-1)].Ships.Add(ship);
                                        shipX = wrapXoffset(-1);
                                        shipY = wrapYoffset(-1);
                                        destroyButtons.Invoke();
                                        movedSpace = true;
                                    });


                                    yield return new WaitUntil(()=>(movedSpace==true));
                                    if (skipMovement == true)
                                    {
                                        break;
                                    }
                                    instructions.text = "";
                                    destroyButtons.Invoke();
                                    universe.DestroyUniverse();
                                    universe.DisplayUniverse(tile);
                                    yield return new WaitForSeconds(0.01f);
                                    }
                                }
                        }
                            
                    }
                }

                // reset already moved flag
                for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                    {
                        PlaySpace playSpace = universe.universeGrid[x, y];
                        foreach (Ship ship in playSpace.Ships)
                        {
                            ship.alreadMoved = false;
                        }
                    }
                }
                instructions.text = activePlayer.name + "All ships have been moved";
                yield return new WaitForSeconds(0.25f);

                #endregion




                int systemCount=0;

                #region COMBAT

                //Ship Combat only be initiated by player with 1 home system
                for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                    {
                        int bx = x;
                        int by = y;
                        PlaySpace playSpace = universe.universeGrid[bx, by];
                        if (playSpace.PlayerInControl == activePlayer)
                        {
                            systemCount++;
                        }


                    }
                }
                if (systemCount <= 1)
                {


                    bool skip = false;

                    while (skip == false)
                    {
                        instructions.text = "Choose target to attack";
                        bool buttonInput = false;

                        CreateNewButton("Skip", new Vector3(7, 4, 0), () =>
                        {
                            Debug.Log("Skipping attack phase");
                            instructions.text = "Skipping attack phase";
                            skip = true;
                            buttonInput = true;
                            destroyButtons.Invoke();
                        });

                        for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                        {
                            for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                            {
                                int bx = x;
                                int by = y;
                                PlaySpace playSpace = universe.universeGrid[bx, by];
                                List<Ship> selfPresent = new();
                                List<Ship> enemyPresent = new();
                                foreach (Ship ship in playSpace.Ships)
                                {
                                    if (ship.PlayerInControl == activePlayer)
                                    {
                                        selfPresent.Add(ship);
                                    }
                                    else
                                    {
                                        enemyPresent.Add(ship);
                                    }
                                }
                                if (selfPresent.Count>0 && enemyPresent.Count>0)
                                {
                                    //spawn combat button
                                    CreateNewButton("Attack", new Vector3( bx - 3.5f, by - 3.5f, 0), () =>
                                    {
                                        destroyButtons.Invoke();
                                        #region attack
                                        int beamHits = 0;
                                        int missileHits = 0;
                                        foreach (Ship s in selfPresent)
                                        {
                                            instructions.text = "Aggressor choose target to attack!";
                                            Debug.Log(s.Name + "IS CURRENTLY ATTACKING");
                                            
                                            //firebeamweapons
                                            for(int i = 0; i < s.beamWeapon; i++)
                                            {
                                                if (Random.value < 0.5f)
                                                {
                                                   beamHits++;            
                                                }
                                            }

                                            //fire missile weapons
                                            for (int i = 0; i < s.missileWeapon; i++)
                                            {
                                                if (Random.value < 0.5f)
                                                {
                                                    missileHits++;
                                                }
                                            }
                                            //space marine assault
                                        }
                                        float xOffset=0;
                                        foreach (Ship lenemyShip in enemyPresent)
                                        {
                                            xOffset++;
                                            string name = lenemyShip.Name;
                                            Ship enemyShip = lenemyShip;
                                            CreateNewButton(name, new Vector3(-5.1f + xOffset * 1.5f, -4.6f, 0), () =>
                                            {
                                                instructions.text = "Attacking " + enemyShip.Name + "!";
                                                //buttonInput = true;
                                                destroyButtons.Invoke();

                                                //block attacks;
                                                int beamBlocks=0;
                                                int missileBlocks = 0;
                                                if (enemyShip.sheilds > 0)
                                                {
                                                    for (int i = 0; i < beamHits; i++)
                                                    {
                                                        if (Random.value < 0.5f)
                                                        {
                                                            beamBlocks++;
                                                        }
                                                    }
                                                }
                                                for(int i = 0; i<enemyShip.armor; i++)
                                                {
                                                    if (Random.value < 0.5f)
                                                    {
                                                        missileBlocks++;
                                                    }
                                                }
                                                int totalHits = beamHits - beamBlocks + (int)Mathf.Max(missileHits - missileBlocks, 0);
                                                enemyShip.armor -= totalHits;
                                                totalHits = -enemyShip.armor;
                                                enemyShip.armor = Mathf.Max(enemyShip.armor, 0);
                                                totalHits = Mathf.Max(totalHits, 0);
                                                //Destroy components and stuff;
                                                for (int i = 0; i < totalHits; i++)
                                                {
                                                    int inventorySlotsFull = (enemyShip.common + 9) / 10 + (enemyShip.rare + 9) / 10 + (enemyShip.veryRare + 9) / 10
                                                    + enemyShip.antiMissileSystems + enemyShip.armor + enemyShip.beamWeapon + enemyShip.engines
                                                    + enemyShip.marines + enemyShip.missileWeapon + enemyShip.sheilds;
                                                    bool criticalHit=false;
                                                    for(int j = 0; j<6-inventorySlotsFull; j++)
                                                    {
                                                        if (Random.Range(0, 6) == 0) criticalHit = true;

                                                    }
                                                    bool targetHit = false;
                                                    if (criticalHit == true)
                                                    {
                                                        enemyShip.criticalHits++;
                                                        targetHit = true;
                                                    }
                                                    
                                                    while (targetHit != true)
                                                    {

                                                        switch (Random.Range(0, 10))
                                                        {
                                                            case 0:
                                                                if (enemyShip.antiMissileSystems > 0)
                                                                {
                                                                    enemyShip.antiMissileSystems--;
                                                                    targetHit = true;
                                                                }
                                                                break;
                                                            case 1:
                                                                if (enemyShip.armor > 0)
                                                                {
                                                                    enemyShip.armor--;
                                                                    targetHit = true;
                                                                }
                                                                break;
                                                            case 2:
                                                                if (enemyShip.beamWeapon > 0)
                                                                {
                                                                    enemyShip.beamWeapon--;
                                                                    targetHit = true;
                                                                }
                                                                break;
                                                            case 3:
                                                                if (enemyShip.common > 0)
                                                                {
                                                                    enemyShip.antiMissileSystems -= 10;
                                                                    targetHit = true;
                                                                }
                                                                break;
                                                            case 4:
                                                                if (enemyShip.engines > 0)
                                                                {
                                                                    enemyShip.engines--;
                                                                    targetHit = true;
                                                                }
                                                                break;
                                                            case 5:
                                                                if (enemyShip.marines > 0)
                                                                {
                                                                    enemyShip.marines--;
                                                                    targetHit = true;
                                                                }
                                                                break;
                                                            case 6:
                                                                if (enemyShip.missileWeapon > 0)
                                                                {
                                                                    enemyShip.missileWeapon--;
                                                                    targetHit = true;
                                                                }
                                                                break;
                                                            case 7:
                                                                if (enemyShip.rare > 0)
                                                                {
                                                                    enemyShip.rare -= 10;
                                                                    targetHit = true;
                                                                }
                                                                break;
                                                            case 8:
                                                                if (enemyShip.sheilds > 0)
                                                                {
                                                                    enemyShip.sheilds--;
                                                                    targetHit = true;
                                                                }
                                                                break;
                                                            case 9:
                                                                if (enemyShip.veryRare > 0)
                                                                {
                                                                    enemyShip.veryRare -= 10;
                                                                    targetHit = true;
                                                                }
                                                                break;
                                                        }
                                                        inventorySlotsFull = (enemyShip.common + 9) / 10 + (enemyShip.rare + 9) / 10 + (enemyShip.veryRare + 9) / 10
                                                        + enemyShip.antiMissileSystems + enemyShip.armor + enemyShip.beamWeapon + enemyShip.engines
                                                        + enemyShip.marines + enemyShip.missileWeapon + enemyShip.sheilds;
                                                        if (inventorySlotsFull == 0)
                                                        {
                                                            enemyShip.criticalHits++;
                                                            targetHit = true;
                                                        }
                                                    }

                                                }

                                                if (enemyShip.criticalHits >= 3)
                                                {
                                                    playSpace.Ships.Remove(enemyShip);
                                                    instructions.text = enemyShip.Name+ " has been destroyed!";
                                                }


                                                //Marine Assault
                                                //localVarShi









                                                //Retaliation
                                                #region ENEMYRETALIATION
                                                beamHits = 0;
                                                missileHits = 0;
                                                foreach (Ship s in enemyPresent)
                                                {
                                                    instructions.text = "Defender choose target to attack";
                                                    Debug.Log(s.Name + "IS CURRENTLY ATTACKING");

                                                    //firebeamweapons
                                                    for (int i = 0; i < s.beamWeapon; i++)
                                                    {
                                                        if (Random.value < 0.5f)
                                                        {
                                                            beamHits++;
                                                        }
                                                    }

                                                    //fire missile weapons
                                                    for (int i = 0; i < s.missileWeapon; i++)
                                                    {
                                                        if (Random.value < 0.5f)
                                                        {
                                                            missileHits++;
                                                        }
                                                    }
                                                    //space marine assault
                                                }
                                                float xOffset = 0;
                                                foreach (Ship lenemyShip in selfPresent)
                                                {
                                                    xOffset++;
                                                    string name = lenemyShip.Name;
                                                    Ship enemyShip = lenemyShip;
                                                    CreateNewButton(name, new Vector3(-5.1f + xOffset * 1.5f, -4.6f, 0), () =>
                                                    {
                                                        instructions.text = "Attacking " + enemyShip.Name + "!";
                                                        buttonInput = true;
                                                        destroyButtons.Invoke();

                                                        //block attacks;
                                                        int beamBlocks = 0;
                                                        int missileBlocks = 0;
                                                        if (enemyShip.sheilds > 0)
                                                        {
                                                            for (int i = 0; i < beamHits; i++)
                                                            {
                                                                if (Random.value < 0.5f)
                                                                {
                                                                    beamBlocks++;
                                                                }
                                                            }
                                                        }
                                                        for (int i = 0; i < enemyShip.armor; i++)
                                                        {
                                                            if (Random.value < 0.5f)
                                                            {
                                                                missileBlocks++;
                                                            }
                                                        }
                                                        int totalHits = beamHits - beamBlocks + (int)Mathf.Max(missileHits - missileBlocks, 0);
                                                        enemyShip.armor -= totalHits;
                                                        totalHits = -enemyShip.armor;
                                                        enemyShip.armor = Mathf.Max(enemyShip.armor, 0);
                                                        totalHits = Mathf.Max(totalHits, 0);
                                                        //Destroy components and stuff;
                                                        for (int i = 0; i < totalHits; i++)
                                                        {
                                                            int inventorySlotsFull = (enemyShip.common + 9) / 10 + (enemyShip.rare + 9) / 10 + (enemyShip.veryRare + 9) / 10
                                                            + enemyShip.antiMissileSystems + enemyShip.armor + enemyShip.beamWeapon + enemyShip.engines
                                                            + enemyShip.marines + enemyShip.missileWeapon + enemyShip.sheilds;
                                                            bool criticalHit = false;
                                                            for (int j = 0; j < 6 - inventorySlotsFull; j++)
                                                            {
                                                                if (Random.Range(0, 6) == 0) criticalHit = true;

                                                            }
                                                            bool targetHit = false;
                                                            if (criticalHit == true)
                                                            {
                                                                enemyShip.criticalHits++;
                                                                targetHit = true;
                                                            }

                                                            while (targetHit != true)
                                                            {

                                                                switch (Random.Range(0, 10))
                                                                {
                                                                    case 0:
                                                                        if (enemyShip.antiMissileSystems > 0)
                                                                        {
                                                                            enemyShip.antiMissileSystems--;
                                                                            targetHit = true;
                                                                        }
                                                                        break;
                                                                    case 1:
                                                                        if (enemyShip.armor > 0)
                                                                        {
                                                                            enemyShip.armor--;
                                                                            targetHit = true;
                                                                        }
                                                                        break;
                                                                    case 2:
                                                                        if (enemyShip.beamWeapon > 0)
                                                                        {
                                                                            enemyShip.beamWeapon--;
                                                                            targetHit = true;
                                                                        }
                                                                        break;
                                                                    case 3:
                                                                        if (enemyShip.common > 0)
                                                                        {
                                                                            enemyShip.antiMissileSystems -= 10;
                                                                            targetHit = true;
                                                                        }
                                                                        break;
                                                                    case 4:
                                                                        if (enemyShip.engines > 0)
                                                                        {
                                                                            enemyShip.engines--;
                                                                            targetHit = true;
                                                                        }
                                                                        break;
                                                                    case 5:
                                                                        if (enemyShip.marines > 0)
                                                                        {
                                                                            enemyShip.marines--;
                                                                            targetHit = true;
                                                                        }
                                                                        break;
                                                                    case 6:
                                                                        if (enemyShip.missileWeapon > 0)
                                                                        {
                                                                            enemyShip.missileWeapon--;
                                                                            targetHit = true;
                                                                        }
                                                                        break;
                                                                    case 7:
                                                                        if (enemyShip.rare > 0)
                                                                        {
                                                                            enemyShip.rare -= 10;
                                                                            targetHit = true;
                                                                        }
                                                                        break;
                                                                    case 8:
                                                                        if (enemyShip.sheilds > 0)
                                                                        {
                                                                            enemyShip.sheilds--;
                                                                            targetHit = true;
                                                                        }
                                                                        break;
                                                                    case 9:
                                                                        if (enemyShip.veryRare > 0)
                                                                        {
                                                                            enemyShip.veryRare -= 10;
                                                                            targetHit = true;
                                                                        }
                                                                        break;
                                                                }
                                                                inventorySlotsFull = (enemyShip.common + 9) / 10 + (enemyShip.rare + 9) / 10 + (enemyShip.veryRare + 9) / 10
                                                                + enemyShip.antiMissileSystems + enemyShip.armor + enemyShip.beamWeapon + enemyShip.engines
                                                                + enemyShip.marines + enemyShip.missileWeapon + enemyShip.sheilds;
                                                                if (inventorySlotsFull == 0)
                                                                {
                                                                    enemyShip.criticalHits++;
                                                                    targetHit = true;
                                                                }
                                                            }

                                                        }

                                                        if (enemyShip.criticalHits >= 3)
                                                        {
                                                            playSpace.Ships.Remove(enemyShip);
                                                            instructions.text = enemyShip.Name + " has been destroyed!";
                                                        }
                                                    });
                                                }
                                                #endregion



                                            });
                                        }
                                        #endregion

                                    });
                                }

                            }
                        }
                        yield return new WaitUntil(() => buttonInput ==  true);
                        yield return new WaitForSeconds(2f);
                    }
                }

                #endregion



                #region RAIDING
                //spawn skip button
                //Raiding only if player has 1 home system
                if (systemCount <= 1)
                {
                    bool skip = false;
                    while (skip == false)
                    {
                        instructions.text = "Choose target to raid.";

                        bool buttonInput = false;

                        CreateNewButton("Skip", new Vector3(7, 4, 0), () =>
                        {
                            Debug.Log("Skipping raid phase");
                            instructions.text = "Skipping raiding phase";
                            skip = true;
                            buttonInput = true;
                            destroyButtons.Invoke();
                        });


                        for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                        {
                            for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                            {
                                int bx = x;
                                int by = y;
                                PlaySpace playSpace = universe.universeGrid[bx, by];
                                List<Ship> selfPresent = new();
                                List<Ship> enemyPresent = new();
                                int marinesPresent=0;
                                foreach (Ship ship in playSpace.Ships)
                                {
                                    if (ship.PlayerInControl == activePlayer)
                                    {
                                        selfPresent.Add(ship);
                                        marinesPresent+=ship.marines;
                                    }
                                    else
                                    {
                                        enemyPresent.Add(ship);
                                    }

                                }


                                if (marinesPresent>0 && enemyPresent.Count==0&& playSpace.PlayerInControl != activePlayer&& playSpace.systemType!= SystemType.Empty)
                                {
                                    
                                    CreateNewButton("Raid", new Vector3(bx - 3.5f, by - 3.5f, 0), () =>
                                    {
                                        int attacks = 0;
                                        int enemyAttacks = 0;
                                        for(int i = 0; i< marinesPresent; i++ )
                                        {
                                            if (Random.value > 0.5f)
                                            {
                                                attacks++; 
                                            }
                                        }
                                        for (int i = 0; i< playSpace.marines; i++)
                                        {
                                            if (Random.value > 0.5f)
                                            {
                                                enemyAttacks++; 
                                            }
                                        }

                                        playSpace.marines = Mathf.Max(0, playSpace.marines - attacks);
                                        marinesPresent -= enemyAttacks;
                                        for(int i =0; i <enemyAttacks; i++)
                                        {
                                            bool killed = false;
                                            int j = 0;
                                            while(killed ==false)
                                            {
                                                if (j >= selfPresent.Count)
                                                {
                                                    break;
                                                }
                                                if (selfPresent[j].marines > 0)
                                                {
                                                    selfPresent[j].marines--;
                                                    killed = true;
                                                    
                                                }
                                                else
                                                {
                                                    j++;
                                                }
                                            }
                                        }
                                        if (marinesPresent > 0 && playSpace.marines==0)
                                        {
                                            instructions.text = "Space marine ground combat succesful! Starting raid.";
                                            //spawn buttons for destructiontheft and espionage here
                                            //---------------

                                            //TEMP--
                                            if (Random.Range(1, 7) < marinesPresent)
                                            {
                                                instructions.text = "Destruction succesful.";
                                                playSpace.PlayerInControl = null;
                                                playSpace.Counters.Remove(Counter.HomeSystem);
                                                
                                            }
                                            //------
                                            destroyButtons.Invoke();
                                            buttonInput = true;
                                        }
                                        else
                                        {
                                            instructions.text = "Ground combat has resulted in failure.";

                                            destroyButtons.Invoke();
                                            buttonInput = true;
                                        }
                                    });
                                }

                            }
                        }



                        yield return new WaitUntil(() => buttonInput == true);
                        yield return new WaitForSeconds(0.4f);
                    }
                }





                #endregion
                //load unload cargo/space marines
                bool end= false;
                while (end == false)
                {
                    instructions.text = "load/unload cargo/spacemarines?";
                    bool reloadButtons = false;
                    CreateNewButton("End", new Vector3(7, 4, 0), () =>
                    {
                        instructions.text = "End load/unload cargo/spacemarines";
                        end = true;
                        reloadButtons = true;
                        destroyButtons.Invoke();
                    });
                    
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer)
                            {

                                CreateNewButton("load/unload", new Vector3(bx - 3.5f, by - 3.5f, 0), () =>
                                {
                                    destroyButtons.Invoke();

                                    CreateNewButton("Exit", new Vector3(7, 4, 0), () =>
                                    {
                                        instructions.text = "End load/unload cargo/spacemarines";
                                        
                                        reloadButtons = true;
                                        destroyButtons.Invoke();
                                    });



                                    instructions.text = "Choose ship";
                                    int xOffset = 0;
                                    foreach (Ship ship in playSpace.Ships)
                                    {
                                        Ship s = ship;
                                        if (s.PlayerInControl == activePlayer)
                                        {
                                            xOffset++;
                                            CreateNewButton(s.Name, new Vector3(-5.1f + xOffset * 1.5f, -4.6f, 0), () =>
                                            {

                                                destroyButtons.Invoke();
                                                CreateNewButton("Exit", new Vector3(7, 4, 0), () =>
                                                {
                                                    instructions.text = "End load/unload cargo/spacemarines";
                                                    
                                                    reloadButtons = true;
                                                    destroyButtons.Invoke();
                                                });

                                                instructions.text = "load/unload what";
                                                //load/unload common
                                                CreateNewButton("load C", new Vector3(-5.5f, -4.6f, 0), () =>
                                                {
                                                    if (playSpace.common > 0&&doesShipHaveFreeSpace(s))
                                                    {
                                                        playSpace.common -= 10;
                                                        s.common += 10;
                                                    }

                                                });
                                                CreateNewButton("unload C", new Vector3(-4.5f, -4.6f, 0), () =>
                                                {
                                                    if (s.common > 0)
                                                    {
                                                        s.common -= 10;
                                                        playSpace.common += 10;
                                                    }

                                                });
                                                //load/unload rare
                                                CreateNewButton("load R", new Vector3(-3f, -4.6f, 0), () =>
                                                {
                                                    if (playSpace.rare > 0&& doesShipHaveFreeSpace(s))
                                                    {
                                                        playSpace.rare -= 10;
                                                        s.rare += 10;
                                                    }
                                                });
                                                CreateNewButton("unload R", new Vector3(-2f, -4.6f, 0), () =>
                                                {
                                                    if (s.rare > 0)
                                                    {
                                                        s.rare -= 10;
                                                        playSpace.rare += 10;
                                                    }
                                                });
                                                //load/unload veryRare
                                                CreateNewButton("load V", new Vector3(2f, -4.6f, 0), () =>
                                                {
                                                    if (playSpace.veryRare > 0&& doesShipHaveFreeSpace(s))
                                                    {
                                                        playSpace.veryRare -= 10;
                                                        s.veryRare += 10;
                                                    }
                                                });
                                                CreateNewButton("unload V", new Vector3(3f, -4.6f, 0), () =>
                                                {
                                                    if (s.veryRare > 0)
                                                    {
                                                        s.veryRare -= 10;
                                                        playSpace.veryRare += 10;
                                                    }
                                                });
                                                //load/unload marines
                                                CreateNewButton("load M", new Vector3(4.5f, -4.6f, 0), () =>
                                                {
                                                    if (playSpace.marines > 0&& doesShipHaveFreeSpace(s))
                                                    {
                                                        playSpace.marines --;
                                                        s.marines ++;
                                                    }
                                                });
                                                CreateNewButton("unload M", new Vector3(5.5f, -4.6f, 0), () =>
                                                {
                                                    if (s.marines > 0&&playSpace.marines<3)
                                                    {
                                                        s.marines --;
                                                        playSpace.common ++;
                                                    }
                                                });
                                            });
                                        }
                                    }


                                });

                                
                            }
                        }
                    }
                    yield return new WaitUntil(()=>(reloadButtons== true));
                }
                //extablish control of new system



                end = false;
                while (end == false)
                {
                    instructions.text = "Take control of system";
                    bool reloadButtons = false;
                    CreateNewButton("End", new Vector3(7, 4, 0), () =>
                    {
                        instructions.text = "End load/unload cargo/spacemarines";
                        end = true;
                        reloadButtons = true;
                        destroyButtons.Invoke();
                    });

                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == null)
                            {
                                foreach (Ship s in playSpace.Ships)
                                {
                                    if (s.PlayerInControl == activePlayer)
                                    {
                                        if (s.marines >= 1)
                                        {

                                            //spawn button  to decrement marines and establish control
                                            CreateNewButton("Take Control", new Vector3(bx-3.5f, by-3.5f, 0), () =>
                                            {
                                                s.marines--;
                                                playSpace.PlayerInControl = activePlayer;
                                                reloadButtons = true;
                                                destroyButtons.Invoke();
                                                universe.DestroyUniverse();
                                                universe.DisplayUniverse(tile);
                                            });


                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    yield return new WaitUntil(()=>reloadButtons == true);
                }


                end = false;
                while (end == false)
                {
                    instructions.text = "Destroy metropolis?";
                    bool reloadButtons = false;
                    CreateNewButton("End", new Vector3(7, 4, 0), () =>
                    {
                        instructions.text = "End destroy metropolis";
                        end = true;
                        reloadButtons = true;
                        destroyButtons.Invoke();
                    });
                    //destroy all metropoli to turn into a raider. skip?
                    //just give the player the choice to destroy each one
                    for (int x = 0; x < universe.universeGrid.GetLength(0); x++)
                    {
                        for (int y = 0; y < universe.universeGrid.GetLength(1); y++)
                        {
                            int bx = x;
                            int by = y;
                            PlaySpace playSpace = universe.universeGrid[bx, by];
                            if (playSpace.PlayerInControl == activePlayer)
                            {
                                CreateNewButton("Self Destruct", new Vector3(bx - 3.5f, by - 3.5f, 0), () =>
                                {
                                    playSpace.PlayerInControl = null;
                                    reloadButtons = true;
                                    destroyButtons.Invoke();
                                    universe.DestroyUniverse();
                                    universe.DisplayUniverse(tile);
                                });
                            }
                        }
                    }
                    yield return new WaitUntil(() => (reloadButtons==true||end ==true));
                }

                instructions.text = ("Checking for win condition");

                bool enemyOwnsSystems = false;
                int systemsOwned=0;
                int totalSystems = 0;
                int achievment = 0;
                int wealth = 0;
                int power = 0;
                foreach(PlaySpace playSpace in universe.universeGrid)
                {
                    if (playSpace.PlayerInControl != null && playSpace.PlayerInControl != activePlayer)
                    {
                        enemyOwnsSystems = true;
                    }
                    if (playSpace.systemType != SystemType.Empty)
                    {
                        totalSystems++;
                        if (playSpace.PlayerInControl == activePlayer)
                        {
                            systemsOwned++;
                            wealth += (int)(playSpace.common * 0.1f);
                            wealth += (int)(playSpace.rare * 0.2f);
                            wealth += (int)(playSpace.veryRare * 0.3f);
                            achievment += playSpace.Counters.Count * 3;
                            power += 5;
                        }
                    }
                    foreach(Ship ship in playSpace.Ships)
                    {
                        if(ship.PlayerInControl == activePlayer)
                        {
                            power += 3;
                        }

                        wealth += (int)(ship.common * 0.1f);
                        wealth += (int)(ship.rare * 0.2f);
                        wealth += (int)(ship.veryRare * 0.3f);

                    }



                }
                //check for win condition
                if (enemyOwnsSystems == false && systemsOwned > 0)
                {
                    victoryCondition = true;
                }
                if (totalSystems < systemsOwned * 2)
                {
                    victoryCondition = true;
                }
                if (systemsOwned < 2 && wealth >= 60 && power >= 60 && achievment >= 12)
                {
                    victoryCondition = true;
                }
                if (systemsOwned > 1 && wealth >= 50 && power >= 50 && achievment >= 50)
                {
                    victoryCondition = true;
                }
                if (victoryCondition)
                {
                    instructions.text = activePlayer.name.ToUpper() + " IS THE WINNER!";
                    break;
                }
            }

        }



        destroyButtons.Invoke();
    }

    private void SetTurnMarker(Player p)
    {
        playerTurn.color = p.color;
        playerName.text = "The <br>" + p.name;


        Debug.Log("Turn " + players.IndexOf(p) + "Is " + p.color);
    }

    public GameObject CreateNewButton(string name, Vector3 location, UnityAction onClickAction)
    {
        GameObject newButton = Instantiate(buttonPrefab);
        newButton.transform.position = location;
        newButton.GetComponentInChildren<TMP_Text>().text = name;

        newButton.GetComponentInChildren<Button>().onClick.AddListener(onClickAction);
        destroyButtons.AddListener(() => { Destroy(newButton); });

        return newButton;
    }
    int nameCount = 0;
    public string GenerateNewName()
    {
        string name = nameCount.ToString();
        nameCount++;
        return name;
    }

}

public class Universe{

    public PlaySpace[,] universeGrid;
    public Universe()
    {
        GenerateUniverseChunks();
        
    }
    #region setup

    private void GenerateUniverseChunks()
    {
        //A universe Chunk is a 2x2 grid of space zones
        universeGrid = new PlaySpace[2*4, 2*4];
        for(int x = 0; x<2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                GenerateSpaceZone(x, y);
            }
        }
    }
    private void GenerateSpaceZone(int xChunk, int yChunk)
    {
        // a space zone is a 4x4 grid of playSpaces
        xChunk =(xChunk) * 4 ;
        yChunk =(yChunk) * 4 ;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int x = i + xChunk;
                int y = j + yChunk;

                universeGrid[x, y]  = new();


                universeGrid[x, y].systemType = SystemType.Empty;
            }
        }

        int numSystems = Random.Range(1, 4);
        for (int i = 0; i < numSystems; i++)//set the number of systems for the chunk
        {
            universeGrid[Random.Range(0, 4)+xChunk, Random.Range(0, 4)+yChunk].systemType = (SystemType)Random.Range(1, 5);
        }


    }
    #endregion



    GameObject emptyParent;
    public void DisplayUniverse(GameObject tile)
    {
        emptyParent = new GameObject();
        emptyParent.transform.position = Vector3.zero;

        for (int x = 0; x < universeGrid.GetLength(0); x++)
        {
            for (int y = 0; y < universeGrid.GetLength(1); y++)
            {
                GameObject playSpace = GameObject.Instantiate(tile, new Vector3(x+0.5f-(0.5f*universeGrid.GetLength(0)), y+0.5f - (0.5f * universeGrid.GetLength(1)), 0), Quaternion.identity, emptyParent.transform);
                
                if(universeGrid[x, y].systemType == SystemType.Home&& universeGrid[x, y].PlayerInControl == null)
                {
                    universeGrid[x, y].systemType = SystemType.Green;
                }
                switch (universeGrid[x, y].systemType)
                {
                    case SystemType.Empty:
                        //playSpace.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.grey;
                        break;
                    case SystemType.Green:
                        playSpace.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.green;
                        break;
                    case SystemType.Yellow:
                        playSpace.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.yellow;
                        break;
                    case SystemType.Blue:
                        playSpace.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.blue;
                        break;
                    case SystemType.Red:
                        playSpace.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.red;
                        break;
                    case SystemType.Home:
                        playSpace.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.white;
                        playSpace.GetComponentsInChildren<SpriteRenderer>()[1].color = universeGrid[x, y].PlayerInControl.color;
                        break;
                }
                
            }
        }
    }
    public void DestroyUniverse()
    {
        GameObject.Destroy(emptyParent);
    }
    struct UniverseChunks
    {
        SpaceZone[,] spaceZones;
    }
    struct SpaceZone
    {
        PlaySpace[,] playSpaces;
    }


}
public class PlaySpace
{
    public Player PlayerInControl = null;

    public SystemType systemType = SystemType.Empty;

    public HashSet<Counter> Counters = new();

    //resources
    public int common = 0;
    public int rare = 0;
    public int veryRare = 0;

    //components
    public int engines = 0;
    public int sheilds = 0;
    public int armor = 0;
    public int beamWeapon = 0;
    public int missileWeapon = 0;
    public int antiMissileSystems = 0;

    public int marines = 0; //max3


    //Ships
    public List<Ship> Ships = new();
    public PlaySpace() 
    {
    }
}
public class Ship
{
    public Ship()
    {
        PlayerInControl = new Player();
        PlayerInControl.name = "Abandoned";
        PlayerInControl.isHuman = false;
        PlayerInControl.color = Color.black;
    }
    public Player PlayerInControl = null;

    public bool alreadMoved = false;

    public string Name = "Default";

    //max3 then explode
    public int criticalHits = 0;

    //resources
    public int common = 0;
    public int rare = 0;
    public int veryRare = 0;

    //components Max 6
    public int engines = 0;
    public int sheilds = 0;
    public int armor = 0;
    public int beamWeapon = 0;
    public int missileWeapon = 0;
    public int antiMissileSystems = 0;
    public int marines = 0; //max3
}
public class Player
{
    public string name = "Default";
    public bool isHuman=false;
    public Color color;
}
public enum SystemType { Empty, Green, Yellow, Blue, Red, Home };
public enum Counter { HomeSystem, SpaceDock, Mine, Barracks, StaticDefenceSystem };