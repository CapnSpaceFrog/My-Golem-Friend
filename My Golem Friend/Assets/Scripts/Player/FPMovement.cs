using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * From where we last left off on 1/25/2022:
 * 
 * Need to rework how we read our movement input. Possibly break up the Vectors into two seperate inputs, or just convert the 0.7/0.7 into an enum
 * type that we can then feed into a switch statement for proper functionality (the more in-depth, the better, since we'll have more refined control)
 * 
 * Once the direction is fixed, our slope interactions will be functional and we can clean up this script before moving on to the InterObjects. InterObjs will require
 * us to implement the dialogue handler bones and we can start to mess around with serializing some string structs to feed into the handler via JSON. We can hold off
 * on finishing the node editor as we have plenty of time before our designers will need to get into the engine.
 * 
 */

    public enum MovementDirection
    {
        Forward,
        Backward,
        Left,
        Right,
        ForwardRight,
        ForwardLeft,
        BackwardRight,
        BackwardLeft,
        None
    }

    //Allow certain logic if no input is being received
    public enum IsMoving
    {
        Yes,
        No
    }

    public struct DirectionData
    {
        public MovementDirection direction;
        public Vector3 vector;
        public bool moveable;
    }

    public class FPMovement : MonoBehaviour
    {
    /*
     * A large majority of these variables can be moved closer to where they are used instead of being placed up here.
     * Additionally, this entire script needs to be re-written to better support sloped walking. Additionally we will
     * add jumping, crouching, maybe sliding? and gliding. Maybe even wall running if im feeling CRAZY
     */
        public LayerMask groundMask;

        public float maxSlopeAngle;
        public float minSlopeAngle;

        private MovementDirection direction;
        private IsMoving moving;

        private bool isGrounded;

        private float groundCastOffset;

        private Transform characterBody;

        private float yOffset;

        public int defaultGravity;
        public int fallingGravity;

        public float groundRayDistance;

        public float MaxSpeed = 200;

        private float OnMoveTimer;
        private float OnStopTimer;

        public float timeToMaxSpeed = 0.45f;

        public float timeToStop = 0.25f;

        private float SpeedScalar;

        private Vector3 velocity;
        private Vector3 lerpSpeed;

        private Transform m_CamTransform;

        private DirectionData[] directionData;

        private Vector3[] defaultStaticVectors;

        //DEBUG VARIABLES
        private Vector3 offsetPosition;

        public void Awake()
        {
            m_CamTransform = Camera.main.transform;

            characterBody = FPCameraController.CharacterBodyTransform;

            RetrievePlayerColliderInfo();

            directionData = new DirectionData[8];
        }

        private void RetrievePlayerColliderInfo()
        {
            groundCastOffset = Player.Collider.bounds.extents.x;

            yOffset = Player.Collider.bounds.extents.y / 2;
        }

        public void Update()
        {
            IsGrounded();

            ProcessPlayerInput();

            //CheckGroundNormals();

            //CheckMovement();

            //SpeedUpTimer();

            //SpeedScalar = Mathf.Clamp(OnMoveTimer / timeToMaxSpeed, 0, 1);

            //SpeedScalar = EaseOutQuint(SpeedScalar);
        }

    private void FixedUpdate()
    {
        CheckGroundNormals();
    }



    private void IsGrounded()
        {
            Vector3 position = new Vector3(transform.position.x, transform.position.y - yOffset, transform.position.z);

            RaycastHit hit;

            if (Physics.Raycast(position, Vector3.down, out hit, groundRayDistance, groundMask))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            //This is a temporary solution until i implement an initial line cast solution
            CalculateVectorDirections(hit);
        }

        private void CalculateVectorDirections(RaycastHit hit)
        {
            //Halfway point between vectors: direction =  halfWayVector = (Vector3.up + Vector3.right).normalized;
            defaultStaticVectors = CalculateGlobalVectorDirections(hit);

            Vector3[] localDirectionVectors = CalculateLocalVectorDirections(hit);

            for (int i = 0; i < directionData.Length; i++)
            {
                directionData[i].vector = localDirectionVectors[i];
                directionData[i].direction = (MovementDirection)i;
            }
        }

        private Vector3[] CalculateGlobalVectorDirections(RaycastHit hit)
        {
            Vector3 staticForward = Vector3.Cross(m_CamTransform.right, Vector3.up).normalized;

            Vector3 staticBackward = -staticForward;

            Vector3 staticRight = Vector3.Cross(staticForward, Vector3.up).normalized;

            Vector3 staticLeft = -staticRight;

            Vector3 staticForwardRight = (staticForward + staticRight).normalized;

            Vector3 staticForwardLeft = (staticForward + staticLeft).normalized;

            Vector3 staticBackwardRight = (staticBackward + staticRight).normalized;

            Vector3 staticBackwardLeft = (staticBackward + staticLeft).normalized;

            return new Vector3[] {staticForward,
                staticBackward,
                staticLeft,
                staticRight,
                staticForwardRight,
                staticForwardLeft,
                staticBackwardRight,
                staticBackwardLeft
            };
        }

        private Vector3[] CalculateLocalVectorDirections(RaycastHit hit)
        {
            Vector3 forward = Vector3.Cross(m_CamTransform.right, hit.normal).normalized;

            Vector3 backward = -forward;

            Vector3 right = Vector3.Cross(hit.normal, forward).normalized;

            Vector3 left = -right;

            Vector3 forwardRight = (forward + right).normalized;

            Vector3 forwardLeft = (forward + left).normalized;

            Vector3 backwardRight = (backward + right).normalized;

            Vector3 backwardLeft = (backward + left).normalized;

            return new Vector3[] {forward,
                backward,
                left,
                right,
                forwardRight,
                forwardLeft,
                backwardRight,
                backwardLeft
            };
        }

        private void ProcessPlayerInput()
        {
            Vector2 inputDirection = InputHandler.MovementInput;

            if (inputDirection == Vector2.zero)
            {
                direction = MovementDirection.None;
                moving = IsMoving.No;

                return;
            }
            else
            {
                //Setting direction to none for later check
                direction = MovementDirection.None;
                moving = IsMoving.Yes;
            }

            switch (inputDirection)
            {
                case Vector2 v when v.Equals(Vector2.up):
                    direction = MovementDirection.Forward;
                    break;

                case Vector2 v when v.Equals(Vector2.down):
                    direction = MovementDirection.Backward;
                    break;

                case Vector2 v when v.Equals(Vector2.right):
                    direction = MovementDirection.Right;
                    break;

                case Vector2 v when v.Equals(Vector2.left):
                    direction = MovementDirection.Left;
                    break;
            }
            
            if (direction == MovementDirection.None)
            {
                if (inputDirection.x > 0)
                {
                    if (inputDirection.y > 0)
                    {
                        direction = MovementDirection.ForwardRight;
                    }
                    else
                    {
                        direction = MovementDirection.BackwardRight;
                    }
                }
                else
                {
                    if (inputDirection.y > 0)
                    {
                        direction = MovementDirection.ForwardLeft;
                    }
                    else
                    {
                        direction = MovementDirection.BackwardLeft;
                    }
                }
            }
        }

        //Raycasting in the direction our Player wants to move and retrieving the ground normal
        private void CheckGroundNormals()
        {
            offsetPosition = new Vector3(characterBody.position.x, transform.position.y - yOffset, characterBody.position.z);

            //Perform all raycasts, hitting the ground at each of the eight points
            for (int i = 0; i < directionData.Length; i++)
            {
                //targetpoisition + (vector direction * radius of collider)
                RaycastHit hit;
                Physics.Raycast(offsetPosition + (directionData[i].vector * groundCastOffset), Vector3.down, out hit, groundRayDistance, groundMask);

            //Calculate whether the movement direction is valid
            //& prevent the Player from moving in such direction
            //Calcualte the angles from a fixed axis (PlayerBody transform)

            //Need to dynamically check each direction against a default direction vector for the proper angle

            //if ((Vector3.Angle(defaultStaticVectors[i], hit.normal) - 90f) > maxSlopeAngle
            //    || (Vector3.Angle(hit.normal, defaultStaticVectors[i]) - 90f) < minSlopeAngle)
            //{
            //    directionData[i].moveable = false;
            //    //Do something
            //}
            //else
            //{
            //    directionData[i].moveable = true;
            //}

            directionData[i].moveable = true;
        }

            CheckMovement();
        }

        private void CheckMovement()
        {
            switch (direction)
            {
                case MovementDirection.Forward:
                    if (directionData[0].moveable)
                    {
                        Move(Player.RB, directionData[0].vector);
                    }
                    break;

                case MovementDirection.Backward:
                    if (directionData[1].moveable)
                    {
                        Move(Player.RB, directionData[1].vector);
                    }
                    break;

                case MovementDirection.Left:
                    if (directionData[2].moveable)
                    {
                        Move(Player.RB, directionData[2].vector);
                    }
                    break;

                case MovementDirection.Right:
                    if (directionData[3].moveable)
                    {
                        Move(Player.RB, directionData[3].vector);
                    }
                    break;

                case MovementDirection.ForwardRight:
                    if (directionData[4].moveable)
                    {
                        Move(Player.RB, directionData[4].vector);
                    }
                    break;

                case MovementDirection.ForwardLeft:
                    if (directionData[5].moveable)
                    {
                        Move(Player.RB, directionData[5].vector);
                    }
                    break;

                case MovementDirection.BackwardRight:
                    if (directionData[6].moveable)
                    {
                        Move(Player.RB, directionData[6].vector);
                    }
                    break;

                case MovementDirection.BackwardLeft:
                    if (directionData[7].moveable)
                    {
                        Move(Player.RB, directionData[7].vector);
                    }
                    break;

                case MovementDirection.None:
                    //purely debug purposes
                    Move(Player.RB, Vector3.zero);
                    return;
            }
        }

        public void Move(Rigidbody RB, Vector3 direction)
        {
            if (moving == IsMoving.Yes)
            {
                //lerpSpeed = new Vector3(Mathf.Lerp(RB.velocity.x, MaxSpeed, 1), Mathf.Lerp(RB.velocity.y, MaxSpeed, 1), Mathf.Lerp(RB.velocity.z, MaxSpeed, 1));

                velocity = direction * MaxSpeed;

                velocity *= Time.deltaTime;

                RB.velocity = velocity;

                //Update this later to actually provide some functionality
                //ClampVelocity(RB);
            }
            else
            {
                RB.velocity = Vector3.zero;
            }
        }

        private void ValidatePosition(RaycastHit hit)
        {
            if (isGrounded && hit.normal != Vector3.up && direction == MovementDirection.None)
            {
                Physics.gravity = Vector3.zero;
            }
            else if (!isGrounded && hit.normal == Vector3.zero)
            {
                Physics.gravity = Vector3.down * fallingGravity;
            }
            else
            {
                Physics.gravity = Vector3.down * defaultGravity;
            }
        }

        public float EaseOutQuint(float scalar)
        {
            return 1 - Mathf.Pow(1 - scalar, 5);
        }

        public float EaseInOutQuint(float x)
        {
            return x < 0.5 ? 16 * x * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;
        }

        public void SpeedUpTimer()
        {
            OnStopTimer = 0;
            OnMoveTimer += Time.deltaTime;

            SpeedScalar = Mathf.Clamp(OnMoveTimer / timeToMaxSpeed, 0, 1);
            SpeedScalar = EaseOutQuint(SpeedScalar);
        }

        public void SlowDownTimer()
        {
            OnMoveTimer = 0;
            OnStopTimer += Time.deltaTime;

            SpeedScalar = Mathf.Clamp(OnStopTimer / timeToStop, 0, 1);
            SpeedScalar = EaseInOutQuint(SpeedScalar);
        }

        //this is so wrong we're clamping our velocity between 0 -> 200 lol
        private void ClampVelocity(Rigidbody RB)
        {
            if (Mathf.Abs(RB.velocity.x) > MaxSpeed)
            {
                if (RB.velocity.x < 0)
                {
                    RB.velocity = new Vector3(-MaxSpeed, 0, RB.velocity.z);
                }
                else
                {
                    RB.velocity = new Vector3(MaxSpeed, 0, RB.velocity.z);
                }
            }
            
            if (Mathf.Abs(RB.velocity.z) > MaxSpeed)
            {
                if (RB.velocity.z < 0)
                {
                    RB.velocity = new Vector3(RB.velocity.x, 0, -MaxSpeed);
                }
                else
                {
                    RB.velocity = new Vector3(RB.velocity.x, 0, MaxSpeed);
                }
            }
        }

#if UNITY_EDITOR
        //private void OnDrawGizmos()
        //{
        //    //XYZ Axes
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawRay(transform.position, directionData[0].vector * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, directionData[1].vector * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, directionData[2].vector * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, directionData[3].vector * groundRayDistance);

        //    //Diagonal XYZ Axes
        //    Gizmos.color = Color.magenta;
        //    Gizmos.DrawRay(transform.position, directionData[4].vector * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, directionData[5].vector * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, directionData[6].vector * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, directionData[7].vector * groundRayDistance);

        //    //Ground Cast Rays
        //    Gizmos.color = Color.cyan;
        //    Gizmos.DrawRay(offsetPosition + (directionData[0].vector * groundCastOffset), Vector3.down * groundRayDistance);
        //    Gizmos.DrawRay(offsetPosition + (directionData[1].vector * groundCastOffset), Vector3.down * groundRayDistance);
        //    Gizmos.DrawRay(offsetPosition + (directionData[2].vector * groundCastOffset), Vector3.down * groundRayDistance);
        //    Gizmos.DrawRay(offsetPosition + (directionData[3].vector * groundCastOffset), Vector3.down * groundRayDistance);
        //    Gizmos.DrawRay(offsetPosition + (directionData[4].vector * groundCastOffset), Vector3.down * groundRayDistance);
        //    Gizmos.DrawRay(offsetPosition + (directionData[5].vector * groundCastOffset), Vector3.down * groundRayDistance);
        //    Gizmos.DrawRay(offsetPosition + (directionData[6].vector * groundCastOffset), Vector3.down * groundRayDistance);
        //    Gizmos.DrawRay(offsetPosition + (directionData[7].vector * groundCastOffset), Vector3.down * groundRayDistance);

        //    //Static Vectors
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawRay(transform.position, defaultStaticVectors[0] * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, defaultStaticVectors[1] * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, defaultStaticVectors[2] * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, defaultStaticVectors[3] * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, defaultStaticVectors[4] * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, defaultStaticVectors[5] * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, defaultStaticVectors[6] * groundRayDistance);
        //    Gizmos.DrawRay(transform.position, defaultStaticVectors[7] * groundRayDistance);

        //    Gizmos.color = Color.green;
        //    Vector3 position = new Vector3(transform.position.x, transform.position.y - yOffset, transform.position.z);
        //    Gizmos.DrawRay(position, Vector3.down * groundRayDistance);
        //}
#endif
    }