using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;
using System.IO;
using System.Text;

public class GazeSwipe : MonoBehaviour
{
    static string Path = "C:\\Users\\mhci430\\Desktop\\GazeSwipeTest";
    public string log_file_name = "";
    static List<string> buffer = new List<string>();

  
    int time_int = 3;

    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    public Text Indication;
   
   
    //public ArrayList<Button> Buttons = new ArrayList<Button>();

    public Button[] button;

    public Text xCoord;
    public Text yCoord;
    public GameObject GazePoint;
    public GameObject MyGaze;

    private float _pauseTimer;
    private Outline _xOutline;
    private Outline _yOutline;


    public Texture texA;
    public Texture texB;
    public Texture texC;
    public Texture TexArrow;
   

    const int buttonCount = 25;

    public GameObject buttonPrefab;
    public Canvas canvasForButtons;
    public GameObject Gaze;
    public GameObject Arrow;
    //public GameObject VisualHint;

    Button[] VisualHintImage = new Button[buttonCount];
    Image[] GazeImage = new Image[1];
    Button[] buttons = new Button[buttonCount];

    static Vector2[] loc = new Vector2[buttonCount];
    bool[] visited = new bool[buttonCount];

    public int selectedbtn;
    public int targetbtn;
    public float TrialTimeStart;
    public float TrialTimeEnd;
    public float CompletionTime;
    static public float GazeAreaRadius = 10000.0f;

    //public GameObject tempGazeObj = Instantiate(MyGaze, canvasForButtons.transform) as GameObject;
    //static Button tempGaze = tempGazeObj.GetComponent<Button>(); 

