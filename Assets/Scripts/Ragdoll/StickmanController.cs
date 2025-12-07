using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StickmanController : MonoBehaviour
{
    public Muscle[] muscles;
    public Transform[] bones;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    Vector2 forceVector;
    [SerializeField]
    private float moveDelay;
    [SerializeField]
    private Rigidbody2D rightLeg;
    [SerializeField]
    private Rigidbody2D leftLeg;
    [SerializeField]
    private Rigidbody2D rightArm;
    [SerializeField]
    private Rigidbody2D leftArm;
    [SerializeField]
    private int musclePower = 100;

    [Space]

    [SerializeField]
    private Transform rightHandPoint;
    [SerializeField]
    private Transform leftHandPoint;

    public Rigidbody2D MainBody { get; set; }

    public Rigidbody2D RightArm => rightArm;
    public Rigidbody2D LeftArm => leftArm;
    public Transform RightHand => rightHandPoint;
    public Transform LeftHand => leftHandPoint;

    [SerializeField] Muscle[] armParts = new Muscle[4];

    float moveDelayPointer;
    int direction;

    // TO DO
    Menu menu;

    public bool isHostUpdate = true;

    private void Start()
    {
        armParts = muscles.ToList().Where(m => m.bone.name.Contains("Arm")).ToArray();

        MainBody = GetComponent<Rigidbody2D>();

        // TO DO
        menu = FindObjectOfType<Menu>();

        isHostUpdate = !Photon.Pun.PhotonNetwork.IsMasterClient && !MultiplayerManager.IsOffline && !menu;
    }

    private void FixedUpdate()
    {
        foreach (Muscle muscle in muscles)
        {
            muscle.FixAngularVelocity();
        }

        if (isHostUpdate)
            return;

        foreach (Muscle muscle in muscles)
        {
            muscle.ActivateMuscle();
        }

        var restRotation = transform.localScale.x < 0 ? -180 : 0;

        var force = muscles.First().force;

        //MainBody.MoveRotation(Mathf.LerpAngle(MainBody.rotation, restRotation, force * Time.deltaTime));

        
    }

    public void Stun()
    {
        musclePower = 0;
        Xyi();
    }

    public void IgnoreCollision(Collider2D collider, bool ignore = true)
    {
        foreach (var muscle in muscles)
        {
            Physics2D.IgnoreCollision(muscle.bone.GetComponent<Collider2D>(), collider, ignore);
        }
    }

    public void SetForceToArms(int force)
    {
        armParts.ToList().ForEach(a => a.force = force);
    }

    public void MoveRight()
    {
        direction = 1;

        while (Time.time > moveDelayPointer)
        {
            Step1(1);
            Step2(1, 0.085f);
            moveDelayPointer = Time.time + moveDelay;
        }
    }

    public void MoveLeft()
    {
        direction = -1;

        while (Time.time > moveDelayPointer)
        {
            Step1(-1);
            Step2(-1, 0.085f);
            moveDelayPointer = Time.time + moveDelay;
        }
    }

    void Step1(int dir, float delay = 0)
    {
        StartCoroutine(DelayInvoke());

        IEnumerator DelayInvoke()
        {
            yield return new WaitForSeconds(delay);

            rightLeg.AddForce(forceVector * dir, ForceMode2D.Impulse);
            leftLeg.AddForce(forceVector * -0.5f * dir, ForceMode2D.Impulse);
        }
        
    }

    void Step2(int dir, float delay = 0)
    {
        StartCoroutine(DelayInvoke());

        IEnumerator DelayInvoke()
        {
            yield return new WaitForSeconds(delay);

            rightLeg.AddForce(forceVector * -0.5f * dir, ForceMode2D.Impulse);
            leftLeg.AddForce(forceVector * dir, ForceMode2D.Impulse);
        }
    }

    public void SetMusclesPower(int power)
    {
        foreach (Muscle muscle in muscles)
        {
            muscle.force = power;
        }
    }

    void Xyi()
    {
        foreach (Muscle muscle in muscles)
        {
            muscle.force = musclePower;
        }
    }

}

[System.Serializable]
public class Muscle
{
    public Rigidbody2D bone;
    public Transform boneAnimated;
    public float restRotation;
    public float force;

    [HideInInspector]
    public Transform boneTransform;

    public void InitBoneTransform()
    {
        boneTransform = bone.transform;
    }

    public void FixAngularVelocity()
    {
        bone.angularVelocity = Mathf.Clamp(bone.angularVelocity, -360, 360);
    }

    public void ActivateMuscle()
    {
        restRotation = boneAnimated.rotation.eulerAngles.z;

        bone.MoveRotation(Mathf.LerpAngle(bone.rotation, restRotation, force * Time.deltaTime));
        
        //if(bone.transform.root.GetComponent<Stickman>().IsPlayer)
        //    if(bone.name.IndexOf("Hip") > -1)
        //        Debug.Log(restRotation +" - "+ bone.rotation);



        //bool condition = bone.name == "Hip_1" || bone.name == "Spine";

        //if(condition && force > 0 && Mathf.Abs(restRotation - bone.rotation) > 170 && bone.velocity.sqrMagnitude < 30)
        //{
        //    bone.rotation = restRotation;
            
        //    Debug.Log("Чиним");
        //}
    }

    public void DestroyMuscle()
    {
        if(bone.GetComponent<Joint2D>())
            Object.Destroy(bone.GetComponent<Joint2D>());

        Object.Destroy(bone);
    }
}
