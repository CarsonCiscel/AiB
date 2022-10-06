using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;

/* Made by The Verse: Do Won Kim + Camryn Streib
 * Microphone Resources: 
 * https://support.unity.com/hc/en-us/articles/206485253-How-do-I-get-Unity-to-playback-a-Microphone-input-in-real-time-
 * https://www.youtube.com/watch?v=GHc9RF258VA&t=473s
 * Last Update 6/29/2022
 */
public class PresentationBreath : MonoBehaviour
{
    /* Square Has Separate Objects in each Corner
        Dot changes direction at each corner of square
        Corner1: Top Left Corner Object
        Corner2: Top Right Corner Object
        Corner3: Bottom Right Corner Object
        Corner4: Bottom Left Corner Object
    */
    [Header("Corner GameObject Components")]
    public GameObject Corner1;
    public GameObject Corner2;
    public GameObject Corner3;
    public GameObject Corner4;

    private Vector3 position1 = new Vector3(0, 0, 0);
    private Vector3 position2 = new Vector3(0, 0, 0);
    private Vector3 position3 = new Vector3(0, 0, 0);
    private Vector3 position4 = new Vector3(0, 0, 0);
    
    /* Setting for Breath Time Duration
        breathTime = 4.0f -> 4 seconds for each side (inhale, hold, exhale)
    */
    [Header("Breath Duration Setting")]
    public float breathTime = 4.0f;
    
    // To Choose which Text Component to display text on
    [Header("Timer Text Component")]
    public TextMeshProUGUI timerText;

    /* To Choose which Text Component to display Breath Phase Name on.
        Ex. "Inhale, Exhale" 
    */
    [Header("Breath Phase Text Component")]
    public TextMeshProUGUI breathText;

    

    /* Settings for changing Game's background color
        Set to smoothly interpolate between two colors as player inhale & exhale
        startColor = Color background starts with (Before inhale)
        endColor = Color background ends up with after inhale finishes and before exhale starts
    */ 
    [Header("Game Background Color Change")]
    public Color startColor = new Color32(37, 125, 97, 0);
    public Color endColor = new Color32(21, 77, 60, 0);

    /* Settings for changing the pitch of Game's background music
        Set to change pitch of background music as player inhale & exhale 
        [Inhale: High to Low pitch] [Exhale: Low to high pitch]

    */
    [Header("Game Background Music Pitch Change")]
    public AudioSource backgroundMusic;
    public float startPitch = 1.15f;
    public float endPitch = 0.9f;
    
    
    
    

    
    // Constants for states

    public enum BreathState
    {
        INHALE,
        HOLD_DOWN,
        EXHALE,
        HOLD_UP
    }

    // dotSpeed: Variable used in function to calculate how fast dot moves
    // private float dotSpeed = 0.0f;
    // currentTime: Variable to hold current time to display (Changes as dot moves)
    private float currentTime;
    
    public const float CLIP_LENGTH = 16.0f;

    /// <summary>
    /// Normalized value to be between 0 and 1 (0% to 100%)
    /// </summary>
    private float progress;
    public void SetProgress(float progress)
    {
        this.progress = progress;
    }

    
    // Start is called before the first frame update
    void Start()
    {   
        // Start of with state [Corner1 -> Corner2] "Inhale"
        // currentstate = (int) BreathState.INHALE;
        currentTime = 0.0f;

        // Initialize Text Objects
        SetTimerText(currentTime);
        SetBreathText((int) BreathState.INHALE);

        // Calculate Speed necessary to make the travel time along each line equal to desired breath Time
        // lineDistance: Very left edge of Corner2 - Very Right Edge of Corner1
        // dotSpeed: (distance from corner to corner) / (breath Time)
        Collider2D col1 = Corner1.GetComponent<Collider2D>();
        Collider2D col2 = Corner2.GetComponent<Collider2D>();

        float lineDistance = (col2.bounds.center.x - col2.bounds.extents.x) - (col1.bounds.center.x + col1.bounds.extents.x);
        // dotSpeed = lineDistance / breathTime;

        position1 = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        position2 = new Vector3(transform.position.x + lineDistance, transform.position.y, transform.position.z);
        position3 = new Vector3(transform.position.x + lineDistance, transform.position.y - lineDistance, transform.position.z);
        position4 = new Vector3(transform.position.x, transform.position.y - lineDistance, transform.position.z);

        

        // Set Background Color to startColor value
        Camera.main.GetComponent<Camera>().backgroundColor = startColor;
        
        // Set Background Music's pitch to startPitch value
        backgroundMusic.pitch = startPitch;
        
        /* DEBUGGING 
        Debug.Log(gameObject.name);
        Debug.Log("Number of microphones detected: " + Microphone.devices.Length);
        Debug.Log("First Microphone detected is " + Microphone.devices[0].ToString());
        Debug.Log(dotSpeed);
        Debug.Log("Corner1 Bound Center: " + col1.bounds.center.x);
        Debug.Log("Corner1 Bound edge: " + col1.bounds.extents.x);*/
    }


    

