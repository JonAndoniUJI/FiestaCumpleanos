using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRecorder : MonoBehaviour
{

    public AudioAnalyzer microPreferences;

    int numberOfRecord = 0;
    bool active;
    AudioClip recordedAudio;

    // Start is called before the first frame update
    void Start()
    {
        /*if (microPreferences.systemActivated == true)
        {
            if(microPreferences.recording && !active)
            {
                StartRecording();
            }
        }*/
    }

    public void StartRecording()
    {
        recordedAudio = GetComponent<AudioSource>().clip;
    }

    public void FinishRecording()
    {
        //active = false;
        SavWav.Save("RecordedAudio" + numberOfRecord, recordedAudio, true);
        Microphone.End(microPreferences.microValues.microphone);
        numberOfRecord++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
