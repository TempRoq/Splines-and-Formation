using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BPTraverser : MonoBehaviour
{
    //GOAL: BP Traverser - a component that goes on an object that follows a Bezier Path, which then gives movement data to its followers.

    private Rigidbody2D rb2d;

    private BezierPath path;
    private List<BPTraverser> childTraversers = new();
    private float speedMultiplier;
    private Vector2 scale;
    private Vector2 locationOffset;
    private float t;
    private float offsetAngle;
    private Tweening.TweenType tweenMethod;

    private bool finishedPath = false;
    private bool hasParent = false;

    private Vector2 localLocationAtOffset = Vector2.zero;


    public void AddChild(BPTraverser bpt)
    {
        childTraversers.Add(bpt);
    }
    public void ClearChildren()
    {
        childTraversers.Clear();
    }
    public void StartFollowingPath(BezierPath _path, bool _hasParent, Vector2 _scale, Vector2 _locationOffset, float initial_t, float _speedMultiplier, float _offsetAngle) //Gas Gas Gas
    {
        finishedPath = false;
        t = initial_t;
        scale = _scale;
        speedMultiplier = _speedMultiplier;
        locationOffset = _locationOffset;
        path = _path;
        offsetAngle = _offsetAngle;
        hasParent = _hasParent;
        tweenMethod = path.Nodes[0].tween;
        localLocationAtOffset = (Vector2)path.LocationAtTime(initial_t, tweenMethod) + locationOffset;

    }
    public void StartFollowingPath(BezierPath _path, BPTraverser _parentBP, Vector3 _origin, Vector2 _scale, Vector2 _locationOffset, float initial_t, float _speedMultiplier, float _offsetAngle) //Gas Gas Gas
    {
        finishedPath = false;
        t = initial_t;
        scale = _scale;
        speedMultiplier = _speedMultiplier;
        locationOffset = _locationOffset;
        path = _path;
        offsetAngle = _offsetAngle;
        hasParent = true;
        tweenMethod = path.Nodes[0].tween;
        localLocationAtOffset = (Vector2)path.LocationAtTime(initial_t, tweenMethod) + locationOffset;
    }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        if (hasParent)
        {
            return;
        }
            ChangeCurrentVelocity(Vector2.zero);
           
    }

    
    private void ChangeCurrentVelocity(Vector2 parentVelocity)
    {
        Vector2 speed = rb2d.velocity;
        if (!finishedPath)
        {
            //Find the velocity at a given offset by finding the location on the curve at time T, finding and normalizing the velocity at time t to get the new forward vector,
            //using it to find the new right vector, and using that to apply offsets to get the new location with a given offset at time T. Then subtract that from the prior
            //local position in order to get the new base velocity.
            Vector2 priorLocalLoc = localLocationAtOffset;
            Vector2 newForward = path.VelocityAtTime(t, tweenMethod).normalized;
            
            localLocationAtOffset = (Vector2)path.LocationAtTime(t, tweenMethod) + (newForward * locationOffset.y) + (-Vector2.Perpendicular(newForward) * locationOffset.x);

            //Offsets the small changes in T being dependent on time.fixeddeltatime
            Vector2 newVel = (localLocationAtOffset - priorLocalLoc) / Time.fixedDeltaTime;

            

            //Apply local rotation by getting the angle from the horizontal formed by the vector, adding the offset
            //turning it back into a radian to get the unit vector of the new direciton. Then multiply it by the local velocity's magnitude.

            Vector3 normDirection = newVel.normalized;
            float theta = (Mathf.Atan2(normDirection.y, normDirection.x) * Mathf.Rad2Deg + offsetAngle) * Mathf.Deg2Rad;
            newVel = Vector2.Scale(new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * newVel.magnitude, scale); //Also apply scaling
            //Add the current velocity of the parent in order to account for it moving. Then pass the velocity to its children so they may do the same.
            speed = newVel + parentVelocity;
            rb2d.velocity = speed;
            t += Time.fixedDeltaTime * (speedMultiplier * path.Nodes[Mathf.FloorToInt(t)].speedMultiplier);
            if (t >= path.Nodes.Length)
            {
                finishedPath = true;
            }
            else
            {
                tweenMethod = path.Nodes[Mathf.Clamp(Mathf.FloorToInt(t), 0, path.Nodes.Length - 1)].tween;
            }

        }

        for (int i = 0; i < childTraversers.Count; i++)
        {
            Debug.Log("Sending to CHild!");
            childTraversers[i].ChangeCurrentVelocity(speed);
            
        }




    }

    /* Old ChangeCurrentVelocity() Function JIC
    public void ChangeCurrentVelocity()
    {

        if (t < path.Nodes.Length)
        {

          



            Vector3 nextPos = path.LocationAtTime(t) + (useTransform ? transformToFollow.position : origin); //Calculates the next position on the curve that the path follower will be at
            Vector3 pathForward = path.VelocityAtTime(t).normalized; //Calculates the direction considered facing forward
            Vector3 pathPerpen = -Vector2.Perpendicular(pathForward); // Calculates the direction "right"
                                                                      //              finds velocity, accounting for offsets in the path.
            Vector3 baseVelocity = nextPos + (speedMultiplier * Time.deltaTime * ((pathForward * locationOffset.y) + (pathPerpen * locationOffset.x))) - transform.position;
            Vector3 normDirection = baseVelocity.normalized;
            float velMag = baseVelocity.magnitude;

            float theta = (Mathf.Atan2(normDirection.y, normDirection.x) * Mathf.Rad2Deg + offsetAngle) * Mathf.Deg2Rad;
            rb2d.velocity = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * velMag;

            t += (Time.fixedDeltaTime * speedMultiplier);
        }
       
    }
    */

}


