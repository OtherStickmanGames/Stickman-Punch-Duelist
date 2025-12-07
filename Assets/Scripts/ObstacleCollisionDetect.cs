using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ObstacleCollisionDetect : MonoBehaviour
{
    StickmanController ragdoll;
    Stickman stickman;

    CameraHandler cameraHandler;

    [SerializeField] float maxVelocity;

    public System.Action<bool> OnSlowMo;

    GameObject vel;

    public static bool slowMo;

    float timer;

    // Start is called before the first frame update
    void Start()
    {
        maxVelocity = float.MinValue;

        vel = GameObject.Find("Velik");

        ragdoll = GetComponentInChildren<StickmanController>();
        stickman = GetComponent<Stickman>();

        cameraHandler = FindObjectOfType<CameraHandler>();

        foreach (var muscle in ragdoll.muscles)
        {
            muscle.bone.gameObject.AddComponent<CollisionPartDetector>().OnCollision += CollisionDetect;
        }
    }

    
    void Update()
    {
        var curVel = GetVelocity();

        if (maxVelocity < curVel)
        {
            maxVelocity = curVel;
        }

        if (vel)
            vel.GetComponent<TMPro.TMP_Text>().text = maxVelocity.ToString();

        if (curVel > 50000)
        {
            var cols = Physics2D.OverlapCircleAll(stickman.Hip.position, 1.5f);

            var obstacles = cols.Where(c => c.gameObject.CompareTag("Obstacle")).ToArray();

            foreach (var obstacle in obstacles)
            {
                obstacle.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }

            //SlowMotion();

        }
    }
    
    public void SlowMotion(float dur)
    {
        if (!slowMo)
        {
            slowMo = true;

            Time.timeScale /= 10;
            Time.fixedDeltaTime /= 10;
            
            StartCoroutine(Return(dur));
            
            if(MultiplayerManager.IsMaster)
                OnSlowMo?.Invoke(slowMo);

            cameraHandler.SlowMoFollow(stickman.Hip);
        }

        IEnumerator Return(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale *= 10;
            Time.fixedDeltaTime *= 10;

            slowMo = false;
            //OnSlowMo?.Invoke(slowMo);

            cameraHandler.PlayerFollowBack();
        }
    }

    void CollisionDetect(int force)
    {
        var cols = Physics2D.OverlapCircleAll(stickman.Hip.position, 1.5f);

        var obstacles = cols.Where(c => c.gameObject.CompareTag("Obstacle")).ToArray();

        foreach (var obstacle in obstacles)
        {
            obstacle.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            var idx = Random.Range(1, 5);
            obstacle.gameObject.layer = LayerMask.NameToLayer($"Obstacle_{idx}");

            SlowMotion(3);
        }
    }

    float GetVelocity()
    {
        float result = 0;

        foreach (var muscle in ragdoll.muscles)
        {
            result += muscle.bone.velocity.sqrMagnitude;
        }

        result /= ragdoll.muscles.Length;

        return result;
    }
}
