using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMAPS
{
    // The base class for all third-person camera controllers
    public abstract class TPCBase
    {
        protected Transform mCameraTransform;
        protected Transform mPlayerTransform;

        public Transform CameraTransform
        {
            get
            {
                return mCameraTransform;
            }
        }
        public Transform PlayerTransform
        {
            get
            {
                return mPlayerTransform;
            }
        }

        public TPCBase(Transform cameraTransform, Transform playerTransform)
        {
            mCameraTransform = cameraTransform;
            mPlayerTransform = playerTransform;
        }

        public void RepositionCamera()
        {

            //The wall object has a LayerMask Tag set as 'Wall'.
            //This line of code is used to get the tag of the object we want to detect, being "Wall".
            LayerMask mask = LayerMask.GetMask("Wall");
            //We check the direction we want the ray to go in, in this case the direction of the player from the camera.
            Vector3 PCVector = PlayerTransform.position - CameraTransform.position;
            Debug.DrawRay(CameraTransform.position, PCVector, Color.green);
            //This gets the direction needed for the ray to go.
            float length = PCVector.magnitude;
            //Normalize the directional vector.
            PCVector = PCVector.normalized;
            //If an object with the LayerMask "Wall" touches the raycast-
            if (Physics.Raycast(CameraTransform.position, PCVector, out RaycastHit hit, length, mask))
            {
                //Offset to fix camera height.
                float offset = CameraConstants.CameraPositionOffset.y - hit.point.y;
                Debug.Log("Hit a Wall.");
                //Set it as assignable Vector3.
                Vector3 offPoint = hit.point;
                //Fix height of camera to be equal to CameraPositionOffset.
                offPoint.y += offset;
                //Sets the camera position.
                CameraTransform.position = offPoint;
                //Updates the Vector.
                PCVector = PlayerTransform.position - CameraTransform.position;
            }
        }

        public abstract void Update();
    }
}
