using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public bool enemy = false;
    private bool moving = true;
    public int health;
    public int armor;
    public int damage;
    public string obj;
    public bool Base;
    Animator m_Animator;
    Rigidbody m_Rigidbody;
    SphereCollider m_Sphere_Collider;

    private int lengthOfLineRenderer = 2;
    private Color c1 = Color.red;
    private Color c2 = Color.red;
    private float range = 0.0f;
    private bool justKilled = false;
    public bool started = false;
    private bool mousedown = false;
    private Vector3 offset;
    GameObject referenceObject;
    Movement referenceScript;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Sphere_Collider = gameObject.GetComponent<SphereCollider>();
        range = m_Sphere_Collider.radius;
          
        //in some cases the object will already have a LineRenderer
        if(gameObject.GetComponent<LineRenderer>() == null){
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.05f;
            lineRenderer.positionCount = lengthOfLineRenderer;
            float alpha = 1.0f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
                );
            lineRenderer.colorGradient = gradient;
          }
        
        
        if(enemy){ 
             gameObject.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
        }
        //this determines which objects are enemies
        obj = "ObjectTwo";
        if(enemy){obj = "ObjectOne";}

        if(!Base){
            //these four lines path them towards the nearest enemy
            var enemies = getAllEnemies();
            var closest = GetClosestEnemy(enemies);
            Vector3 relativePos =  closest.position - transform.position;
            transform.rotation = Quaternion.LookRotation(relativePos);
        }

        if(m_Sphere_Collider.enabled){
            m_Sphere_Collider.enabled = !m_Sphere_Collider.enabled;
        }
        
       
    }

    // Update is called once per frame
    void Update()
    {
      if(started){
        if(!m_Sphere_Collider.enabled){
           m_Sphere_Collider.enabled = !m_Sphere_Collider.enabled;
        }
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        var playerPosition = transform.position;
        
        // points line at enemy within range
        for (int i = 0; i < lengthOfLineRenderer; i++)
        {
          if(moving || referenceScript == null){
            if(enemy){
              lineRenderer.SetPosition(i, playerPosition - new Vector3(i*range/2 , 0 , 0));
            }
            else{
              lineRenderer.SetPosition(i, playerPosition + new Vector3(i*range/2 , 0 , 0));
            }
          }
          else{
              var enemyPosition = referenceScript.transform.position;
              if(i == 0){lineRenderer.SetPosition(i, playerPosition);}
              else{lineRenderer.SetPosition(i, enemyPosition);}
          }
        }

        if(!Base){
          if(moving){ 
              if(justKilled){
                var enemies = getAllEnemies();
                var closest = GetClosestEnemy(enemies);
                Vector3 relativePos =  closest.position - transform.position;
                transform.rotation = Quaternion.LookRotation(relativePos);
                justKilled = false;
              }
              m_Animator.SetBool("Moving", true);  
          }
          else if(!m_Animator.GetBool("Attack1Trigger") && !moving)
          {            
            if(referenceObject == null){
                moving = true;
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
                m_Animator.ResetTrigger("Attack1Trigger");
                m_Animator.SetBool("Moving", true);
                justKilled = true;
            }
            else{
              m_Animator.SetTrigger("Attack1Trigger");
              referenceScript.takeDamage(damage);
              m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
              if(referenceScript.getHealth() <= 0 || referenceScript == null){
                  Destroy(referenceObject);
                  moving = true;
                  m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
                  m_Animator.ResetTrigger("Attack1Trigger");
                  m_Animator.SetBool("Moving", true);
                  justKilled = true;
              }
            }
            
           
          }

          else{
            m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
          }
        }
        
      } 
      else{
          //starts the action, unless the character is not on the map
          if(Input.GetKeyDown (KeyCode.Space) && transform.position.x > -24 && transform.position.x  < 29){
             started = true;
          }
         }
      }

    //this is how characters take damage, being told a damage amount and subtracting amor
    public void takeDamage(int damagetaken){
      if((damagetaken-armor)>0)
      {health = health -(damagetaken-armor) ;}
    }

    public int getHealth(){
      return health;
    }

    private Transform GetClosestEnemy (Transform[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach(Transform potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
     
        return bestTarget;
    }
    
    private Transform[] getAllEnemies(){
      var tagg = "ObjectOne";
      if(!enemy){tagg = "ObjectTwo";}
      var enemiesObjects = GameObject.FindGameObjectsWithTag(tagg);
      Transform[] enemies = new Transform[enemiesObjects.Length];
      int i = 0;
      foreach(GameObject ene in enemiesObjects){
          enemies[i] = ene.transform;
          i++;
      }
      return enemies;
    }


    void OnMouseDown(){
      mousedown = false;
    }

    void OnMouseDrag()    {
      if(!mousedown){
          if (Input.GetKey (KeyCode.LeftControl)){    
              Instantiate(this, new Vector3(transform.position.x, 0,transform.position.z),transform.rotation);
              mousedown = true;
          }
          else{
            float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen ));
          }
      }
     }
     void OnMouseUp(){
       mousedown = true;
     }
     
    
    void OnTriggerEnter(Collider collision)
    {
      

        if(collision is CapsuleCollider && collision.gameObject.tag == obj && !Base){
            
            referenceObject = collision.gameObject;
            
            referenceScript = referenceObject.GetComponent<Movement>();
            
            moving = false;
            m_Animator.SetBool("Moving", false);
            m_Animator.SetTrigger("Attack1Trigger");
            referenceScript.takeDamage(damage);
            if(referenceScript.getHealth() == 0){
              Destroy(referenceObject);
              moving = true;
            }
            Vector3 relativePos =  referenceScript.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(relativePos);
        }
    }

}
