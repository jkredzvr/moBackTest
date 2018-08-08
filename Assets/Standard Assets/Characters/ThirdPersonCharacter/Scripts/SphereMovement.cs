using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class SphereMovement : MonoBehaviour
    {   
        //Rate of rotation for input WASD
        public float tiltAngle = .01f;
        public float smooth = 5.0f;
        private float tiltAroundZ = 0.0f;
        private float tiltAroundX = 0.0f;

        // Update is called once per frame
        void Update()
        {
            tiltAroundZ = tiltAroundZ % 360.0f;
            tiltAroundX = tiltAroundX % 360.0f;
            //Update Sphere rotation values
            Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);
            //Smooth lerp current rotation to target rotation 
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        }

        public void move(Vector3 moveVector)
        {
            tiltAroundZ += moveVector.x * tiltAngle;

            tiltAroundX -= moveVector.z * tiltAngle;
        }

        public void ChangeMovementSpeed(float newMovementSpeed)
        {
            tiltAngle = newMovementSpeed;
        }
    }
}