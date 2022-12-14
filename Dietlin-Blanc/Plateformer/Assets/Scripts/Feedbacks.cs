using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Feedbacks : MonoBehaviour
{
    [SerializeField] private Vector2 speedStretchValues;

    [SerializeField] private Toggle particlesFeedbacksToggle;
    [SerializeField] private Toggle soundFeedbacksToggle;
    [SerializeField] private Toggle playerDeformationFeedbacksToggle;
    [SerializeField] private Toggle damageFeedbacksToggle;
    [SerializeField] private Toggle vibrationsFeedbackToggle;

    [SerializeField] private GameObject playerSprite;


    [SerializeField] private float damageEffectDuration;
    [SerializeField] private float vibrationEffectDuration;
    [SerializeField] private float blinkDelay;

    [Header("Color")]
    [SerializeField] private Color noMoreJumpsColor;

    [Header("Sound")]
    [Space(5)]
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip succionSound;
    [SerializeField] private AudioClip bounceSound;
    [Space(10)]

    [Header("Camera shake")]
    [Space(5)]
    [SerializeField] private float shakeDuration = 0f;
    [SerializeField] private float shakeAmount = 0.7f;
    [Space(10)]

    [Header("Gamepad vibations")]
    [Space(5)]
    [SerializeField, Range(0, 1)] private float damageLowFreqVibrations;
    [SerializeField, Range(0, 1)] private float damageHighFreqVibrations;


    private float blinkTimer;
    private float vibrationsTimer;
    private float shakeTimer;

    private ParticleSystem jumpParticles;
    private ParticleSystem wallParticles;

    private PlayerController player;
    private BoxCollider2D boxCollider;

    private Transform cam_main;
    private Vector3 originalPos;

    private bool particleFeedbacksEnabled;
    private bool soundFeedbacksEnabled;
    private bool playerDeformationFeedbacksEnabled;
    private bool damageFeedbacksEnabled;
    private bool vibrationsFeedbackEnabled;

    private AudioSource audioSource;

    private Vector2 refScale;

    private float audioTimer;
    private float immunityTimer;

    private bool blinkingActive;
    private bool shakingActive;
    private bool vibrationsActive;

    private Color baseColor;
    private SpriteRenderer rend;

    public enum sounds { damage, victory, bounce, succion}
    

    // Start is called before the first frame update
    void Start()
    {
        particleFeedbacksEnabled = true;
        soundFeedbacksEnabled = true;
        playerDeformationFeedbacksEnabled = true;
        damageFeedbacksEnabled = true;
        vibrationsFeedbackEnabled = true;
        audioSource = gameObject.GetComponent<AudioSource>();
        audioTimer = 20f;
        audioSource.clip = null;

        jumpParticles = gameObject.GetComponent<ParticleSystem>();
        jumpParticles.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });
        jumpParticles.Play();

        player = gameObject.GetComponent<PlayerController>();
        refScale = transform.localScale;
        boxCollider = gameObject.GetComponent<BoxCollider2D>();

        cam_main = Camera.main.transform;
        originalPos = cam_main.position;
        shakeTimer = 0f;
        vibrationsTimer = 0f;
        immunityTimer = damageEffectDuration;

        blinkingActive = false;
        shakingActive = false;
        vibrationsActive = false;

        rend = playerSprite.GetComponent<SpriteRenderer>();
        baseColor = rend.color;

    }

    // Update is called once per frame
    void Update()
    {
        if (audioTimer < 20f) audioTimer += Time.deltaTime;
        if (immunityTimer < 20f) immunityTimer += Time.deltaTime;
        if (player.JumpsLeft == 0) rend.color = noMoreJumpsColor;
        else rend.color = baseColor;
    }

    public void OnDamageTaken()
    {
        if (!damageFeedbacksEnabled) return;
        if (immunityTimer > damageEffectDuration) immunityTimer = 0f;
    
        if (!blinkingActive) StartCoroutine(BlinkEffect());
        if(!shakingActive) StartCoroutine(shakeCamera());
        if (vibrationsFeedbackEnabled && !vibrationsActive) StartCoroutine(GamepadVibrations(damageLowFreqVibrations, damageHighFreqVibrations));
        if (immunityTimer == 0f) PlaySound(sounds.damage);
    }

    public void JumpParticles()
    {
        if (!particleFeedbacksEnabled) return;
        jumpParticles.Play();
    }

    public void OnBounce()
    {
        PlaySound(sounds.bounce);
    }

    public void PlaySound(sounds s)
    {
        if (!soundFeedbacksEnabled) return;
        switch (s) {
            case sounds.damage:
                audioSource.clip = damageSound;
                audioSource.volume = .3f;
                break;
            case sounds.bounce:
                audioSource.clip = bounceSound;
                audioSource.volume = .2f;
                break;
            case sounds.victory:
                audioSource.clip = victorySound;
                audioSource.volume = .1f;
                break;
            case sounds.succion:
                audioSource.clip = succionSound;
                audioSource.volume = .1f;
                break;
        }
        if (s == sounds.bounce || audioTimer > audioSource.clip.length)
        {
            audioTimer = 0f;
            audioSource.Play();
        }
    }

    public void Stretch(float maxXSpeed, float maxYSpeed)
    {
        if (!playerDeformationFeedbacksEnabled) return;
        float maxX_scale = speedStretchValues.x, maxY_scale = speedStretchValues.y;
        float newXScale = LinearScale(maxX_scale, refScale.x, maxXSpeed, Mathf.Abs(player.CurrentHorizontalSpeed));
        float newYScale = ExponentialScale(maxY_scale, refScale.y, maxYSpeed, Mathf.Abs(player.Speed.y));
        playerSprite.transform.localScale = new Vector3(newXScale,newYScale);
    }


    public void OnParticleFeedbackChange()
    {
        particleFeedbacksEnabled = particlesFeedbacksToggle.isOn;
    }

    public void OnSoundFeedbacksChange()
    {
        soundFeedbacksEnabled = soundFeedbacksToggle.isOn;
    }

    public void OnPlayerDeformationFeedbacksChange()
    {
        playerDeformationFeedbacksEnabled = playerDeformationFeedbacksToggle.isOn;
    }

    public void OnDamageFeedbacksChange()
    {
        damageFeedbacksEnabled = damageFeedbacksToggle.isOn;
    }

    public void OnVibrationsFeedbackChange()
    {
        vibrationsFeedbackEnabled = vibrationsFeedbackToggle.isOn;
    }

    public void OnVictory()
    {
        PlaySound(sounds.victory);
    }


    private float LinearScale(float smax, float sref, float vmax, float v)
    {
        return (smax - sref) * v / vmax + sref;
    }

    private float ExponentialScale(float smax, float sref, float vmax, float v)
    {
        float a = (smax - sref) / (Mathf.Exp(vmax / 10) - 1);
        return (Mathf.Exp(v/10) - 1) * a + sref;
    }

    private IEnumerator BlinkEffect()
    {
        blinkingActive = true;
        while (blinkTimer < damageEffectDuration)
        {
            int status = (int)(blinkTimer / blinkDelay);
            blinkTimer += Time.deltaTime;
            if (status % 2 == 0)
            {
                playerSprite.SetActive(true);
            } else
            {
                playerSprite.SetActive(false);
            }
            yield return null;
        }
        if (!playerSprite.activeSelf) 
            playerSprite.SetActive(true);
        blinkingActive = false;
        blinkTimer = 0f;
        yield return null;
    } 

    private IEnumerator GamepadVibrations(float lowFreqSpeed, float highFreqSpeed)
    {
        vibrationsActive = true;
        Gamepad g = Gamepad.current;
        while (vibrationsTimer < vibrationEffectDuration)
        {
            vibrationsTimer += Time.deltaTime;
            if (g != null) g.SetMotorSpeeds(lowFreqSpeed, highFreqSpeed);
            yield return null;
        }
        if (g != null)
            g.SetMotorSpeeds(0f, 0f);
        //while (immunityTimer < damageEffectDuration) yield return null;
        yield return new WaitForSeconds(damageEffectDuration - vibrationEffectDuration);
        vibrationsTimer = 0f;
        vibrationsActive=false;
        yield return null;  
    }

    private IEnumerator shakeCamera()
    {
        shakingActive = true;
        while (shakeTimer < shakeDuration)
        {
            cam_main.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeTimer += Time.deltaTime * 3;
            yield return null;
        }
        cam_main.localPosition = originalPos;
        //while (immunityTimer < damageEffectDuration) yield return null;
        yield return new WaitForSeconds(damageEffectDuration - shakeDuration);
        shakeTimer = 0f;
        shakingActive = false;
        yield return null;
    }



}
