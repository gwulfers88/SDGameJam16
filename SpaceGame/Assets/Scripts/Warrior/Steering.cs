using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Steering : Photon.MonoBehaviour
{
    #region Constants

    const float smallNumber = 0.0000001f;

    #endregion

    #region Debug

    //Turn on debugging
    public bool debug = false;

    #endregion

    #region Locomotion Variables

    public Vector3 velocity = new Vector3();
    public float mass = 1.0f;
    public float currentSpeed = 0;
    public float maxSpeed = 20;

    #endregion

    #region Game Objects

    //Game objects that we want to access
    public GameObject target;

    #endregion

    #region Flags

    //Flags turning on each behavior (faking high level using flags)
    public bool seek = false;
    public bool flee = false;
    public bool arrive = false;
    public bool pursuit = false;
    public bool evade = false;
    public bool wander = false;
    public bool avoidance = false;
    public bool hide = false;
    public bool pathFollow = false;
    public bool flocking = false;

    #endregion

    #region Weights

    //Weights for each behavior
    public float seekWeight = 1;
    public float fleeWeight = 1;
    public float arriveWeight = 1;
    public float pursuitWeight = 1;
    public float evadeWeight = 1;
    public float wanderWeight = 1;
    public float avoidanceWeight = 1;
    public float hideWeight = 1;
    public float pathFollowWeight = 1;
    public float separationWeight = 1;
    public float cohesionWeight = 1;
    public float alignmentWeight = 1;

    #endregion

    #region Arrive

    //Used for Arrive deceleration
    public enum Deceleration
    {
        Slow = 3, Normal = 2, Fast = 1
    };

    //Initialize deceleration to slow
    public Deceleration deceleration = Deceleration.Slow;

    //This value is required to provide fine tweaking of the deceleration.
    public float decelerationTweaker = 0.5f;

    #endregion

    #region Avoidance

    //Minimum distance to look for collisions
    public float minCollisionDistanceLength = 1;

    //Avoidance whisker angle
    public float whiskerAngle = 25;
    public float whiskerAngleFar = 45;

    //Avoidance whisker scale lengths
    public float whiskerScaleLengthForward = 0.5f;
    public float whiskerScaleLengthSide = 0.25f;

    #endregion

    #region Hide

    //Distance to hide behind rocks (far enough so agent isn't sitting in the rock, close enough to get there quickly)
    public float hideDistance = 5;

    //Distance from enemy from hide spot (don't want agent to go towards blocked hide spots)
    public float distanceFromHideSpot = 10;

    #endregion

    #region PathFollow

    //Path-Follow queue 
    public Queue<GameObject> path = new Queue<GameObject>();

    //Looped flag - when looped, agent will continue back to beginning and do it again
    public bool looped = false;

    //Minimum path arrived distance
    public float minPathArrivedDistance = 1;

    #endregion

    #region Wander

    //Wander settings
    public float wanderRadius = 10;
    public float wanderDistance = 15;
    public float wanderJitter = 5;

    //Target for wandering
    public Vector3 wanderTarget = new Vector3();

    #endregion

    #region Flocking

    //Viewing radius to determine flocking neighbors
    public float neighborhoodRadius = 15;

    //List of all neighbors
    List<GameObject> neighbors = new List<GameObject>();

    #endregion

    public void Locomotion()
    {
        Vector3 steeringForce = new Vector3(0, 0, 0);

        //Applly steering forces
        if (seek)
        {
            steeringForce += seekWeight * Seek();
        }

        if (flee)
        {
            steeringForce += fleeWeight * Flee();
        }

        if (arrive)
        {
            steeringForce += arriveWeight * Arrive();
        }

        if (pursuit)
        {
            steeringForce += pursuitWeight * Pursuit();
        }

        if (evade)
        {
            steeringForce += evadeWeight * Evade();
        }

        if (avoidance)
        {
            steeringForce += avoidanceWeight * Avoidance();
        }

        if (hide)
        {
            steeringForce += hideWeight * Hide();
        }

        if (pathFollow)
        {
            steeringForce += pathFollowWeight * PathFollow();
        }

        if (wander)
        {
            steeringForce += wanderWeight * Wander();
        }

        if (flocking)
        {
            steeringForce += Flocking();
        }

        //Movement
        Vector3 acceleration = steeringForce / mass;
        velocity += acceleration * Time.deltaTime;

        //Make sure vehicle does not exceed maximum velocity
        Truncate(ref velocity, maxSpeed);

        currentSpeed = velocity.magnitude;

        //transform.position += velocity * Time.deltaTime;
        GetComponent<Rigidbody>().velocity = velocity;

        if (currentSpeed > smallNumber)
        {
            transform.forward = velocity.normalized;
        }
    }

    /// <summary>
    /// Truncate a vector to a maximum force.
    /// </summary>
    /// <param name="vector">Vector.</param>
    /// <param name="max">Max.</param>
    void Truncate(ref Vector3 vector, float max)
    {
        //Is vector magnitude greater than max force?
        if (vector.magnitude > max)
        {
            //Set it equal to max force by normalizing (unit vector) * max force
            vector.Normalize();
            vector *= max;
        }
    }

    /// <summary>
    /// Clears steering behaviors.
    /// </summary>
    public void ClearBehaviors()
    {
        //Shut off all behaviors
        seek = false;
        flee = false;
        arrive = false;
        pursuit = false;
        evade = false;
        wander = false;
        avoidance = true;
        hide = false;
        pathFollow = false;
        flocking = false;
    }

    /// <summary>
    /// Stop the agent
    /// </summary>
    public void Stop()
    {
        //Set velocity to 0
        velocity = new Vector3();
    }

    /// <summary>
    /// Seek steering behavior, returns a steering force. Moves towards a target.
    /// </summary>
    public Vector3 Seek()
    {
        //Start off with nothing to seek toward, so no velocity change
        Vector3 seekVector = new Vector3();

        if (target != null)
        {
            //Find the vector directly to the target (Ptarget - Pcurrent)
            Vector3 vectorToTarget = target.transform.position - transform.position;

            //Calculate the desired velocity to the target (Vnorm * maxSpeed)
            Vector3 desiredVelocity = vectorToTarget.normalized * maxSpeed;

            //Subtract current velocity from desired velocity to get new heading toward target
            seekVector = (desiredVelocity - velocity);
        }

        return seekVector;
    }

    /// <summary>
    /// Flee steering behavior, returns a steering force. Moves away from a target.
    /// </summary>
    Vector3 Flee()
    {
        //Flee vector is the opposite of seek
        return Seek() * -1;
    }

    /// <summary>
    /// Arrive steering behavior, returns a steering force. Decelerates onto the target position.
    /// </summary>
    Vector3 Arrive()
    {
        Vector3 result = new Vector3();

        //Calculate the vector to the target
        Vector3 toTarget = target.transform.position - transform.position;

        //Calculate the distance to the target position
        float distance = toTarget.magnitude;

        if (distance > 0)
        {
            //Calculate the speed required to reach the target given the desired deceleration
            float speed = distance / ((float)deceleration * decelerationTweaker);

            //Make sure the velocity does not exceed the max
            speed = Mathf.Min(speed, maxSpeed);

            //Just like seek, but do not need to normalize toTarget (already computed distance)
            Vector3 desiredVelocity = toTarget * speed / distance;
            result = (desiredVelocity - velocity);
        }

        return result;
    }

    /// <summary>
    /// Pursuit steering behavior, returns a steering force. Chases a target and tries to cut them off.
    /// </summary>
    Vector3 Pursuit()
    {
        Vector3 result = new Vector3();

        //If the evader is ahead and facing the agent then we can just seek
        //for the evader's current position.
        Vector3 toEvader = target.transform.position - transform.position;

        double relativeHeading = Vector3.Dot(transform.forward, target.transform.forward);

        if ((Vector3.Dot(toEvader, transform.forward) > 0) &&
            (relativeHeading < -0.95)) //acos(0.95)=18 degs
        {
            //Target is ahead, seek to it
            result = Seek();
        }
        else
        {
            //Not ahead, so predict where the evader will be.
            //Look-ahead time is proportional to the distance between the evader
            //and the pursuer; it is inversely proportional to the sum of the agents' velocities
            Steering evader = target.GetComponent<Steering>();

            if (evader == null)
            {
                Debug.Log("not a steering agent");
                //Not a steering agent, cannot predict path, just return Seek()
                return Seek();
            }

            //Calculate look ahead time based on evaders current speed
            float lookAheadTime = toEvader.magnitude / (maxSpeed + evader.currentSpeed);

            //Now seek to the predicted future position of the evader
            Vector3 seekPosition = target.transform.position + evader.velocity * lookAheadTime;

            Vector3 vectorToTarget = seekPosition - transform.position;

            //Calculate the desired velocity to the target (Vnorm * maxSpeed)
            Vector3 desiredVelocity = vectorToTarget.normalized * maxSpeed;

            //Subtract current velocity from desired velocity to get new heading toward target
            result = (desiredVelocity - velocity);
        }

        return result;
    }

    /// <summary>
    /// Evade steering behavior, returns a steering force. Runs from a target, opposite of pursue.
    /// </summary>
    Vector3 Evade()
    {
        //Do not need to include the check for facing direction

        //Calculate vector to the pursuer
        Vector3 toPursuer = target.transform.position - transform.position;

        //Look-ahead time is proportional to the distance between the pursuer
        //and the evader; it is inversely proportional to the sum of the agents' velocities
        Steering pursuer = target.GetComponent<Steering>();

        if (pursuer == null)
        {
            //Not a steering agent, cannot predict path, just return Flee()
            return Flee();
        }

        //Calculate look ahead time baed on evaders current speed
        float lookAheadTime = toPursuer.magnitude / (maxSpeed + pursuer.currentSpeed);

        //Now flee away from predicted future position of the pursuer
        Vector3 fleePosition = target.transform.position + pursuer.velocity * lookAheadTime;

        //Calculate vector to the flee position
        Vector3 vectorToTarget = transform.position - fleePosition;

        //Calculate the desired velocity to the target (Vnorm * maxSpeed)
        Vector3 desiredVelocity = vectorToTarget.normalized * maxSpeed;

        //Subtract current velocity from desired velocity to get new heading toward target
        return (desiredVelocity - velocity);
    }

    /// <summary>
    /// Avoidance steering behavior, returns a steering force. Avoids obstacles (walls, objects, etc.)
    /// </summary>
    Vector3 Avoidance()
    {
        //Create the whiskers for avoidance
        Vector3 whiskerLeft = Quaternion.AngleAxis(-whiskerAngle, Vector3.up) * transform.forward;
        Vector3 whiskerRight = Quaternion.AngleAxis(whiskerAngle, Vector3.up) * transform.forward;
        Vector3 whiskerFarLeft = Quaternion.AngleAxis(-whiskerAngleFar, Vector3.up) * transform.forward;
        Vector3 whiskerFarRight = Quaternion.AngleAxis(whiskerAngleFar, Vector3.up) * transform.forward;

        //Calculate the forces for each whisker, including the one straight ahead
        return RayAvoidance(transform.forward, whiskerScaleLengthForward, 1) +
               RayAvoidance(whiskerLeft, whiskerScaleLengthSide, -1) +
               RayAvoidance(whiskerRight, whiskerScaleLengthSide, 1) +
               RayAvoidance(whiskerFarLeft, whiskerScaleLengthSide, -1) +
               RayAvoidance(whiskerFarRight, whiskerScaleLengthSide, 1);
    }

    /// <summary>
    /// A single ray looking for objects that need to be avoided. Returns a steering force.
    /// </summary>
    /// <returns>The avoidance.</returns>
    /// <param name="direction">Direction.</param>
    /// <param name="scaleDistance">Scale distance.</param>
    Vector3 RayAvoidance(Vector3 direction, float scaleDistance, float sideFactor)
    {
        Vector3 steeringForce = new Vector3();

        //Calculate distance of the ray - scale it based on how fast agent is moving
        float distance = minCollisionDistanceLength + velocity.magnitude * scaleDistance;

        //Use raycast to determine if a collision is coming up
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            //Going to hit, calculate by how much agent is going to hit the target
            Vector3 diff = (hit.point - transform.position);
            float curDistance = diff.magnitude;

            //Avoid rocks - may need to avoid everything, or specific things
            if ((hit.collider.tag == "Rock") || (hit.collider.tag == "Ant"))
            {
                if (debug)
                {
                    //Whisker ray - going to hit
                    Debug.DrawRay(transform.position, direction * distance, Color.cyan);
                }

                //Determine which side of the object the agent is on
                float testSide = Vector3.Dot(hit.normal, transform.forward);

                //Compute the angle to use to create the lateral vector
                //sideFactor is used to flip the side due to the issues with the right whiskers
                float angle = testSide >= 0 ? -90 * sideFactor : 90 * sideFactor;

                //Create the lateral vector
                Quaternion rotateVector = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 lateralVector = rotateVector * hit.normal;

                steeringForce += velocity.magnitude * lateralVector / curDistance;
                //Debug.DrawRay(hit.point, steeringForce, Color.red);
            }
            else if (hit.collider.tag == "Wall")
            {
                if (debug)
                {
                    //Whisker ray - going to hit
                    Debug.DrawRay(transform.position, direction * distance, Color.cyan);
                }

                //Calculate steering force away from the object, and scale it by distance to target
                //The closer to target, the greater the steering force
                steeringForce += velocity.magnitude * hit.normal / curDistance;
                //Debug.DrawRay(hit.point, steeringForce, Color.white);
            }
            else
            {
                if (debug)
                {
                    //Whisker ray - not going to hit
                    Debug.DrawRay(transform.position, direction * distance, Color.blue);
                }
            }
        }
        else
        {
            if (debug)
            {
                //Whisker ray - not going to hit
                Debug.DrawRay(transform.position, direction * distance, Color.blue);
            }
        }

        return steeringForce;
    }

    /// <summary>
    /// Hide steering behavior, returns a steering force. Hides from an enemy behind an obstacle.
    /// </summary>
    Vector3 Hide()
    {
        Vector3 hideSpot = FindHideSpot();

        //Basic seek toward a position 
        Vector3 vectorToTarget = hideSpot - transform.position;
        Vector3 desiredVelocity = vectorToTarget.normalized * maxSpeed;
        return (desiredVelocity - this.velocity);
    }

    /// <summary>
    /// Finds a hide spot location for the hide steering behavior, returns a position vector.
    /// </summary>
    /// <returns>The hide spot.</returns>
    Vector3 FindHideSpot()
    {
        Vector3 hideSpot = new Vector3();

        if (target != null)
        {
            //Find all rocks to hide behind
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Rock");

            //Set distance to infinity, so first rock we look at is closest
            float distance = Mathf.Infinity;

            //Look for nearest rock
            foreach (GameObject obj in objs)
            {
                //Calculate direction to the rock
                Vector3 directionToObject = obj.transform.position - target.transform.position;

                //Calculate distance to the hide spot behind the rock - hidedisance used instead of object radius
                float distanceToObject = directionToObject.magnitude + hideDistance;

                //Create a direction vector to the hidespot behind the rock
                directionToObject = directionToObject.normalized * distanceToObject;

                //Ray to fleeTarget
                //Debug.DrawRay(fleeTarget.transform.position, directionToObject, Color.green);

                //Calculate the hiding spot position (agent position + direction to hide spot)
                Vector3 hidingSpotPos = target.transform.position + directionToObject;

                //Ray from enemy to hiding spot
                //Debug.DrawLine(fleeTarget.transform.position, hidingSpotPos);

                //Calculate a vector to the hiding position
                Vector3 vecToSpot = hidingSpotPos - transform.position;

                //Calculate distance to see if this hiding spot is the closest hiding spot
                //Using sqrMagnitude to check distance - faster as it doesn't perform a sqaure root
                //Good method to use when comparing relative distances
                //Also check to make sure this hiding spot is a minimum distance away from the enemy, or agent may not be fleeing far enough
                if ((vecToSpot.sqrMagnitude < distance) &&
                    (Vector3.Distance(hidingSpotPos, target.transform.position) > distanceFromHideSpot))
                {
                    //This is the closest hiding spot, make this the actual hiding spot
                    hideSpot = hidingSpotPos;

                    //Save distance to compare against next hiding spot
                    distance = vecToSpot.sqrMagnitude;
                }

                //Ray from enemy to selected hiding spot
                if (debug)
                {
                    Debug.DrawLine(target.transform.position, hidingSpotPos, Color.magenta);
                }
            }

            //Ray from enemy to selected hiding spot
            if (debug)
            {
                Debug.DrawLine(target.transform.position, hideSpot, Color.green);
            }
        }
        else
        {
            //Where's my fleeTarget?
        }

        return hideSpot;
    }

    /// <summary>
    /// Path follow steering behavior, returns a steering force. Follows a path (set of waypoints).
    /// </summary>
    /// <returns>The follow.</returns>
    Vector3 PathFollow()
    {
        Vector3 result = new Vector3();

        //Is there a path?
        if (path.Count == 0)
        {
            //No path left
            pathFollow = false;

            //No path left, not looping, stop the agent
            Stop();

            //No where to move
            return result;
        }

        //Current path node
        GameObject node = path.Peek();

        //Calculate distance to current path node
        float distance = Vector3.Distance(path.Peek().transform.position, transform.position);

        //Check to see agent is close to current path node
        if (distance <= minPathArrivedDistance)
        {
            //Move to next node, if there is one
            node = path.Dequeue();

            if (looped)
            {
                //Looped path, add node to end of queue
                path.Enqueue(node);
            }
        }

        //Seek/Arrive to the node
        target = node;

        //More waypoints to follow?
        if (path.Count > 0)
        {
            //Seek towards current path node
            result = Seek();

            //Arrive could work as well to cut down on turn radius if agent overshoots
            //deceleration = Deceleration.Normal;
            //result =  Arrive();
        }
        else
        {
            //No more nodes, arrive at the destination
            deceleration = Deceleration.Fast;
            result = Arrive();
        }

        return result;
    }

    /// <summary>
    /// Returns a random double in the range -1 < n < 1
    /// </summary>
    float RandomClamped()
    {
        return Random.value - Random.value;
    }

    /// <summary>
    /// Wander steering behavior, returns a steering force. Wanders around, randomly picking a direction around a projected circle.
    /// </summary>
    Vector3 Wander()
    {
        //Add a small random vector to the target’s position
        wanderTarget += new Vector3(RandomClamped() * wanderJitter, 0,
                                    RandomClamped() * wanderJitter);

        //Reproject this new vector back onto a unit circle
        wanderTarget.Normalize();

        //Increase the length of the vector to the same as the radius of the wander circle
        wanderTarget *= wanderRadius;

        //Move the target into a position wanderDist in front of the agent
        Vector3 targetWorld = (wanderDistance * transform.forward) + wanderTarget + transform.position;

        if (debug)
        {
            //Line that shows where the wander target is
            Debug.DrawLine(transform.position, targetWorld);
        }

        //Steer toward it
        return targetWorld - transform.position;
    }

    /// <summary>
    /// Looks for all neighbors of the same tag within a neighborhood radius.
    /// </summary>
    void TagNeighbors()
    {
        //Clear neighbor list
        neighbors.Clear();

        //Show radius
        //Debug.DrawRay(transform.position, transform.forward*neighborhoodRadius, Color.green, 0);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, neighborhoodRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.tag == tag)
            {
                neighbors.Add(hit.gameObject);
            }
        }

        return;



        //Get all similar objects to create flock
        GameObject[] flock = GameObject.FindGameObjectsWithTag(tag);

        //Loop through each member of flock to find all neighbors
        foreach (GameObject item in flock)
        {
            //Do not include self when checking neighbors
            if (GetInstanceID() != item.GetInstanceID())
            {
                //Get the distance to other flock-mate
                float distance = Vector3.Distance(transform.position, item.transform.position);

                //Is flock-mate a neighbor?
                if (distance < neighborhoodRadius)
                {
                    //Yes, neighbor, save for later
                    neighbors.Add(item);
                }
            }
        }
    }

    /// <summary>
    /// Flocking steering behavior, returns a steering force. Finds all neighbors, and then computes
    /// separation, cohesion, and alignment steering behaviors.
    /// </summary>
    Vector3 Flocking()
    {
        //Neighbors used for all flocking functions, only calculate once
        TagNeighbors();

        //Calculate all steering forces for the flocking function
        return separationWeight * Separation() +
               cohesionWeight * Cohesion() +
               alignmentWeight * Alignment();
    }

    /// <summary>
    /// Separation steering behavior, returns a steering force. Creates a force away from neighbors.
    /// </summary>
    Vector3 Separation()
    {
        Vector3 steeringForce = new Vector3();

        //Iterate through the neighbors and calculate separation force
        foreach (GameObject item in neighbors)
        {
            //Calculate vector to neighbor
            Vector3 toAgent = transform.position - item.transform.position;

            //Calculate distance to neighbor
            float length = toAgent.magnitude;

            //Avoid divide by zero errors
            if (length > smallNumber)
            {
                //Scale the force inversely proportional to the agent's distance from its neighbor.
                steeringForce += toAgent.normalized / length;
            }
            else
            {
                //Agent is on top of neighbor, just get it moving
                steeringForce += transform.forward;
            }
        }

        return steeringForce;
    }

    /// <summary>
    /// Cohesion steering behavior, returns a steering force. Creates a force toward neighbors.
    /// </summary>
    Vector3 Cohesion()
    {
        //Find the center of mass of all the agents
        Vector3 centerOfMass = new Vector3();
        Vector3 steeringForce = new Vector3();

        //Iterate through the neighbors and sum up all the position vectors
        foreach (GameObject item in neighbors)
        {
            //Center of mass is the sum of all positions of the neighbors
            centerOfMass += item.transform.position;
        }

        if (neighbors.Count > 0)
        {
            //Center of mass is the average of the sum of positions
            centerOfMass /= neighbors.Count;

            //Seek toward centerOfMass
            Vector3 vectorToTarget = centerOfMass - transform.position;

            //Calculate the desired velocity to the target (Vnorm * maxSpeed)
            Vector3 desiredVelocity = vectorToTarget.normalized * maxSpeed;

            //Subtract current velocity from desired velocity to get new heading toward target
            steeringForce = (desiredVelocity - velocity);
        }

        return steeringForce;
    }

    /// <summary>
    /// Alignment steering behavior, returns a steering force. Creates a force in the same direction as neighbors.
    /// </summary>
    Vector3 Alignment()
    {
        //Average heading of the neighbors
        Vector3 averageHeading = new Vector3();

        //Iterate through all the neighbors and sum their heading vectors
        foreach (GameObject item in neighbors)
        {
            averageHeading += item.transform.forward;
        }

        //Average their heading vectors if neighbor count > 0
        if (neighbors.Count > 0)
        {
            averageHeading /= neighbors.Count;

            //Calculate steering force by subtracting from agent's forward direction
            averageHeading -= transform.forward;
        }

        return averageHeading;
    }
}

