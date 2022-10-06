using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using BreathLib;

public class MicBreathDetect : BreathLib.Unity.BreathDetectionMono
{
    /* Simple and Important Microphone Settings
        MicOn = Whether to use microphone or keyboard keys.
        threshold = How much sound is required to recognize breathing. 
            [Recommend 4 for laptop mics]
        sensitivitity: How Sensitive Mic is
        _mixerGroupMicrophone: Which audio mixer group (output) microphone should output to
            From AudioMixer, create a Microphone class, and set it's volume to -80 dB
    */
    [Header("Important Mic Settings")]
    public bool MicOn = true;
    public float threshold = 4;
    public float sensitivity = 100;
    public AudioMixerGroup _mixerGroupMicrophone;
    

    /* Settings related to displaying info about the mic
        MicCheckDot: A dot that lights up green if input is louder than threshold. Red Else.
    */
    [Header("Important Mic UI Settings")]
    public SpriteRenderer MicCheckDot;
    public TextMeshProUGUI loudnessText;
    public TextMeshProUGUI thresholdText; 
    
    // Variable Made public to see how loud mic input is
    //loudness: Variable used to store how loud mic input is 
    [Header("Observable Mic Value")]
    public float loudness = 0;
    AudioSource _audio;

    /* Epheremeral required by breath library intergration.
        Raw data required in order to aggregate data into a single probability value
        float BreathSample breathData: [Inbreath, Outbreath, Nobreath, Pitch, Volume ] {0-1, sum is 1}
            (Probability that breath is an inhale, exhale, or background noise)
    */
    [Header("Epheremeral Data for AiB Library")]
    // public float[] simpleData = {0.0f, 0.0f, 1.0f};
    public BreathSample breathSample;
    public override SupportedPlatforms SupportedPlatforms => SupportedPlatforms.MacOS | SupportedPlatforms.Windows | SupportedPlatforms.Linux;

    // Start is called before the first frame update
    void Start()
    {
        /* Microphone
            This sets the Audio Clip of the Audio Source to be the Microphone
            Microphone.Start() Variables
                1. selected Device
                2. bool for loop
                3. seconds of loop (no reason for 10s)
                4. Frequency we want to record. Default Unity value is 44100 = [AudioSettings.outputSampleRate]
            loop: true -> keep listening to microphone
            outputAudioMixerGroup: Where to output microphone's input sounds
        */
        _audio = GetComponent<AudioSource>(); //Microphone.devices[0]
        //_audio.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
        _audio.clip = Microphone.Start(null, true, 10, 44100);
        //_audio.clip = Microphone.Start("Default - Microphone (High Definition Audio Device)", true, 10, AudioSettings.outputSampleRate);
        _audio.loop = true;
        _audio.outputAudioMixerGroup = _mixerGroupMicrophone;
        while (!(Microphone.GetPosition(null) > 0)){ }
        _audio.Play();
        
        /* Mic never loads with code below!
        if ((Microphone.GetPosition(null) > 0)){
            _audio.Play();
        }*/
        
        // Display Loudness every 0.2 sec.
        InvokeRepeating("SetLoudnessText", 0.01f, 0.2f);
        
    }
    


    

    // Update is called once per frame
    void Update()
    {
        // Cannot use code below, Loudness stuck at 0.1
        /*if ((Microphone.GetPosition(null) > 0)){
            _audio.Play();
        }*/
        
        // Display Current Threshold
        SetThresholdText(threshold);

        // Calculate Loudness of Mic
        loudness = GetAveragedVolume() * sensitivity;

        // Light up MicCheckDot if input is loud enough
        // Color (Red, Green, Blue, Alpha): Values 0~1
        if (loudness > threshold){
            // Set Ephermeral Data
            // simpleData = new float[] {0.5f, 0.5f, 0.0f};
            // breathSample.In = 0.5f;
            breathSample.Out = 0.5f;
            breathSample.No = 0.0f;

            // Green Solid Color
            MicCheckDot.color = new Color (0, 1, 0.3f, 1);
        } else {
            // Set Ephermeral Data
            // simpleData = new float[] {0.0f, 0.0f, 1.0f};
            // If No is 1, other values do not matter.
            // breathSample.In = 0.5f;
            breathSample.Out = 0.5f;
            breathSample.No = 1.0f;

            // Red Semi-Transparent Color
            MicCheckDot.color = new Color (1, 0.3f, 0.3f, 0.3f);
        }

    }

    // Display Current Loudness
    private void SetLoudnessText(){
        loudnessText.text = "Loudness: " + loudness.ToString("0.0");
    }

    // Display Threshold
    private void SetThresholdText(float threshold){
        thresholdText.text = "Threshold: " + threshold.ToString("0.0");
    }

    // Get Value from Threshold Slider
    public void SetThresholdValue(float value){
        threshold = value;
    }

    // Get Microphone on or off
    public void SetMicOnValue(bool toggleVal){
        MicOn = toggleVal;
    }
    
    // For Microphone
    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        _audio.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }

    

    // public BreathSample[] aggregate = new BreathSample[4];

    /* :: Struct :: */
// | *** stuff above ***
// | float (no)
// | float (in)
// | float (mouth)
// | float (pitch)
// | float (volume)
// | float (no)
// | float (in)
// | float (mouth)
// | float (pitch)
// | float (volume)
// | float (no)
// | float (in)
// | float (mouth)
// | float (pitch)
// | float (volume)
// | float (no)
// | float (in)
// | float (mouth)
// | float (pitch)
// | float (volume)
// | *** stuff below ***

    /* :: Objects :: */
// | *** stuff above ***
// | pointer ( first )
// | pointer ( 2nd )
// | pointer ( 3rd )
// | pointer ( 4th )
// | *** stuff below ***


    // Getter Function to get Ephermeral Data
    public override BreathSample GetBreathSample()
    {
        // BreathSample is a Struct, not a class, so it copies the values within and not just a pointer to the object. 
        return breathSample;

        // float?[] temp = new float?[] { simpleData[2], simpleData[0], null, null, null };
        // return new BreathSample(temp);

        // BreathSample original = new BreathSample(); // 0
        // BreathSample newer = original;

        // Debug.Log(breathSample);
        // 0x450da3

        // original.In = 0.5f;
        // aggregate[0] = breathSample; // 0x450da3

        // breathSample.In = 0.0f;
        // aggregate[1] = breathSample; // 0x450da3

        // breathSample.In = 1.0f;
        // aggregate[2] = breathSample; // 0x450da3

        // breathSample.In = 0.75f;
        // aggregate[3] = breathSample; // 0x450da3


        /* :: Struct :: */
        // aggregate[0].In = 0.5f;
        // aggregate[1].In = 0.0f;
        // aggregate[2].In = 1.0f;
        // aggregate[3].In = 0.75f;

        /* :: Object :: */
        // aggregate[0].In = 0.75f;
        // aggregate[1].In = 0.75f;
        // aggregate[2].In = 0.75f;
        // aggregate[3].In = 0.75f;


        /* :: Struct :: */
        // newer.In == 0
        // original.In == 0.5f
        // original.In != newer.In

        /* :: Class/Object :: */
        // newer.In == 0.5
        // original.In == 0.5f
        // original.In == newer.In

    }
}
