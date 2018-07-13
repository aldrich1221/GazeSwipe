using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;
using System.IO;
using System.Text;


public class EyeBehaviorTest : MonoBehaviour
{
    static string log_file_name = "C:\\Users\\mhci430\\Desktop\\GazeSwipeTest\\GazeTest.txt";
    static List<string> buffer = new List<string>();

    public Text Cross1;
    public Text Cross2;
    public Text Cross3;
    public Text Cross4;
    public Text Cross5;
    public Text Cross6;
    public Text Cross7;
    public Text Cross8;
    public Text Cross9;
    public Text Cross10;
    public Text Cross11;
    public Text Cross12;
    public Text Cross13;
    public Text Cross14;
    public Text Cross15;
    public Text Cross16;
    public Text Cross17;
    public Text Cross18;
    public Text Cross19;
    public Text Cross20;
    public Text Cross21;
    public Text Cross22;
    public Text Cross23;
    public Text Cross24;
    public Text Cross25;
    int time_int = 3;

    public Text Indication;



    public Text xCoord;
    public Text yCoord;
    public GameObject GazePoint;

    private float _pauseTimer;
    private Outline _xOutline;
    private Outline _yOutline;

    void Start()
    {
        _xOutline = xCoord.GetComponent<Outline>();
        _yOutline = yCoord.GetComponent<Outline>();
        InvokeRepeating("timer", 1, 1);
    }

    void Update()
    {
        if (_pauseTimer > 0)
        {
            _pauseTimer -= Time.deltaTime;
            return;
        }

        GazePoint.SetActive(false);
        _xOutline.enabled = false;
        _yOutline.enabled = false;

        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        Debug.Log("Go Eyetracking");

 

        WriteLog("C:\\Users\\mhci430\\Desktop\\GazeSwipeTest\\GazeTest.txt", "Haha");
        if (gazePoint.IsValid)
        {
            Vector2 gazePosition = gazePoint.Screen;
            yCoord.color = xCoord.color = Color.white;
            Vector2 roundedSampleInput = new Vector2(Mathf.RoundToInt(gazePosition.x), Mathf.RoundToInt(gazePosition.y));
            xCoord.text = "x (in px): " + roundedSampleInput.x;
            yCoord.text = "y (in px): " + roundedSampleInput.y;
            Debug.Log("X: " + roundedSampleInput.x + " Y: " + roundedSampleInput.y);
        }
        if (Input.GetKeyDown(KeyCode.Space) && gazePoint.IsRecent())
        {
            _pauseTimer = 3f;
            GazePoint.transform.localPosition = (gazePoint.Screen - new Vector2(Screen.width, Screen.height) / 2f) / GetComponentInParent<Canvas>().scaleFactor;
            yCoord.color = xCoord.color = new Color(0 / 255f, 190 / 255f, 255 / 255f);
            GazePoint.SetActive(true);
            _xOutline.enabled = true;
            _yOutline.enabled = true;
        }
    }
    public void WriteLog(string path,string datatext)
    {
        using (StreamWriter outputFile = new StreamWriter(log_file_name, true))
        {
            string[] lines = buffer.ToArray();
            foreach (string line in lines)
                outputFile.WriteLine(line);
            outputFile.Write(datatext); // write an specfic symbol to separate the time block

            outputFile.WriteLine("");
            buffer.Clear();
        }
    }

    

    public void timer()
    {

        time_int -= 1;

        Indication.text = time_int + "";

        if (time_int == 0)
        {
            Indication.text = "time\nup";
            CancelInvoke("timer");
        }
    }



    // write buffer to file and clear buffer
                  



}
