using Cinemachine;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Vehicle : MonoBehaviour
{
    protected enum TypeControll
    {
        AI,
        Keyboard
    }

    protected TypeControll typeControll;

    [Header("Wheel Collider")]
    [SerializeField] protected WheelCollider frontRight;
    [SerializeField] protected WheelCollider frontLeft;
    [SerializeField] protected WheelCollider backRight;
    [SerializeField] protected WheelCollider backLeft;

    [Header("Wheel Mesh")]
    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform backRightTransform;
    [SerializeField] Transform backLeftTransform;

    [Header("Speedometer UI")]
    [SerializeField] protected TMP_Text Speedometer_Txt;

    [Header("Car Properties")]
    [SerializeField] public float breakingForce = 1000f;
    [SerializeField] protected AnimationCurve TurnAngle;

    [Header("Car Physics")]
    [SerializeField] protected float downForce = 50f;
    [SerializeField] protected GameObject centerMass;

    [Header("Audio Car")]
    [SerializeField] AudioClip LowAccelClip;
    [SerializeField] AudioClip LowDeAccelClip;
    [SerializeField] AudioClip HiAccelClip;
    [SerializeField] AudioClip HiDeAccelClip;

    [Header("Audio Car Setting")]
    [SerializeField] float pitchMultiplier = 0.8f;
    [SerializeField] float lowPitchMin = 1f;
    [SerializeField] float lowPitchMax = 6f;
    [SerializeField] float highPitchMultiplier = 0.25f;
    [SerializeField] float maxRolloffDistance = 500;
    [SerializeField] float dopplerLevel = 1;
    [SerializeField] bool useDoppler = true;

    [Header("Lap Number")]
    public int lap = -1;

    AudioSource m_LowAcc;
    AudioSource m_LowDeAcc;
    AudioSource m_HiAcc;
    AudioSource m_HiDeAcc;
    private bool m_StartSound;

    protected float vertical;
    protected float horizontal;
    //protected float maxSpeed;

    protected float currentAcceleration = 0f;
    protected float currentBreakingForce = 0f;
    protected float currentTurnAngle = 0f;
    protected float slipAngle;

    [Header("Gear")]
    public float[] gears;
    public int gearNum = 1;

    [Header("Engine")]
    [SerializeField] protected float engineRpm;
    [SerializeField] public float maxRpm = 5000;
    [SerializeField] protected float totalPower;
    public AnimationCurve enginePower;

    protected float smoothTime = 0.01f;


    virtual protected void Start()
    {

        if (Speedometer_Txt != null)
            StartCoroutine(CalculateSpeed());

    }

    protected virtual void Update()
    {
        if (GameManager.isStart)
        {
            CarAudio();

        }
    }

    virtual protected void FixedUpdate()
    {

        if (GameManager.isStart)
        {
            CheckInput();
            CalTotalPower();
            currentAcceleration = totalPower * vertical;
            SteeringCar();
            ApplyAcceleration();
            UpdateSingleWheel();
            ApplyBreakForce();
            addDownForce();
        }
    }

    #region method control car

    //Input checking like moving, pause game or breaking
    protected void CheckInput()
    {
        if (typeControll == TypeControll.Keyboard && !GameManager.isPause)
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal"); ;

            if (Input.GetKey(KeyCode.Space))
                currentBreakingForce = breakingForce;
            else currentBreakingForce = 0;

            if (Input.GetKey(KeyCode.Escape))
            {
                UIManager.Instance.ActiveMenu(UIManager.Ui.PauseMenu);
            }
        }
    }

    //Caculator Engine RPM and TotalPower
    protected void CalTotalPower()
    {
        totalPower = 3.6f * enginePower.Evaluate(engineRpm) * vertical;

        float velocity = 0.0f;
        if (engineRpm >= maxRpm)
        {
            //current:	The current value.
            //target: The value we are trying to reach.
            //currentVelocity: The current velocity, this value is modified by the function every time you call it.
            //smoothTime: Approximately the time it will take to reach the target.A smaller value will reach the target faster.
            engineRpm = Mathf.SmoothDamp(engineRpm, maxRpm - 500, ref velocity, 0.05f);
        }
        else
        {
            //current:	The current value.
            //target: The value we are trying to reach.
            //currentVelocity: The current velocity, this value is modified by the function every time you call it.
            //smoothTime: Approximately the time it will take to reach the target.A smaller value will reach the target faster.
            engineRpm = Mathf.SmoothDamp(engineRpm, 1000 + (Mathf.Abs(engineRpm) * (gears[gearNum])), ref velocity, smoothTime);
        }
    }

    //apply acceleration to back wheel
    protected void ApplyAcceleration()
    {
        backRight.motorTorque = currentAcceleration / 2;
        backLeft.motorTorque = currentAcceleration / 2;
    }

    //Apply breaking force to all wheel
    protected void ApplyBreakForce()
    {
        frontRight.brakeTorque = currentBreakingForce;
        frontLeft.brakeTorque = currentBreakingForce;
        backRight.brakeTorque = currentBreakingForce;
        backLeft.brakeTorque = currentBreakingForce;
    }

    // take care of steerings
    protected virtual void SteeringCar()
    {
        //Angle to turn is evaluate base one velocity of car (the velocity will be mps -> *3.6f make it kmph)
        //like 50km/h -> 30 degree 
        currentTurnAngle = TurnAngle.Evaluate(gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3.6f) * horizontal;

        currentTurnAngle += Vector3.SignedAngle(transform.forward, gameObject.GetComponent<Rigidbody>().velocity + transform.forward, Vector3.up);

        //Making the wheels not go like a circle
        currentTurnAngle = Mathf.Clamp(currentTurnAngle, -90f, 90f);

        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
    }

    //Animate wheel moving
    protected void UpdateSingleWheel()
    {
        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);
    }
    protected void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 pos;
        Quaternion rotation;
        col.GetWorldPose(out pos, out rotation);

        trans.position = pos;
        trans.rotation = rotation;
    }

    // Add physic to car
    protected void addDownForce()
    {
        GetComponent<Rigidbody>().AddForce(-transform.up * downForce * GetComponent<Rigidbody>().velocity.magnitude);
        GetComponent<Rigidbody>().centerOfMass = centerMass.transform.localPosition;
    }


    IEnumerator CalculateSpeed()
    {
        bool isPlaying = true;
        while (isPlaying)
        {
            yield return new WaitForFixedUpdate();
            Speedometer_Txt.text = (Mathf.RoundToInt(gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3.6f)).ToString("0") + " km/h";
        }

    }

    #endregion

    #region method audio

    protected void CarAudio()
    {
        // get the distance to main camera
        float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;

        // stop sound if the object is beyond the maximum roll off distance
        if (m_StartSound && camDist > maxRolloffDistance * maxRolloffDistance)
        {
            StopSound();
        }

        if (!m_StartSound) StartSound();
        // The pitch is interpolated between the min and max values, according to the car's revs.
        float pitch = ULerp(lowPitchMin, lowPitchMax, engineRpm / maxRpm);
        // clamp to minimum pitch (note, not clamped to max for high revs while burning out)
        pitch = Mathf.Min(lowPitchMax, pitch);

        if (m_StartSound)
        {

            // adjust the pitches based on the multipliers
            m_LowAcc.pitch = pitch * pitchMultiplier;
            m_LowDeAcc.pitch = pitch * pitchMultiplier;
            m_HiAcc.pitch = pitch * highPitchMultiplier * pitchMultiplier;
            m_HiDeAcc.pitch = pitch * highPitchMultiplier * pitchMultiplier;

            // get values for fading the sounds based on the acceleration
            float accFade = Mathf.Abs((vertical > 0) ? vertical : 0);

            float decFade = 1 - accFade;

            // get the high fade value based on the cars revs
            float highFade = Mathf.InverseLerp(0.2f, 0.8f, engineRpm / 10000);
            float lowFade = 1 - highFade;

            // adjust the values to be more realistic
            highFade = 1 - ((1 - highFade) * (1 - highFade));
            lowFade = 1 - ((1 - lowFade) * (1 - lowFade));
            accFade = 1 - ((1 - accFade) * (1 - accFade));
            decFade = 1 - ((1 - decFade) * (1 - decFade));

            // adjust the source volumes based on the fade values
            m_LowAcc.volume = lowFade * accFade;
            m_LowDeAcc.volume = lowFade * decFade;
            m_HiAcc.volume = highFade * accFade;
            m_HiDeAcc.volume = highFade * decFade;

            // adjust the doppler levels
            m_HiAcc.dopplerLevel = useDoppler ? dopplerLevel : 0;
            m_LowAcc.dopplerLevel = useDoppler ? dopplerLevel : 0;
            m_HiDeAcc.dopplerLevel = useDoppler ? dopplerLevel : 0;
            m_LowDeAcc.dopplerLevel = useDoppler ? dopplerLevel : 0;
        }
    }

    //Play Sound
    protected void StartSound()
    {
        m_HiAcc = SetUpEngineSound(HiAccelClip);
        m_HiDeAcc = SetUpEngineSound(HiDeAccelClip);
        m_LowAcc = SetUpEngineSound(LowAccelClip);
        m_LowDeAcc = SetUpEngineSound(LowDeAccelClip);
        m_StartSound = true;
    }

    //Destroy Audio source
    protected void StopSound()
    {
        foreach (var source in GetComponents<AudioSource>())
        {
            Destroy(source);
        }
        m_StartSound = false;
    }

    //Add sound
    AudioSource SetUpEngineSound(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0f;
        source.spatialBlend = 1;
        source.loop = true;

        source.Play();
        return source;
    }

    // unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }

    #endregion


    // Make sound when hit something like another car or barrier
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Barrier") || collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("AiDriver"))
        {
            SoundManager.Instance.HitSfx(gameObject);
        }
    }
}
