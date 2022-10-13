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

    [SerializeField] private GameObject playerSprite;


    private ParticleSystem jumpParticles;
    private ParticleSystem wallParticles;

    private PlayerController player;
    private BoxCollider2D boxCollider;

    private bool particleFeedbacksEnabled;
    private bool soundFeedbacksEnabled;
    private bool playerDeformationFeedbacksEnabled;

    private Vector2 refScale;

    public enum sounds { damage, victory, bounce, succion}

    // Start is called before the first frame update
    void Start()
    {
        particleFeedbacksEnabled = true;
        soundFeedbacksEnabled = true;
        playerDeformationFeedbacksEnabled = true;

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
        
    }

    public void JumpParticles()
    {
        if (!particleFeedbacksEnabled) return;
        jumpParticles.Play();
    }

    public void WallParticles()
    {
        if (!particleFeedbacksEnabled) return;
    }

    public void PlaySound()
    {
        if (!soundFeedbacksEnabled) return;
    }

    public void RemanentEffectActivate()
    {
        if (!playerDeformationFeedbacksEnabled) return;
    }

    public void RemanentEffectDeactivate()
    {
        if (!playerDeformationFeedbacksEnabled) return;
    }

    public void Stretch(float maxXSpeed, float maxYSpeed)
    {
        float maxX_scale = speedStretchValues.x, maxY_scale = speedStretchValues.y;
        float newXScale = LinearScale(maxX_scale, refScale.x, maxXSpeed, Mathf.Abs(player.Speed.x));
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


    private float LinearScale(float smax, float sref, float vmax, float v)
    {
        return (smax - sref) * v / vmax + sref;
    }

    private float ExponentialScale(float smax, float sref, float vmax, float v)
    {
        float a = (smax - sref) / (Mathf.Exp(vmax / 10) - 1);
        return (Mathf.Exp(v/10) - 1) * a + sref;
    }



}