    void Start()
    {


        //Button1.onClick.AddListener(MyClick);
        _xOutline = xCoord.GetComponent<Outline>();
        _yOutline = yCoord.GetComponent<Outline>();
        InvokeRepeating("timer", 1, 1);
        int i = 0;
        while (i < loc.Length)
        {
            //loc[i] = new Vector2(Random.Range(Screen.width * 0.1f, Screen.width * 0.9f), Random.Range(Screen.height * 0.1f, Screen.height * 0.9f));
            i = i + 1;
        }

        createButtons();
        showTarget();

        GameObject tempGazeObj = Instantiate(Gaze, canvasForButtons.transform) as GameObject;
        Image tempGaze = tempGazeObj.GetComponent<Image>();
        GazeImage[0] = tempGaze;

       

        string ThisTimeString = System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
        Debug.Log("Now: " + ThisTimeString);
        log_file_name = Path + "\\GazeSwipe_" + ThisTimeString+".txt";
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

        //float angle = SwipeAngle();

        Vector2 MyGazePosition=new Vector2(0.0f,0.0f);
        //WriteLog("C:\\Users\\mhci430\\Desktop\\GazeSwipeTest\\GazeTest.txt", "Haha");
        if (gazePoint.IsValid)
        {
            Vector2 gazePosition = gazePoint.Screen;
            MyGazePosition= gazePoint.Screen;
            yCoord.color = xCoord.color = Color.white;
            Vector2 roundedSampleInput = new Vector2(Mathf.RoundToInt(gazePosition.x), Mathf.RoundToInt(gazePosition.y));
            xCoord.text = "x (in px): " + roundedSampleInput.x;
            yCoord.text = "y (in px): " + roundedSampleInput.y;


            GazeImage[0].GetComponent<RectTransform>().position = new Vector3(gazePosition.x, gazePosition.y, 1.0f);

            VisualHint(gazePosition.x, gazePosition.y);





            //Instantiate(MyGaze, (gazePoint.Screen - new Vector2(Screen.width, Screen.height) / 2f) / GetComponentInParent<Canvas>().scaleFactor);
            //MyGaze.transform.localPosition = (gazePoint.Screen - new Vector2(Screen.width, Screen.height) / 2f) / GetComponentInParent<Canvas>().scaleFactor;
            //tempGaze.GetComponent<RectTransform>().position = new Vector2(500f, 500f);
            //Debug.Log("X: " + roundedSampleInput.x + " Y: " + roundedSampleInput.y);
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
        if(Input.touchCount > 0)  //if there is a touch event
        {
            float minError = Mathf.Infinity;
            float angle = SwipeAngle();
            if (angle <= 360.0f)
            {
                int[] test = new int[25];
                int testi = 0;
                float selectbtnangle = 100000;
                int selectedBtnIndex = 10000;
                Texture textureArrow = TexArrow;
                for (int i = 0; i < loc.Length; i++)
                {
                    float btnX = buttons[i].GetComponent<RectTransform>().position.x;
                    float btnY = buttons[i].GetComponent<RectTransform>().position.y;
                    Vector2 BtnEyeVector = new Vector2(btnX - MyGazePosition.x, btnY - MyGazePosition.y);
                    bool mytest = (float)BtnEyeVector.sqrMagnitude < (float)GazeAreaRadius;
                    Debug.Log("button:" + i + " X:" + btnX + " Y:" + btnY + " GX:" + MyGazePosition.x + " GY:" + MyGazePosition.y +" dis "+ BtnEyeVector.sqrMagnitude+" radius: "+GazeAreaRadius+"ans:"+mytest);
                    //Debug.Log("distance" + BtnEyeVector.sqrMagnitude);
                    if (BtnEyeVector.sqrMagnitude < GazeAreaRadius)
                    {
                       

                        test[testi] = i;
                        testi = testi + 1;
                        float BtnAngle = ComputeTheta(BtnEyeVector.x, BtnEyeVector.y);
                        float error = Mathf.Pow(BtnAngle - angle, 2.0f);

                        if (error < minError)
                        {
                            selectbtnangle = BtnAngle;
                            minError = error;
                            selectedBtnIndex = i;

                        }
                    }
                   

                }
                Debug.Log("Candidate:"+ test[0]+"/"+ test[1] + "/" + test[2] + "/" + test[3]);

                if (selectedBtnIndex != 10000)
                {
                    selectedbtn = selectedBtnIndex;
                    Debug.Log("We select : " + selectedBtnIndex + " selectbtnangle: " + selectbtnangle +" SwipeAngle: "+angle);
                    buttonClickCallBack(selectedBtnIndex);
                   
                    //Debug.Log("reset");
                    //int a=showTarget(selectedBtnIndex);
                    //buttons[selectedBtnIndex].onClick();
                }
            }
        }
    }







    IEnumerator MyDelay()
    {

        yield return new WaitForSeconds(30000);
    }


    public void WriteLog(string path, string datatext)
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
    private void MyClick()
    {
        Debug.Log("Press!!");
        Indication.text = "Press!";
    }

    public float SwipeAngle()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }
            if (t.phase == TouchPhase.Ended)
            {
                secondPressPos = new Vector2(t.position.x, t.position.y);
          
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
                //currentSwipe.Normalize();
                //double angle = Vector3.Angle(currentSwipe.y transform.forward);
                //Debug.Log("Swipelength:" + currentSwipe.sqrMagnitude);
                if (currentSwipe.sqrMagnitude > 10000.0f)
                {
                    float angle = ComputeTheta(currentSwipe.x, currentSwipe.y);
                    //Debug.Log("this is angle:" + angle);
                    return angle;
                }
            }
        }
        return 1000.0f;
    }

    private float ComputeTheta(float vectorX,float vectorY)
    {
        float theta;
        if (vectorX > 0)
        {   theta=(180/Mathf.PI)*Mathf.Atan(vectorY / vectorX);
        }
        else if (vectorX < 0 && vectorY >= 0)
        {
            theta= (180 / Mathf.PI) * (Mathf.Atan(vectorY / vectorX) + Mathf.PI);
        }
        else if (vectorX < 0 && vectorY < 0)
        {
            theta = (180 / Mathf.PI) * (Mathf.Atan(vectorY / vectorX) - Mathf.PI);
        }
        else if (vectorX == 0 && vectorY > 0)
        {
            theta = (180 / Mathf.PI) * Mathf.PI / 2;
        }
        else if (vectorX == 0 && vectorY < 0)
        {
            theta = (180 / Mathf.PI) * (-Mathf.PI / 2);
        }
        else
        {
            theta = 0.0f;
        }
        if (theta < 0)
        {
            theta = theta + 360;
        }
        return theta;

    }

    // write buffer to file and clear buffer



   

    void createButtons()
    {
       
        for (int i = 0; i < loc.Length; i++)
        {
            
            //Instantiate Button
            GameObject tempButtonObj = Instantiate(buttonPrefab, canvasForButtons.transform) as GameObject;
            Button tempButton = tempButtonObj.GetComponent<Button>();
            buttons[i] = tempButton;

         
            //Create rect for position
            //Rect buttonRect = new Rect(loc[i].x, loc[i].y, Screen.width * 0.025f, Screen.height * 0.05f);

            //Assign Position and Size of each Button 
            buttons[i].GetComponent<RectTransform>().position =new Vector2(500f-(i%5)*100f+ Screen.width*0.5f, 500f-(i/5)*100f+ Screen.height*0.5f);
            buttons[i].GetComponent<RectTransform>().sizeDelta = new Vector2(100f,100f);
            
            //Don't capture local variable
            int tempIndex = i;
            //Add click Event
            buttons[i].onClick.AddListener(() => buttonClickCallBack(tempIndex));


            ///Create visual for each button
            GameObject tempArrowObj = Instantiate(Arrow, canvasForButtons.transform) as GameObject;
            Button tempArrow = tempArrowObj.GetComponent<Button>();
            VisualHintImage[i] = tempArrow;

            /*
            VisualHintImage[i].GetComponent<RectTransform>().position = new Vector2(500f - (i % 5) * 100f, 500f - (i / 5) * 100f);
            Texture textureArrow = TexArrow;
            Sprite spriteHint = Sprite.Create((Texture2D)textureArrow, new Rect(0.0f, 0.0f, textureArrow.width,
            textureArrow.height), new Vector2(0.5f, 0.5f), 100.0f);

            //Change the Button Image
            //buttons[buttonIndex].image.sprite = UISprite;
            VisualHintImage[i].image.sprite = spriteHint;
            */



        }




    }

    //Called when Button is clicked
    void buttonClickCallBack(int buttonIndex)
    {
        //Debug.Log(buttonIndex);

        //Get Texture to change
        //Texture textureToUse = visited[buttonIndex] ? texA : texB;
        Texture textureToUse = texB;

        //Convert that Texture to Sprite
        Sprite spriteToUse = Sprite.Create((Texture2D)textureToUse, new Rect(0.0f, 0.0f, textureToUse.width,
            textureToUse.height), new Vector2(0.5f, 0.5f), 100.0f);

        //Change the Button Image
        buttons[buttonIndex].image.sprite = spriteToUse;

        //Flip Image
        visited[buttonIndex] = !visited[buttonIndex];

        TrialTimeEnd = Time.time;
        string error = "0";
        if (buttonIndex != targetbtn)
        {
            error = "1";
        }
        CompletionTime = TrialTimeEnd - TrialTimeStart;
       // string CompletionTimeText = CompletionTime.ToString();

        string datatext = "Time: " + CompletionTime.ToString() + " Error: " + error;
        Debug.Log(datatext);
        WriteLog(log_file_name, datatext);
        //show next target
        InvokeRepeating("showTarget", 3, 1);
    }



   private void showTarget() {
        //Convert that Texture to Sprite
        for (int i=0;i<25;i++)
        {
            Texture textureToUse = texA;
            Sprite spriteToUse = Sprite.Create((Texture2D)textureToUse, new Rect(0.0f, 0.0f, textureToUse.width,
              textureToUse.height), new Vector2(0.5f, 0.5f), 100.0f);

            //Change the Button Image
            
            buttons[i].image.sprite = spriteToUse;
        }

        targetbtn=(int)Mathf.Floor(Random.Range(0.0f, 25.0f));
        Texture textureTarget = texC;
        Sprite spriteTarget = Sprite.Create((Texture2D)textureTarget, new Rect(0.0f, 0.0f, textureTarget.width,
            textureTarget.height), new Vector2(0.5f, 0.5f), 100.0f);

        //Change the Button Image
        //buttons[buttonIndex].image.sprite = UISprite;
        buttons[targetbtn].image.sprite = spriteTarget;
        TrialTimeStart = Time.time;
        CancelInvoke("showTarget");

    }

    private void VisualHint(float eyeX, float eyeY)
    {
        //Method1
        /*
        for (int i = 0; i < loc.Length; i++)
        {
            //Destroy(VisualHintImage[i]);
            float btnX = buttons[i].GetComponent<RectTransform>().position.x;
            float btnY = buttons[i].GetComponent<RectTransform>().position.y;
            Vector2 BtnEyeVector = new Vector2(btnX - eyeX, btnY - eyeY);
            if(BtnEyeVector.sqrMagnitude < (float)GazeAreaRadius)
            {
                  
                    //VisualHintImage[i].GetComponent<RectTransform>().position = new Vector2 (btnX, btnY);

                //Create rect for position
                //Rect buttonRect = new Rect(loc[i].x, loc[i].y, Screen.width * 0.025f, Screen.height * 0.05f);

                //Assign Position and Size of each Button 
                //buttons[i].GetComponent<RectTransform>().position = new Vector2(500f - (i % 5) * 100f, 500f - (i / 5) * 100f);


                //float BtnAngle = ComputeTheta(BtnEyeVector.x, BtnEyeVector.y);
                //DrawArrow(buttons[i].GetComponent<RectTransform>().position,new Vector2(eyeX,eyeY),color.blue);

            }
            
        }
        */

        //Method 2
        Texture textureArrow = TexArrow;
        for (int i = 0; i < loc.Length; i++)
        {
            float btnX = buttons[i].GetComponent<RectTransform>().position.x;
            float btnY = buttons[i].GetComponent<RectTransform>().position.y;
            Vector2 BtnEyeVector = new Vector2(btnX - eyeX, btnY - eyeY);
            bool mytest = (float)BtnEyeVector.sqrMagnitude < (float)GazeAreaRadius;
            Debug.Log("button:" + i + " X:" + btnX + " Y:" + btnY + " GX:" +eyeX + " GY:" + eyeY + " dis " + BtnEyeVector.sqrMagnitude + " radius: " + GazeAreaRadius + "ans:" + mytest);
            //Debug.Log("distance" + BtnEyeVector.sqrMagnitude);
            if (BtnEyeVector.sqrMagnitude < GazeAreaRadius)
            {
                //Debug.Log("button " + i + "IS IN THE GAZE");
  
                float BtnAngle = ComputeTheta(BtnEyeVector.x, BtnEyeVector.y);
                
                VisualHintImage[i].GetComponent<RectTransform>().position = new Vector2(btnX, btnY);
                VisualHintImage[i].GetComponent<RectTransform>().rotation=Quaternion.Euler(new Vector3(0,0,BtnAngle));
                Sprite spriteHint = Sprite.Create((Texture2D)textureArrow, new Rect(0.0f, 0.0f, textureArrow.width,
                textureArrow.height), new Vector2(0.5f, 0.5f), 100.0f);
                VisualHintImage[i].image.sprite = spriteHint;
            }
            else
            {
                VisualHintImage[i].GetComponent<RectTransform>().position = new Vector2(1000000.0f, 1000000.0f);
                Sprite spriteHint = Sprite.Create((Texture2D)textureArrow, new Rect(0.0f, 0.0f, textureArrow.width,
                textureArrow.height), new Vector2(0.5f, 0.5f), 100.0f);


                //Change the Button Image
                //buttons[buttonIndex].image.sprite = UISprite;
                VisualHintImage[i].image.sprite = spriteHint;
            }




        }
    }
        private void DrawArrow(Vector2 from, Vector2 to, Color color)
    {
        /*
        Handles.BeginGUI();
        Handles.color = color;
        Handles.DrawAAPolyLine(3, from, to);
        Vector2 v0 = from - to;
        v0 *= 10 / v0.magnitude;
        Vector2 v1 = new Vector2(v0.x * 0.866f - v0.y * 0.5f, v0.x * 0.5f + v0.y * 0.866f);
        Vector2 v2 = new Vector2(v0.x * 0.866f + v0.y * 0.5f, v0.x * -0.5f + v0.y * 0.866f); ;
        Handles.DrawAAPolyLine(3, to + v1, to, to + v2);
        Handles.EndGUI();
        */
    }


}
