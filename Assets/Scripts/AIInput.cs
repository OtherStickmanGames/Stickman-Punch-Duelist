using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour
{
    [SerializeField] Transform hip;

    Stickman controller;
    Stickman player;

    float cooldown;

    readonly string[] ebososes = new string[] 
    {
        "Узрите Писю",
        //"Уебатор 2000",
        "Ебучий сфинктор",
        "Уля ля",
        "Жопорезка",
        "Забор в Вагине",
        "Жри Солому",
        "Творога нет",
        "Капуста",
        "Хе-хе-хе, бой",
        "Стоша Говнозад",
        "Ауууф",
        "И че ты мне сделаешь?",
        "Сливать - мое призвание",
    };

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerInput>()?.GetComponent<Stickman>();

        controller = GetComponent<Stickman>();
        controller.NickName = ebososes[Random.Range(0, ebososes.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            var distance = Vector2.Distance(hip.transform.position, player.Hip.transform.position);

            if (distance > 1.9f)
            {
                if (hip.transform.position.x > player.Hip.transform.position.x)
                {
                    controller.StartWalk(-1);
                }
                else
                {
                    controller.StartWalk(1);
                }
            }
            else
            {
                if (controller.IsMove)
                    controller.StopWalk();

                if (cooldown > 1)
                {
                    Punch();
                    cooldown = 0;
                }
            }
        }

        cooldown += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (var item in GetComponentsInChildren<Rigidbody2D>())
            {
                item.AddForce(Vector2.right * 3000, ForceMode2D.Impulse);
            }
            
        }
    }

    void Punch()
    {
        var random = Random.Range(0, 4);

        switch (random)
        {
            case 0:
                controller.Punch(PunchPart.RightArm);
                break;
            case 1:
                controller.Punch(PunchPart.LeftArm);
                break;
            case 2:
                controller.Punch(PunchPart.RightLeg);
                break;
            case 3:
                controller.Punch(PunchPart.LeftLeg);
                break;
            default:
                break;
        }
    }
}
