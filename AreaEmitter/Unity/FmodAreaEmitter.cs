using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FmodAreaEmitter : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference fmodEvent;

    [Header("Variables (can be detected automatically)")]
    [SerializeField] private GameObject player;
    [SerializeField] private Collider emitterCollider;

    [Header("Do you want to child colliders? Requires Rigidbody")]
    [SerializeField] private bool useChildColliders;
    [SerializeField] private Collider[] emitersChildColliders;

    private bool isPlayerInside;
    private float distanceToPlayer;

    private Vector3 closestPoint;                                  
    private Vector3 emitterPoint;
    private Vector3 playerPosition;

    private EventInstance AmbientEmitter;
    private EventDescription AudioDes;
    private float maxDistance;

    #region MonoBehaviour
    private void Awake()
    {   //In Awake script checks if it has neccesary objects to be operational, in other case it will disable itself                                            
        //it looks for Player (NOT CORRECT IF YOUR LISTENER POSITION IS NOT ON PLAYER) and collider/child colliders (depending on your use of single or multiple colliders in more complex shapes)

        if (player == null)             {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)             {
                Debug.LogWarning("FMOD Area Emitter could not detect a player. Will be disabled now.");
                this.GetComponent<FmodAreaEmitter>().enabled = !this.GetComponent<FmodAreaEmitter>().enabled;
            }
        }

        if (useChildColliders)          {
            emitersChildColliders = this.GetComponentsInChildren<Collider>();

            if (emitersChildColliders == null)     {
                Debug.LogWarning("FMOD Area Emitter could not detect a colliders on object. Will be disabled now.");
                this.GetComponent<FmodAreaEmitter>().enabled = !this.GetComponent<FmodAreaEmitter>().enabled;
            }
        }
        else if (emitterCollider == null)  {
            emitterCollider = this.GetComponent<Collider>();

            if (emitterCollider == null)       {
                Debug.LogWarning("FMOD Area Emitter could not detect a colliders on object. Will be disabled now.");
                this.GetComponent<FmodAreaEmitter>().enabled = !this.GetComponent<FmodAreaEmitter>().enabled;
            }
        }
    }

    private void Start()
    {
        AmbientEmitter = RuntimeManager.CreateInstance(fmodEvent);

        AudioDes = RuntimeManager.GetEventDescription(fmodEvent);
        AudioDes.getMinMaxDistance(out float minDistance, out maxDistance);         //As we don't need the minimal value and there is no function to get only max distance, minDistance is just declared here and never used

        FindPointForEmitter();

        UpdateEmitter3dPosition();
        AmbientEmitter.start();
    }

    private void FixedUpdate()
    {
        playerPosition = player.transform.position;
        FindPointForEmitter();
        if (distanceToPlayer > maxDistance || distanceToPlayer < 0)                 //When Player is too far, there is no point for further calculations
            return;

        UpdateEmitter3dPosition();
    }

    private void OnDestroy()        //To make sure we will stop this sound from playing after we change level we got to stop it and release, if disabling function copy that also to "OnDisable", and start sound on "OnEnable"
    {
        AmbientEmitter.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        AmbientEmitter.release();
    }
    #endregion

    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            isPlayerInside = false;
    }
    #endregion

    private void FindPointForEmitter()
    {
        if (isPlayerInside)         //When Player is inside, collider we want sounds to be played as 2D events - which is 3D one attached to the listener position
        {
            emitterPoint = playerPosition;
        }
        else                        //When we are outside, audio will behave like 3D source for which we shall provide a position that will be feeling more natural especially for large areas - the closest point between player and area
        {
            if (useChildColliders)
                GetDistanceToChildColliders();
            else
                GetDistanceToCollider();

            emitterPoint = closestPoint;
        }
    }

    private void GetDistanceToCollider()
    {
        closestPoint = emitterCollider.ClosestPoint(playerPosition);
        distanceToPlayer = Vector3.Distance(closestPoint, playerPosition);
    }

    private void GetDistanceToChildColliders()
    {
        float x = 100f;         //closest distance among checked colliders
        float y = 0f;           //distance of collider checked at the moment
        Vector3 newClosestPoint = closestPoint;

        foreach (Collider edge in emitersChildColliders)        //The loop that basically checks which of the child colliders is closest to the player
        {
            Vector3 newPoint = edge.ClosestPoint(playerPosition);
            y = Vector3.Distance(newPoint, playerPosition);

            if (x > y)      {
                newClosestPoint = newPoint;
                x = y;
            }
        }

        closestPoint = newClosestPoint;
        distanceToPlayer = x;
    }

    private void UpdateEmitter3dPosition()      
    {
        AmbientEmitter.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(emitterPoint)); 
    }

    private void OnDrawGizmos()                 //This part simply allows us to see the emitter point in Unity with Gizmos enabled
    {
        Gizmos.DrawSphere(emitterPoint, 0.5f);
    }
}
