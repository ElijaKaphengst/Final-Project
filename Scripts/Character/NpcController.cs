using Character.NPC.PathSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Character.NPC
{
    /// <summary> Controlls the movement of an NPC. </summary>
    public class NpcController : Character
    {
        #region Variables/BaseMethods

        /// <summary> The Time the NPC should wait when he hits a waypoint. </summary>
        public float waitTime = 1f;
        /// <summary> The minimum range of the stoppingDistance of the NPC. </summary>
        public float minRange = 1f;
        /// <summary> The maximum range of the stoppingDistance of the NPC. </summary>
        public float maxRange = 5f;
        /// <summary> How the NPC should walk. </summary>
        public WalkType walkType;
        /// <summary> True if the player has a 'RandomAreaControll'. </summary>
        public bool hasArea;
        /// <summary> The path on wich the NPC moves. </summary>
        public Path path;
        /// <summary> The Area in wich the NPC moves. </summary>
        [HideInInspector]
        public RandomAreaControll areaControll;

        /// <summary> The current time that the NPC waited until the next time he moves. </summary>
        private float timer;
        /// <summary> The index of the current Waypoint that's the target of the NPC. </summary>
        private int waypointInd;
        /// <summary> The target position of the NPC. </summary>
        private Vector3 target;
        /// <summary> Reference of the 'NpcInteractions' of the NPC. </summary>
        private NpcInteractions interactions;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            interactions = GetComponent<NpcInteractions>();

            //Check if the NPC hasn't an NavMeshAgent and if it does call the 'Idle' method and return.
            if (agent == null)
            {
                Idle();
                return;
            }
            
            //Set the speed of the NPC.
            SetSpeed(Speed);
            //Clamp the stopping distance of the NavMeshAgent to the minimum and the maximum range values. 
            agent.stoppingDistance = Mathf.Clamp(agent.stoppingDistance, minRange, maxRange);
        }

        private void Update()
        {
            //Set the timer
            SetTimer();
        }

        private void FixedUpdate()
        {
            //Check if the player is in interaction range and if it's call the 'Idle' method
            if (interactions.playerInRange)
                Idle(); //If it's not then check if the NPC is near his target position and if it's call the 'BeginMove' method 
            else if (Vector3.Distance(transform.position, agent.destination) < agent.stoppingDistance && !interactions.playerInRange)
                BeginMove();
        }

        #endregion

        #region MainCharMethods

        #region BeginMove

        /// <summary> Begins to move the NPC. </summary>
        void BeginMove()
        {
            //Check how the NPC should move.
            switch (walkType)
            {
                //If it doesn't should move call the 'Idle' method.
                case WalkType.Idle:
                    Idle();
                    break;
                //If it should move along a path the check if the Timer reaches the wait time and then call the 'WalkAlongPath' method.
                case WalkType.Path:
                    if (timer >= waitTime - .05f)
                        WalkAlongPath();
                    break;
                //If it should walk random inside an area then call the 'WalkRandom' method.
                case WalkType.Random:
                    WalkRandom();
                    break;
                //If it should Follow a target
                case WalkType.Follow:
                    {
                        //Check if there's a target to follow and if it isn't then call the 'Idle' method.
                        if (targetCharacter == null)
                            Idle(); //Check if the NPC reaches the target and if it does then call the 'Idle' method.
                        else if (Vector3.Distance(targetCharacter.transform.position, transform.position) < maxRange)
                            Idle(); //Check if the NPC doesn't reaches the target and then call the 'FollowCharacter' metho
                        else if (Vector3.Distance(transform.position, targetCharacter.transform.position) > maxRange || Vector3.Distance(target, targetCharacter.transform.position) > maxRange)
                            FollowCharacter(targetCharacter);
                    }
                    break;
            }
        }

        #endregion

        #region Overrides

        /// <summary> Overrides the 'Move' method to move the NPC. </summary>
        /// <param name="targetPosition"> The position to wich the NPC will move. </param>
        public override void Move(Vector3 targetPosition)
        {
            //Set the 'isStopped' bool to false so the NavMeshAgent can move.
            agent.isStopped = false;
            //Set the destination of the NavMeshAgent to the 'targetPosition'.
            agent.SetDestination(targetPosition);
        }

        /// <summary> Overrides the 'SetSpeed' method. </summary>
        /// <param name="speed"> How fast the NPC will move. </param>
        public override void SetSpeed(float speed)
        {
            //Sets the speed value of the NPC.
            base.SetSpeed(speed);
            //Sets the speed value of the NavMeshAgent.
            agent.speed = speed;
        }

        #endregion

        #region Timer

        /// <summary> Sets the timer if the NPC reaches his target. </summary>
        void SetTimer()
        {
            //Check if the NPC reaches his target. If not set the timer to 0.
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                //Set the timer to the time he's near to it.
                timer += Time.deltaTime;

                //Check if the timer reaches the wait time and then set the timer to 0.
                if (timer > waitTime)
                    timer = 0;
            }
            else
                timer = 0;
        }

        #endregion

        #region ChangeWalkType

        /// <summary> Changes how the NPC will walk. </summary>
        /// <param name="_walkType"> How the NPC should walk. </param>
        public void ChangeWalkType(WalkType _walkType)
        {
            //Sets the 'walkType'
            walkType = _walkType;
        }

        #endregion

        #endregion

        #region MoveMethods

        #region Idle
        /// <summary> Does Idle things. </summary>
        void Idle()
        {
            //Check if the NPC has an NavMeshAgent if it doesn't have one return.
            if (agent == null)
                return;

            //Stops the NavMeshAgent,
            agent.isStopped = true;
            //Set the destination to the position of the NPC.
            agent.SetDestination(transform.position);
        }

        #endregion

        #region Path

        /// <summary> Lets the NPC walk along the path assigned to is. </summary>
        void WalkAlongPath()
        {
            //Check if there is no path attached to the NPC.
            if (path == null || path.WayPoints.Length <= 0)
            {
                //Call the 'Idle' method.
                Idle();
                return;
            }

            //Get the target position from the Path.
            target = path.WalkAlongPath(ref waypointInd);
            //Move to the target position.
            Move(target);
        }

        #endregion

        #region Random

        /// <summary> Lets the NPC walk inside an area. </summary>
        void WalkRandom()
        {
            //Check if the NPC has an area.
            if (hasArea || areaControll != null)
            {
                //Move to a random position inside of the area.
                Move(areaControll.FindRandomPos(this));
                return;
            }   //Check if the NPC does have an area but the 'hasArea' bool is false.
            else if (areaControll != null && !hasArea)
            {
                //Call the 'Idle' method.
                Idle();
                //Check if the NPC is inside of the area and if it's set the 'hasArea' bool to true.
                hasArea = areaControll.IsInside(transform);
                return;
            }   //Check if the NPC doesn't have an area and the 'hasArea' bool is false.
            else if (!hasArea && areaControll == null)
            {
                //Call the 'Idle' method.
                Idle();
                //Find the nearest area from the NPCs position.
                AIMaster.instance.GetNearestArea(this);
            }
        }

        #endregion

        #region Follow
        /// <summary> Lets the NPC follow a character. </summary>
        /// <param name="character"> The character wich the NPC will follow. </param>
        void FollowCharacter(Character character)
        {
            //Set the 'targetCharacter' to the 'character'.
            targetCharacter = character;

            //Check if the NPC is near to the target character and if it's call the 'Idle' method.
            if (Vector3.Distance(target, character.transform.position) < maxRange)
                Idle();

            //Set the target position a random position inside of a sphere at the target character.
            target = character.transform.position + Random.insideUnitSphere * Random.Range(minRange, maxRange);

            NavMeshHit hit;
            //Find the closest point from the target position on the NavMesh.
            if (NavMesh.SamplePosition(target, out hit, 5, NavMesh.AllAreas))
                target = hit.position;

            //Move the NPC to the target position.
            Move(target);
        }
        #endregion

        #endregion
    }

    /// <summary> How the NPC should walk. </summary>
    public enum WalkType
    {
        /// <summary> Don't move and play's the Idle animation. </summary>
        Idle,
        /// <summary> Walks along a path. </summary>
        Path,
        /// <summary> Walks random inside of an area. </summary>
        Random,
        /// <summary> Follows the player. </summary>
        Follow
    }
}