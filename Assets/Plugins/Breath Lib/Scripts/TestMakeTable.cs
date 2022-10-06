using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreathLib;

public class TestMakeTable : MonoBehaviour
{
    public TextAsset jsonFile;

	public GameObject[] point;

    //breathbar UI 
    public BreathBar breathBar;

    //breathbar value
    public float breathTime = 0.0f;

	public float interval = 0.0625f;
    
    private void Start()
    {
		Debug.Log(PatternLibrary.TableForPattern(jsonFile, interval));
        
        TableForPattern(PatternLibrary.GetBreathPattern(jsonFile), interval);
        StartCoroutine(TableForPattern(PatternLibrary.GetBreathPattern(jsonFile),interval));
	}

    public IEnumerator TableForPattern(Pattern pattern, float interval = 0.0625f)
    {
       for (float time = 0; time <= 1; time += interval)
        {
            BreathSample bs = pattern.GetTargetAtNormalizedTime(time);
           
           /* GameObject go;
            for (int index = 0; index < bs.Length; index++)
            {
                if (bs[index].HasValue)
                {
                    go = Instantiate(point);
                    go.transform.position = new Vector3(time*2, bs[index].Value + 1.2f * index, 0);
                }
               
            }*/

             GameObject go = Instantiate(point[0]);
             go.transform.position = new Vector3(time*2, pattern.GetTargetAtNormalizedTime(time).In.Value ,0);
             go = Instantiate(point[1]);
             go.transform.position = new Vector3(time*2, pattern.GetTargetAtNormalizedTime(time).No.Value + 1.2f , 0);
             go = Instantiate(point[2]);
             go.transform.position = new Vector3(time*2, pattern.GetTargetAtNormalizedTime(time).Mouth.Value + 1.2f, 0);
             go = Instantiate(point[3]);
             go.transform.position = new Vector3(time*2, pattern.GetTargetAtNormalizedTime(time).Pitch.Value/100, 0);
             go = Instantiate(point[4]);
             go.transform.position = new Vector3(time*2, pattern.GetTargetAtNormalizedTime(time).Volume.Value, 0);
             

            if(pattern.GetTargetAtNormalizedTime(time).No.Value == 1.0)
            {  
                breathBar.SetBreath(breathTime);
            }
            else if(pattern.GetTargetAtNormalizedTime(time).In.Value == 1.0)
            {   breathTime++;
                breathBar.SetBreath(breathTime);
            }
            else if(pattern.GetTargetAtNormalizedTime(time).In.Value == 0)
            {   breathTime--;
                breathBar.SetBreath(breathTime);
            }
          
             yield return new WaitForSecondsRealtime(1.0f);

        }
    }

    public GameObject dot;
    public float progress;
    public float lengthOfGraph;

    private Vector3 origin;

    private IEnumerator waitThenLoad()
    {
        yield return new WaitForSecondsRealtime(2.0f);
    }

    private void Awake()
    {
        if (dot != null)
            origin = dot.transform.position;
    }

    private void Update()
    {
       // if (dot != null)
        //    dot.transform.position = origin + new Vector3(progress * lengthOfGraph, 0, 0);


        
    }
}
