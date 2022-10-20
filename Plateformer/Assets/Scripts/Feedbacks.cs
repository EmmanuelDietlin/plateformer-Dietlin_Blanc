using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Feedbacks : MonoBehaviour
{
    [SerializeField] private Vector2 landingStretchValues;
    [SerializeField] private Vector2 speedStretchValues;

    [SerializeField] private Toggle particlesFeedbacksToggle;
    [SerializeField] private Toggle soundFeedbacksToggle;
    [SerializeField] private Toggle playerDeformationFeedbacksToggle;
    [SerializeField] private Toggle damageFeedbacksToggle;

    [SerializeField] private GameObject playerSprite;

    [SerializeField] private float damageEffectDuration;
    [SerializeField] private float vibrationEffectDuration;
    [SerializeField] private float blinkDelay;

    [Header("Sound")]
    [Space(5)]
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip succionSound;
    [SerializeField] private AudioClip bounceSound;
    [Space(10)]

    private float blinkTimer;
    private float vibrationsTimer;

    private ParticleSystem jumpParticles;
    private ParticleSystem wallParticles;

    private PlayerController player;
    private BoxCollider2D boxCollider;

    private bool particleFeedbacksEnabled;
    private bool soundFeedbacksEnabled;
    private bool playerDeformationFeedbacksEnabled;
    private bool damageFeedbacksEnabled;

    private AudioSource audioSource;

    private Vector2 refScale;

    private float audioTimer;

    public enum sounds { damage, victory, bounce, succion}
    

    // Start is called before the first frame update
    void Start()
    {
        particleFeedbacksEnabled = true;
        soundFeedbacksEnabled = true;
        playerDeformationFeedbacksEnabled = true;
        damageFeedbacksEnabled = true;
        audioSource = gameObject.GetComponent<AudioSource>();
        audioTimer = 20f;
        audioSource.clip = null;

        jumpParticles = gameObject.GetComponent<ParticleSystem>();
        jumpParticles.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });
        jumpParticles.Play();

        player = gameObject.GetComponent<PlayerController>();
        refScale = transform.localScale;
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioTimer < 20f) audioTimer += Time.deltaTime;
    }

    public void OnDamageTaken()
    {
        if (!damageFeedbacksEnabled) return;
        blinkTimer = 0;
        StartCoroutine(BlinkEffect());
        PlaySound(sounds.damage);

    }

    public void JumpParticles()
    {
        if (!particleFeedbacksEnabled) return;
        jumpParticles.Play();
    }

    public void WallParticles()
    {
        if (!particleFeedbacksEnabled) return;
        //TODO: particules mur
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
                //audioSource.clip = bounceSound;
                //audioSource.volume = .2f;
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

    public void RemanentEffectActivate()
    {
        if (!playerDeformationFeedbacksEnabled) return;
        //TODO remanence
    }

    public void RemanentEffectDeactivate()
    {
        if (!playerDeformationFeedbacksEnabled) return;
        //TODO remanence
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
        yield return null;
    } 

    private IEnumerator GamepadVibrations()
    {
        while (vibrationsTimer < damageEffectDuration)
        {
            vibrationsTimer += Time.deltaTime;
            //if ()
            yield return null;
        }
    }



}
