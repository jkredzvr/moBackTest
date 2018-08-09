using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class SphereMovement : MonoBehaviour
    {   
        //Rate of rotation for input WASD
        public float tiltAngle = .01f;
        float tiltAroundZ = 0.0f;
        float tiltAroundX = 0.0f;

        // Update is called once per frame
        void Update()
        {
            //Rotate the sphere relative to World Space Up.
            transform.Rotate(new Vector3(tiltAroundX, 0.0f, tiltAroundZ), Space.World); //, SpaceWorld
        }

        public void move(Vector3 moveVector)
        {
            //Map x,y input movement from WASD into X and Z rotations
            //Rotate in the Z and X axis 
            tiltAroundZ = moveVector.x * tiltAngle;
            tiltAroundX = -1.0f * moveVector.z * tiltAngle;
        }

        public void ChangeMovementSpeed(float newMovementSpeed)
        {
            tiltAngle = newMovementSpeed;
        }
    }
}