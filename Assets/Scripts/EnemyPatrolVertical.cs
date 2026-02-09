using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPatrolVertical : MonoBehaviour
{   
    [Header("Patrol Bounds")]
    [SerializeField] private Transform edge1;
    [SerializeField] private Transform edge2;

    [UnitHeaderInspectable("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    private UnityEngine.Vector3 initialScale;
    private bool movingtoEdge1 = true;
    


    private void MoveInDirection(int direction)
    {
        //face in direction
        //get the abs value of the x component and multiply it by 1 or -1 to set the facing direction
        // enemy.localScale = new UnityEngine.Vector3(Mathf.Abs(initialScale.x)*direction, initialScale.y, initialScale.z); 

        //move in direction
        enemy.position = new UnityEngine.Vector3(enemy.position.x, 
            enemy.position.y + Time.deltaTime*direction*speed, enemy.position.z);
    }

    private void setDirection()
    {
        if (movingtoEdge1)
        {   
            if(enemy.position.y <= edge1.position.y)
                MoveInDirection(1);
            else
            {
                ChangeDirection();
            }
        }
        else
        {   if(enemy.position.y >= edge2.position.y)
                MoveInDirection(-1);
            else
            {
              ChangeDirection();
            }
        }
    }

    private void ChangeDirection()
    {
        movingtoEdge1 = !movingtoEdge1;
    }



    private void Awake()
    {
        initialScale = enemy.localScale;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   

        setDirection();

    }
}
