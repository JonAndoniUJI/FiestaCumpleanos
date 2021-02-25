using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioAnalyzer : MonoBehaviour
{

	[Range(64, 8192)] public int numberOfSamples = 1024; //step by 2
	public Slider sensitivitySlider;
	public MicrophoneInput microValues;
	public AudioRecorder grabadorAudio;
	DocumentWritter documentWritter;

	float rmsValue, dbValue, minLimitDecibelsValue,
		refValue = 0.01f;
	public float expireMicTime, expiringTime = 3;
	
	public bool answerWhileAsking = false, asking = false;

	//Variables que dan datos
	public float totalTime, stimulusTime, answerTime, interruptionTime,
		initialInteractionTotalTime, stimulusSystemTime, interruptionSystemTime, initialAnswerSystemTime, endingAnswerSystemTime;

	

	public bool recording, systemActivated, canStart = false;
	public Light recordingLight;
	public AudioClip recordedAudio;


	// Start is called before the first frame update
	void Start()
    {
		PlayPrefsManager.SetSensitivity(sensitivitySlider.value);
		minLimitDecibelsValue = PlayPrefsManager.GetSensitivity();
		Debug.Log(minLimitDecibelsValue);

		microValues = gameObject.GetComponent<MicrophoneInput>();
		documentWritter = gameObject.GetComponentInChildren<DocumentWritter>();

		recordingLight.color = Color.red;
		expireMicTime = expiringTime;

		sensitivitySlider.onValueChanged.AddListener(delegate {
			SensitivityValueChangedHandler(sensitivitySlider);
		});

		//Reset time values
		systemActivated = false;
		
		recording = false;
		canStart = false;
		answerWhileAsking = false;
		initialInteractionTotalTime = 0;
		
		stimulusTime = 0;
		stimulusSystemTime = 0;
		
		answerTime = 0;
		initialAnswerSystemTime = 0;
		endingAnswerSystemTime = 0;
		
		interruptionTime = 0;
		interruptionSystemTime = 0;
	}

    // Update is called once per frame
    void Update()
    {
		if(systemActivated)
		{

			float[] audioSamples = new float[numberOfSamples];

			//Array que almacena las frecuencias
			float[] frequencySamples = new float[numberOfSamples];

			GetComponent<AudioSource>().GetOutputData(audioSamples, 0); // filling arrays with samples
			GetComponent<AudioSource>().GetSpectrumData(frequencySamples, 0, FFTWindow.Rectangular);


			float sum = 0;
			for (int j = 0; j < numberOfSamples; j++)
			{
				sum += audioSamples[j] * audioSamples[j]; // sum squared samples
			}
			rmsValue = Mathf.Sqrt(sum / numberOfSamples); // rms = square root of average
			dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB


			if (dbValue < minLimitDecibelsValue)
			{
				if (!recording)
					dbValue = minLimitDecibelsValue; // clamp it to the min value
				else
				{
					expireMicTime -= Time.deltaTime;
					if (expireMicTime <= 0)
						FinishRecording();
				}
			}

			else
			{
				if (!recording && canStart)
				{
					StartRecording();
				}
				else
				{
					expireMicTime = expiringTime;
				}
			}

			Debug.Log("Valor de sonido: " + dbValue);
			
			ActualizarTiempos();
		}
	}

	public void ActualizarTiempos()
	{
		float elapsedTime = Time.deltaTime;
		if (systemActivated)
		{
			totalTime += elapsedTime;

			if (asking)
				stimulusTime += elapsedTime;

			if (recording)
				answerTime += elapsedTime;
		}
	}

	public void enableSystem()
	{
		systemActivated = true;
	}

	public void disableSystem()
	{
		systemActivated = false;
		documentWritter.closeDocument();
	}

	public void StartAsking()
	{
		if(systemActivated)
		{
			initialInteractionTotalTime = totalTime;
			asking = true;
			canStart = true;
		}
					
	}

	public void FinishAsking()
	{
		if (systemActivated)
		{
			asking = false;
			stimulusSystemTime = totalTime;
		}
	}

	public void SensitivityValueChangedHandler(Slider sensitivitySlider)
	{
		minLimitDecibelsValue = sensitivitySlider.value;
	}

	public void StartRecording()
	{
		if (asking)
		{
			answerWhileAsking = true;
			interruptionTime = stimulusTime;
			interruptionSystemTime = totalTime;
		}

		microValues.UpdateMicrophone();
		initialAnswerSystemTime = totalTime;
		grabadorAudio.StartRecording();
		recordingLight.color = Color.green;
		recording = true;		
	}

	public void FinishRecording()
	{
		endingAnswerSystemTime = totalTime;

		//Grabar los datos
		documentWritter.writeDocument(initialInteractionTotalTime, stimulusTime, stimulusSystemTime, answerTime,
			initialAnswerSystemTime, endingAnswerSystemTime, interruptionTime, interruptionSystemTime);

		//Finalizar la grabación y guardarla
		grabadorAudio.FinishRecording();
		
		
		//Resetear las variables necesarias
		recording = false;
		canStart = false;
		answerWhileAsking = false;
		initialInteractionTotalTime = 0;
		stimulusTime = 0;
		stimulusSystemTime = 0;
		answerTime = 0;
		initialAnswerSystemTime = 0;
		endingAnswerSystemTime = 0;
		interruptionTime = 0;
		interruptionSystemTime = 0;

		recordingLight.color = Color.red;
		microValues.UpdateMicrophone();
	}
}
