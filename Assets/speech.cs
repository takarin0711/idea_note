using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;

using System;

public class speech : MonoBehaviour
{
    public TextMesh InformationMesh;
    private GCSpeechRecognition _speechRecognition;
    //private InputField _contextPhrases;

    private ReactionManager _reactionManager;

    public UnityEngine.VR.WSA.Input.GestureRecognizer InputActionRecognizer { get; private set; }

    public enum State { Stop, Recording, Analyzing }
    public State Status { get; private set; }

    public int pStatus;
    // 仮
    private AudioSource _audioSource;
    public AudioClip ChargeSound;
    public AudioClip FireSound;
    public AudioClip ThinkSound;
    public AudioClip FailSound;

    public string Str;

    // 仮

    void Awake()
    {
        InputActionRecognizer = new UnityEngine.VR.WSA.Input.GestureRecognizer();
        InputActionRecognizer.SetRecognizableGestures(UnityEngine.VR.WSA.Input.GestureSettings.Hold);

        InputActionRecognizer.HoldStartedEvent += InputActionRecognizer_HoldStartEvent;
        InputActionRecognizer.HoldCompletedEvent += InputActionRecognizer_HoldCompletedEvent;
    }

    private void InputActionRecognizer_HoldStartEvent(UnityEngine.VR.WSA.Input.InteractionSourceKind source, Ray headRay)
    {
        _audioSource.clip = ChargeSound;
        _audioSource.Play();

        //StartRecordButtonOnClickHandler();
    }

    private void InputActionRecognizer_HoldCompletedEvent(UnityEngine.VR.WSA.Input.InteractionSourceKind source, Ray headRay)
    {
        _audioSource.clip = FireSound;
        _audioSource.Play();
        //StopRecordButtonOnClickHandler();
    }

    // Use this for initialization
    void Start()
    {
        _speechRecognition = GCSpeechRecognition.Instance;
        _speechRecognition.SetLanguage(Enumerators.LanguageCode.ja_JP);
        _speechRecognition.RecognitionSuccessEvent += SpeechRecognizedSuccessEventHandler;
        _speechRecognition.NetworkRequestFailedEvent += SpeechRecognizedFailedEventHandler;

        _reactionManager = ReactionManager.Instance;

        // 仮
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 1;
        _audioSource.dopplerLevel = 0;
        // 仮
        Status = State.Stop;
        pStatus = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        _speechRecognition.RecognitionSuccessEvent -= SpeechRecognizedSuccessEventHandler;
        _speechRecognition.NetworkRequestFailedEvent -= SpeechRecognizedFailedEventHandler;
    }

    //private void ApplySpeechContextPhrases()
    //{
    //    string[] phrases = _contextPhrases.text.Trim().Split(","[0]);

    //    if (phrases.Length > 0)
    //        _speechRecognition.SetSpeechContext(phrases);
    //}

    public void SpeechRecognizedFailedEventHandler(string obj, long l)
    {
        InformationMesh.text = "Speech Recognition failed with error: " + obj;
        _audioSource.clip = FailSound;
        _audioSource.Play();
        Status = State.Stop;
        pStatus = 0;
        //Str = null;
    }

    public void SpeechRecognizedSuccessEventHandler(RecognitionResponse obj, long l)
    {
        if (obj != null && obj.results.Length > 0)
        {
            // InformationMesh.text = "Speech Recognition succeeded! Detected Most useful: " + obj.results[0].alternatives[0].transcript;
            InformationMesh.text = "" + obj.results[0].alternatives[0].transcript;
            /*
            string other = "\nDetected alternative: ";

            foreach (var result in obj.results)
            {
                foreach (var alternative in result.alternatives)
                {
                    if (obj.results[0].alternatives[0] != alternative)
                        other += alternative.transcript + ", ";
                }
            }

            InformationMesh.text += other;
            */
            Str = obj.results[0].alternatives[0].transcript;

            SpeechReaction(obj);
        }
        else
        {
            InformationMesh.text = "Speech Recognition succeeded! Words are no detected.";
            _audioSource.clip = FailSound;
            _audioSource.Play();

        }
        Status = State.Stop;
        pStatus = 0;

        //Str = "";
        //Str = null;
    }

    private void SpeechReaction(RecognitionResponse obj)
    {
        //_reactionManager.Action(obj);
    }

    private void StopRuntimeDetectionButtonOnClickHandler()
    {
        _speechRecognition.StopRecord();
        InformationMesh.text = "";
    }

    public void StartRecordButtonOnClickHandler()
    {
        if (Status == State.Stop)
        {
            InformationMesh.text = "";
            _speechRecognition.StartRecord(false);
            Status = State.Recording;
            pStatus = 1;
        }
    }

    public void StopRecordButtonOnClickHandler()
    {
        if (Status == State.Recording)
        {
            //ApplySpeechContextPhrases();
            _speechRecognition.StopRecord();
            Status = State.Analyzing;
            pStatus = 2;
        }
        
    }
}