    // Update is called once per frame
    void Update () { 
        

        float breathProgress = 0;
        // Go Left [Corner1 -> Corner2] "Inhale" if mix input louder than threshold
        // if (Mic is on and loud enough) or (Mic is off and button pressed)
        if ( progress < 0.25f ) {
            breathProgress = UpdateBreathProgress(0, progress);
        }
    
        // Go Down Automatically [2 -> 3] "Hold" no matter what
        else if (progress < 0.5f) {
            breathProgress = UpdateBreathProgress(1, progress);

        } 
        // Go Right [3 -> 4] "Exhale" if mix input louder than threshold. 
        else if (progress < 0.75f) {
            breathProgress = UpdateBreathProgress(2, progress);
        } 
        // Go Up [4 -> 1] "Hold" no matter what
        else if (progress <= 1.0f) {
            breathProgress = UpdateBreathProgress(3, progress);
        }
        else {
            throw new System.Exception("Progress is out of bounds");
        }

        Debug.Log("BreathProgress: " + breathProgress + " | Progress: " + progress);

        currentTime = breathProgress * (CLIP_LENGTH/4);
        SetTimerText(currentTime);
    }

    public float UpdateBreathProgress(int state, float progress)
    {
        Debug.Assert(state >= 0 && state < 4, "State must be between 0 and 3");

        float[] progressValues = {0.0f, 0.25f, 0.50f, 0.75f, 1.0f};
        Vector3[] positions = {position1, position2, position3, position4, position1};

        float breathProgress = Mathf.InverseLerp(progressValues[state], progressValues[1+state], progress);
        
        //transform.Translate(Vector2.left * dotSpeed * Time.deltaTime, Space.World);
        transform.position = Vector3.Lerp(positions[state], positions[1+state], breathProgress);

        if (state == 0)
        {
            Camera.main.GetComponent<Camera>().backgroundColor = Color.Lerp(startColor, endColor, breathProgress);
            backgroundMusic.pitch = Mathf.Lerp(startPitch, endPitch, breathProgress);
        }
        else if (state == 2)
        {
            Camera.main.GetComponent<Camera>().backgroundColor = Color.Lerp(endColor, startColor, breathProgress);
            backgroundMusic.pitch = Mathf.Lerp(endPitch, startPitch, breathProgress);
        }

        SetBreathText(state);
        return breathProgress;
    }

    /// <summary>
    /// When Dot Collides with Object (Ex. Corner Object)
    /// </summary>
    /// <param name="col">Collider for when another object enters</param>
    // void OnTriggerEnter2D(Collider2D col)
    // {
    //     // NOTE: Can't use switch statement because Corner1.name is non-constant
        
    //     // DEBUGGING
    //     Debug.Log("Hit detected with Object " + col.gameObject.name);

    //     // If we hit a corner, change direction dot moves
    //     if(col.gameObject.name == Corner1.name){
    //         currentstate = INHALE;
    //     }
    //     else if (col.gameObject.name == Corner2.name){
    //         currentstate = HOLD_DOWN;
    //     }
    //     else if (col.gameObject.name == Corner3.name){
    //         currentstate = EXHALE;
    //     }
    //     else if (col.gameObject.name == Corner4.name){
    //         currentstate = HOLD_UP;
    //     }
    //     //Reset Timer & Phrase Text for each Phase 
    //     currentTime = 0;
    //     SetTimerText(currentTime);
    //     SetBreathText(0);
    // }

    /// <summary>
    /// Display Current Time to Text Object that timerText contains
    /// </summary>
    private void SetTimerText(float time)
    {
        timerText.text = time.ToString("0.00") + "s";
    }

    // Display Current Breath Phase to Text Object that breathText contains
    private void SetBreathText(int state){
        string phaseString = "";
        switch((BreathState) state){
            case BreathState.INHALE:
                phaseString = "Inhale";
                break;
            case BreathState.EXHALE:
                phaseString = "Exhale";
                break;
            case BreathState.HOLD_DOWN:
            case BreathState.HOLD_UP:
                phaseString = "Hold";
                break;
            default:
                break;
        }
        breathText.text = (phaseString + " For " + breathTime.ToString("0.00") + "s");
    }
    
    
}
