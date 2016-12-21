using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Collections;

public class Chat : MonoBehaviour
{
    public List<string> words;
    public List<int> sizes;

    private const int NUMBER_OF_WORDS = 30;
    private const int TEXTBOX_SIZE = 60;

    private float easyTime = 6;
    private float mediumTime = 7;
    private float hardTime = 5;

    public Vector2 size;
    private int rotation;
    private int fontSize;
    private Color color, backupColor, backgroundColor;
    private Color originalColor;
    private Color bonusColor;
    private int backupFontSize = -1;
    private Matrix4x4 backupRotation;

    private float time = 0;

    private float translateTime = 0;
    private int translateCheck;
    private int maxTranslateTime = 2;
    private float offsetX = 0;
    private float offsetY = 0;

    private float rotateTime = 0;
    private int rotateCheck;
    private int maxRotateTime = 2;
    private float offsetRotate = 0;

    private string line;
    private StreamReader file;

    public string currentMessage = string.Empty;

    private static System.Random rd;

    public string currentString = "";

    public int score = 0;
    public int lives = 5;

    public String name;
    private List<String> leaderboardNames;
    private List<int> leaderboardPoints;
    private Boolean updatedLeaderboard = false;
    private int ok;

    public int difficulty;
    public int check;
    public Camera camera;

    public bool runningState;
    public bool leaderBoardState;
    public bool leaderBoardRead;
    public bool enterNameState;

    private const int POINTSTOMEDIUM = 40;
    private const int POINTSTOHARD = 100;

    private const string WORDS_FILENAME = "words.txt";
    private const string LEADERBOARD_FILENAME = "leaderboard.txt";
    private const int MAX_WORD_SIZE = 25;

    private AudioSource[] source;
    private AudioSource correctSound;
    private AudioSource wrongSound;
    private AudioSource ambientalSound;
    private float volLowRange = .5f;
    private float volHighRange = 1.0f;

    private int isPause;
    private int error;

