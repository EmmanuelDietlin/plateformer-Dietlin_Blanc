using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Feedbacks : MonoBehaviour
{
    [SerializeField] private Vector2 jumpStretchValues;
    [SerializeField] private Vector2 landingStretchValues;

    [SerializeField] private Toggle particlesFeedbacksToggle;
    [SerializeField] private Toggle soundFeedbacksToggle;
    [SerializeField] private Toggle playerDeformationFeedbacksToggle;


    private ParticleSystem jumpParticles;
    private ParticleSystem wallParticles;

    private PlayerController player;
    private BoxCollider2D collider;

    private bool particleFeedbacksEnabled;
    private bool soundFeedbacksEnabled;
    private bool playerDeformationFeedbacksEnabled;

    private Vector2 refScale;

    // Start is called before the first frame update
    void Start()
    {
        particleFeedbacksEnabled = true;
        soundFeedbacksEnabled = true;
        playerDeformationFeedbacksEnabled = true;

        jumpParticles = gameObject.GetComponent<ParticleSystem>();
        jumpParticles.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.01f, 20) });
        jumpParticles.Play();

        player = gameObject.GetComponent<PlayerController>();
        refScale = transform.localScale;
        collider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JumpParticles()
    {
        if (!particleFeedbacksEnabled) return;
        Debug.Log("doot");
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

    public void Stretch(float maxX_scale, float maxY_scale, float maxXSpeed, float maxYSpeed)
    {
        transform.localScale = new Vector3(newScale(maxX_scale, refScale.x, maxXSpeed, Mathf.Abs(player.Speed.x)),
            newScale(maxY_scale, refScale.y, maxYSpeed, Mathf.Abs(player.Speed.y)));
        //collider.size = refScale;
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

    private float newScale(float smax, float sref, float vmax, float v)
    {
        return (smax - sref) * v / vmax + sref;
    }




}