    private void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Q)
        {
            isPause = 1 - isPause;
        }

        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.W)
        {
            error = 1 - error;
        }

        // Game ON state
        if (runningState)        
        {
            GUI.SetNextControlName("MyTextField");
            currentMessage = GUI.TextField(new Rect(0, Screen.height - 40, Screen.width - 100, 20), currentMessage);
            GUI.FocusControl("MyTextField");

            // Display score & lives & time 
            GUI.skin.label.fontSize = 30;
            backupColor = GUI.color;
            GUI.color = bonusColor;
            GUI.Label(new Rect(20, fontSize, 180, 50), "Score: " + score);
            GUI.color = backupColor;
            GUI.Label(new Rect(20, 2 * fontSize, 180, 50), "Lives: " + lives);
            GUI.Label(new Rect(20, 3 * fontSize, 180, 50), "Time: " + String.Format("{0:0.00}", getTime()));

            GUI.skin.label.fontSize = 30;
            size = GUI.skin.label.CalcSize(new GUIContent(currentString));

            if (difficulty == 0)            // easy -> normal text, or colored text, or scaled text, or rotated text
            {
                if (check == 0)             // normal text
                {
                    GUI.Label(new Rect((Screen.width - size.x) / 2 + offsetX, (Screen.height - size.y) / 2 + offsetY, size.x, size.y), currentString);
                }
                else if (check == 1)        // color text
                {
                    setRandomColor();
                    GUI.Label(new Rect((Screen.width - size.x) / 2 + offsetX, (Screen.height - size.y) / 2 + offsetY, size.x, size.y), currentString);
                    resetColor();
                }
                else if (check == 2)        // scale text
                {
                    setFontSize();
                    size = GUI.skin.label.CalcSize(new GUIContent(currentString));
                    GUI.Label(new Rect((Screen.width - size.x) / 2 + offsetX, (Screen.height - size.y) / 2 + offsetY, size.x, size.y), currentString);
                    resetFontSize();
                }
                else if (check == 3)        // rotate text
                {
                    setRotation();
                    GUI.Label(new Rect((Screen.width - size.x) / 2 + offsetX, (Screen.height - size.y) / 2 + offsetY, size.x, size.y), currentString);
                    resetRotation();
                }
               else if (check == 4)        // different letters size
                {
                    int t = (int)((Screen.width - size.x) / 2 + offsetX);
                    int i = 0;
                    int q = GUI.skin.label.fontSize;

                    foreach (char c in currentString)
                    {
                        string cString = c.ToString();
                        GUI.skin.label.fontSize = sizes[i];
                        GUI.Label(new Rect(t, (Screen.height - size.y) / 2 + offsetY, sizes[i], size.y), cString);
                        t += MAX_WORD_SIZE;
                        i++;
                    }
                    GUI.skin.label.fontSize = q;
                }
            }
            else if (difficulty == 1)           // medium -> combined easy animations and random change during setup
            {
                if (check == 0)                  // normal and background color change
                {
                    camera.backgroundColor = backgroundColor;
                    GUI.Label(new Rect((Screen.width - size.x) / 2 + offsetX, (Screen.height - size.y) / 2 + offsetY, size.x, size.y), currentString);
                }
                else if (check == 1)             // Scale and color
                {
                    setFontSize();
                    setRandomColor();
                    GUI.Label(new Rect((Screen.width - size.x) / 2 + offsetX, (Screen.height - size.y) / 2 + offsetY, size.x, size.y), currentString);
                    resetColor();
                    resetFontSize();
                }
                else if (check == 2)            // Different letters size and color
                {
                    setRandomColor();
                    int t = (int)((Screen.width - size.x) / 2 + offsetX);
                    int i = 0;
                    int q = GUI.skin.label.fontSize;

                    foreach (char c in currentString)
                    {
                        string cString = c.ToString();
                        GUI.skin.label.fontSize = sizes[i];
                        GUI.Label(new Rect(t, (Screen.height - size.y) / 2 + offsetY, sizes[i], size.y), cString);
                        t += MAX_WORD_SIZE;
                        i++;
                    }
                    GUI.skin.label.fontSize = q;
                    resetColor();
                }
                else if (check == 3)            // Scale and rotate
                {
                    setRotation();
                    setFontSize();
                    GUI.Label(new Rect((Screen.width - size.x) / 2 + offsetX, (Screen.height - size.y) / 2 + offsetY, size.x, size.y), currentString);
                    resetFontSize();
                    resetRotation();
                }
                else if (check == 4)            // Different letter and rotate
                {
                    setRotation();
                    int t = (int)((Screen.width - size.x) / 2 + offsetX);
                    int i = 0;
                    int q = GUI.skin.label.fontSize;

                    foreach (char c in currentString)
                    {
                        string cString = c.ToString();
                        GUI.skin.label.fontSize = sizes[i];
                        GUI.Label(new Rect(t, (Screen.height - size.y) / 2 + offsetY, sizes[i], size.y), cString);
                        t += MAX_WORD_SIZE;
                        i++;
                    }
                    GUI.skin.label.fontSize = q;
                    resetRotation();
                }

                // Easy random change during text view in medium level
                if (rd.Next(400) == 0)
                    reset(1);
            }
            else if (difficulty == 2)     // hard
            {
                if (check == 0)             // rotate and translate normal text
                {
                    camera.backgroundColor = backgroundColor;
                    rotate();
                    translate();
                    GUI.Label(new Rect((Screen.width - size.x) / 2 + offsetX, (Screen.height - size.y) / 2 + offsetY, size.x, size.y), currentString);
                    resetRotation();
                }
                else if (check == 1)        // translate different text size
                {
                    camera.backgroundColor = backgroundColor;
                    int t = (int)((Screen.width - size.x) / 2 + offsetX);
                    int i = 0;
                    int q = GUI.skin.label.fontSize;

                    foreach (char c in currentString)
                    {
                        string cString = c.ToString();
                        GUI.skin.label.fontSize = sizes[i];
                        GUI.Label(new Rect(t, (Screen.height - size.y) / 2 + offsetY, sizes[i], size.y), cString);
                        t += MAX_WORD_SIZE;
                        i++;
                    }
                    translate();
                    GUI.skin.label.fontSize = q;
                }
                else if (check == 2)    // rotate colored text
                {
                    rotate();
                    setRandomColor();
                    GUI.Label(new Rect((Screen.width - size.x) / 2 + offsetX, (Screen.height - size.y) / 2 + offsetY, size.x, size.y), currentString);
                    resetColor();
                    resetRotation();
                }
                else if (check == 3)    // rotate and translate different size text
                {
                    camera.backgroundColor = backgroundColor;
                    rotate();
                    translate();
                    int t = (int)((Screen.width - size.x) / 2 + offsetX);
                    int i = 0;
                    int q = GUI.skin.label.fontSize;

                    foreach (char c in currentString)
                    {
                        string cString = c.ToString();
                        GUI.skin.label.fontSize = sizes[i];
                        GUI.Label(new Rect(t, (Screen.height - size.y) / 2 + offsetY, sizes[i], size.y), cString);
                        t += MAX_WORD_SIZE;
                        i++;
                    }
                    resetRotation();
                }
                else if (check == 4)    // make text appear and dissapear
                {
                    if (rd.Next(1000) > 900 || isPause == 1)
                        GUI.Label(new Rect((Screen.width - size.x) / 2 + offsetX, (Screen.height - size.y) / 2 + offsetY, size.x, size.y), currentString);

                }

                // Medium random change during text view in medium level
                if (rd.Next(200) == 0 && isPause == 0)
                    reset(1);
            }

            // Enter listener
            if (Event.current.keyCode == KeyCode.Return && !string.IsNullOrEmpty(currentMessage.Trim()))
            {
                checkWord();
            }

            // Send button action
            if (GUI.Button(new Rect(Screen.width - 80, Screen.height - 40, 80, 20), "Send"))
            {
                if (!string.IsNullOrEmpty(currentMessage.Trim()))
                {
                    checkWord();
                }
                   
            }
        } else if (enterNameState)
        {
            GUI.skin.label.fontSize = 30;
            size = GUI.skin.label.CalcSize(new GUIContent("Enter name and get ready!"));
            GUI.Label(new Rect((Screen.width - size.x) / 2, (Screen.height - size.y) / 2 - size.y, size.x, size.y), "Enter name and get ready!");

            GUI.SetNextControlName("MyNameField");
            name = GUI.TextField(new Rect((Screen.width) / 2 - 100, (Screen.height) / 2, 200, 20), name);
            GUI.FocusControl("MyNameField");

            if (GUI.Button(new Rect((Screen.width - size.x) / 2, (Screen.height - size.y) / 2 + 2*size.y, size.x, size.y), "Start"))
            {
                if (name.Length != 0)
                {
                    enterNameState = false;
                    runningState = true;
                    resetGame();
                }
            }
            // Enter listener
            if (Event.current.keyCode == KeyCode.Return && !string.IsNullOrEmpty(name.Trim()))
            {
                enterNameState = false;
                runningState = true;
                resetGame();
            }

            if (Event.current.keyCode == KeyCode.Escape)
                enterNameState = false;
        }
        else
        {
            // Leaderboard state
            if (leaderBoardState) 
            {
                GUI.skin.label.fontSize = 50;
                size = GUI.skin.label.CalcSize(new GUIContent("Best of the best - Condorii de lunca"));
                GUI.Label(new Rect((Screen.width - size.x) / 2, 20, size.x, size.y), "Best of the best - Condorii de lunca");
                ok = 90;
                GUI.skin.label.fontSize = 25;
                for (int i = 0; i < leaderboardNames.Count; i++)
                {
                    if (name.Equals(leaderboardNames[i])) {
                        backupColor = GUI.color;
                        GUI.color = new Color(1, 0, 0, 1);
                    }
                    size = GUI.skin.label.CalcSize(new GUIContent(leaderboardNames[i]));
                    GUI.Label(new Rect(Screen.width / 2 - 150 , ok, size.x, size.y), leaderboardNames[i]);

                    size = GUI.skin.label.CalcSize(new GUIContent(Convert.ToString(leaderboardPoints[i])));
                    GUI.Label(new Rect(Screen.width / 2 + 20, ok, size.x, size.y), Convert.ToString(leaderboardPoints[i]));

                    if (name.Equals(leaderboardNames[i]))
                        GUI.color = backupColor;

                    ok += 35;
                }

                // Enter listener
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    leaderBoardState = false;
                    updatedLeaderboard = false;
                }
                if (GUI.Button(new Rect((Screen.width - 120) / 2, (Screen.height - 3 * size.y), 120, size.y), "Back"))
                {
                    leaderBoardState = false;
                    updatedLeaderboard = false;
                }

            }
            else // Start menu state
            {
                // Send button action
                GUI.skin.label.fontSize = 20;
                size = GUI.skin.label.CalcSize(new GUIContent("Start Game"));
                if (GUI.Button(new Rect((Screen.width - size.x) / 2, (Screen.height - size.y) / 2 - size.y, size.x, size.y), "Start Game"))
                {
                    enterNameState = true;
                }
                size = GUI.skin.label.CalcSize(new GUIContent("Show Leaderboard"));
                if (GUI.Button(new Rect((Screen.width - size.x) / 2, (Screen.height - size.y) / 2 + size.y, size.x, size.y), "Show Leaderboard"))
                {
                    leaderBoardState = true;
                }

                size = GUI.skin.label.CalcSize(new GUIContent("Exit"));
                if (GUI.Button(new Rect((Screen.width - size.x) / 2, (Screen.height - size.y) / 2 + 3*size.y, size.x, size.y), "Exit"))
                {
                    Application.Quit();
                }
            }
        }
    }

    private void setRotation()
    {
        backupRotation = GUI.matrix;
        GUIUtility.RotateAroundPivot(rotation, new Vector2(Screen.width/2, Screen.height/2));
    }

    private void resetRotation()
    {
        GUI.matrix = backupRotation;
    }

    private void setFontSize()
    {
        backupFontSize = GUI.skin.label.fontSize;
        GUI.skin.label.fontSize = fontSize;
    }

    private void resetFontSize()
    {
        GUI.skin.label.fontSize = backupFontSize;
    }

    private void setRandomColor()
    {
        backupColor = GUI.color;
        GUI.color = color;
    }

    private void resetColor()
    {
        GUI.color = backupColor;
    }

    private void checkWord()
    {
        bool b = false;
        if(error == 1)
        {
            if(currentMessage.Length >= currentString.Length-1 && currentMessage.Length <= currentString.Length + 1)
            {
                int minLength = currentMessage.Length < currentString.Length ? currentMessage.Length : currentString.Length;
                int nrError = 0;
                if (currentMessage.Length != currentString.Length)
                    nrError++;
                for (int i = 0; i < minLength;i++)
                {
                    if (currentMessage[i] != currentString[i])
                        nrError++;
                }

                if (nrError <= 1)
                    b = true;
            }
        }

        if (currentMessage.Equals(currentString) || b)
        {
            camera.backgroundColor = Color.red;
            correctSound.Play();

            if (getTime() > 20 * 1/currentString.Length)
            {
                switch (difficulty) 
                {
                    case 0:  score += 5;
                        break;
                    case 1: score += 10;
                        break;
                    case 2: score += 20;
                        break;
                }
            }
            else
            {
                switch (difficulty)
                {
                    case 0:
                        score += 1;
                        break;
                    case 1:
                        score += 5;
                        break;
                    case 2:
                        score += 10;
                        break;
                }
            }
            reset(0);          
        }
        else
        {
            lives--;
            camera.backgroundColor = Color.blue;
            wrongSound.Play();
            checkIfAlive();
            reset(0);
        }

        currentMessage = string.Empty;

        checkIfAlive();
        checkLevel();
        
    }

    private void checkIfAlive()
    {
        runningState = lives > 0;
        if (!runningState)
        {
            if (!updatedLeaderboard) {
                updateLeaderboard();
                updatedLeaderboard = true;

            }
            leaderBoardState = true;
        }     
    }

    void Awake()
    {
        source = GetComponents<AudioSource>();
        correctSound = source[1];
        wrongSound = source[2];
        ambientalSound = source[0];
        ambientalSound.Play();
    }

    private struct helper
    {
        public string username;
        public int score;
    }

    private void updateLeaderboard()
    {
        Debug.Log("Hellow, gurl");
        Debug.Log(leaderboardNames.Count);

        helper[] array = new helper[11];

        for (int i = 0; i < leaderboardNames.Count; i++)
        {
            array[i].username = leaderboardNames[i];
            array[i].score = leaderboardPoints[i];
        }

        array[10].username = name;
        array[10].score = score;

        // Sort the array
        Array.Sort(array, delegate (helper h1, helper h2)
        {
            return h2.score.CompareTo(h1.score);
        });

        // Write the leaderboard to the file
        FileStream fileStream = new FileStream(LEADERBOARD_FILENAME, FileMode.Create);
        StreamWriter writer = new StreamWriter(fileStream);
        leaderboardNames.Clear();
        leaderboardPoints.Clear();

        for (int i = 0; i < 10; i++)
        {
            writer.WriteLine(array[i].username);
            writer.WriteLine(array[i].score);

            leaderboardNames.Add(array[i].username);
            leaderboardPoints.Add(array[i].score);
        }
        writer.Close();
    }


    private void checkLevel()
    {
        if (score >= POINTSTOHARD)
        {
            difficulty = 2;
        }
        else if (score >= POINTSTOMEDIUM)
        {
            difficulty = 1;
        }
    }

    private void rotate()
    {
        if (isPause == 1)
            goto et1;
        if (rotateTime == 0)
        {
            rotateCheck = rd.Next(2);
            rotateTime += Time.deltaTime;
        } else if (rotateTime > maxRotateTime)
        {
            rotateTime = 0;
        }
        else
        {
            rotateTime += Time.deltaTime;
        }

        if (rotateCheck == 0)
        {
            offsetRotate++;
        }
        else if (rotateCheck == 1)
        {
            offsetRotate--;
        }

        et1:
        backupRotation = GUI.matrix;
        GUIUtility.RotateAroundPivot(offsetRotate, new Vector2(Screen.width / 2 + offsetX, Screen.height / 2 + offsetY));
    }

    private void translate()
    {
        if (isPause == 1)
            return;
        if (translateTime == 0)
        {
            translateCheck = rd.Next(4);
            translateTime += Time.deltaTime;
        }
        else if (translateTime >= maxTranslateTime)
        {
            translateTime = 0;
        }
        else
        {
            translateTime += Time.deltaTime;
        }
        if (translateCheck == 0 && (Screen.width - size.x) / 2 + offsetX > 0)
        {
            offsetX--;
        }
        else if (translateCheck == 1 && (Screen.height - size.y) / 2 + offsetY > 0)
        {
            offsetY--;
        }
        else if (translateCheck == 2 && (Screen.width + size.x) / 2 + offsetX < Screen.width)
        {
            offsetX++;
        }
        else if (translateCheck == 3 && (Screen.height + size.y) / 2 + offsetY < Screen.height - TEXTBOX_SIZE)
        {
            offsetY++;
        }
        else
        {
            translateTime = 0;
        }
    }

    public void Start()
    {
        // Get camera settings to change background color
        camera = GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;

        // Set black background color
        originalColor = new Color(0, 0, 0, 1);
        camera.backgroundColor = originalColor;
        bonusColor = new Color(1, 1, 1, 1);

        rd = new System.Random();
        words = new List<string>();
        sizes = new List<int>();

        leaderboardNames = new List<String>();
        leaderboardPoints = new List<int>();

        runningState = false;
        leaderBoardState = false;
        enterNameState = false;

        file = new StreamReader(WORDS_FILENAME);
        while ((line = file.ReadLine()) != null)
            words.Add(line);

        reset(0);
        difficulty = 0;
        name = "";

        // Analyze leaderboard file
        ok = 0;
        file = new StreamReader(LEADERBOARD_FILENAME);
        leaderboardNames.Clear();
        leaderboardPoints.Clear();
        while ((line = file.ReadLine()) != null)
        {
            if (ok % 2 == 0)
                leaderboardNames.Add(line);
            else
                leaderboardPoints.Add(Convert.ToInt32(line));
            ok++;
        }
        ok = 0;
        file.Close();
    }

    public void Update()
    {
        if (runningState)
        {
            if (isPause == 0)
                time += Time.deltaTime;
            if (difficulty == 0 && time >= easyTime)
            {
                lives--;
                checkIfAlive();
                reset(0);
            }
            else if (difficulty == 1 && time >= mediumTime)
            {
                lives--;
                checkIfAlive();
                reset(0);
            }
            else if (difficulty == 2 && time >= hardTime)
            {
                lives--;
                checkIfAlive();
                reset(0);
            }
        }
    }

    private double getTime()
    {
        if (difficulty == 0)
        {
            return easyTime - time;
        }
        else if (difficulty == 1)
        {
            return mediumTime - time;
        }
        else
        {
            return hardTime - time;
        }
    }

    // reset 0 -> hard reset
    // reset 1 soft reset (for mid change during gameplay)
    private void reset(int set)
    {
        if (set == 0)
        {
            currentString = words[rd.Next(words.Count)];
        }

        sizes.Clear();
        for (int i = 0; i < currentString.Length; i++)
        {
            sizes.Add(MAX_WORD_SIZE - rd.Next(15));
        }
        rotation = rd.Next(180);
        color = new Color((float)rd.NextDouble(), (float)rd.NextDouble(), (float)rd.NextDouble(), 1);
        backgroundColor = new Color((float)rd.NextDouble(), (float)rd.NextDouble(), (float)rd.NextDouble(), 1);
        fontSize = 20 + rd.Next(20);
        camera.backgroundColor = originalColor;
        if (set == 0)
        {
            time = 0;
            check = rd.Next(5);
            camera.backgroundColor = originalColor;
            offsetRotate = 0;
            offsetX = 0;
            offsetY = 0;

            easyTime = 0.85f * currentString.Length;
            mediumTime = 0.75f * currentString.Length;
            hardTime = 0.70f * currentString.Length;
        }
        
    }

    private void resetGame()
    {
        difficulty = 0;
        time = 0;
        offsetX = 0;
        offsetY = 0;
        offsetRotate = 0;
        score = 0;
        lives = 5;
        updatedLeaderboard = false;
    }
}
