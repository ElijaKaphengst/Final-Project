using Managers;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Character
{
    /// <summary> Controlls the Interactions of a character. </summary>
    public class CharacterInteractions : MonoBehaviour
    {
        /// <summary> The range in wich the character can interact. </summary>
        public float interactRange = 5f;
        /// <summary> The radius of the interaction capsule. </summary>
        public float interactRadius = 3f;
        /// <summary> the off the of the capsule from the character. </summary>
        public float capsuleOffset;
        /// <summary> If the charcter is interacting with something/someone. </summary>
        public bool isInteracting;
        /// <summary> The object that was hitted by the interaction CapsuleCastAll. </summary>
        [HideInInspector]
        public Object hitObject;
        /// <summary> The object that is applied to the hitObject when no object get hit. </summary>
        [HideInInspector]
        public Object hitObjectRef;
        /// <summary> The position where the CapsuleCastAll should start. </summary>
        [HideInInspector]
        public Vector3 capsuleStartPoint;
        /// <summary> The position where the CapsuleCastAll should end. </summary>
        [HideInInspector]
        public Vector3 capsuleEndPoint;
        /// <summary> The Tag that the hitted GameObject should have. </summary>
        public Tags hitTag;
        /// <summary> The Layers that can get hit by the CapsuleCastAll </summary>
        public LayerMask layersToInteract;

        /// <summary>
        /// Casts a CapsuleCastAll and returns the first Unity Object that will be hit.
        /// </summary>
        /// <param name="capsuleStart"> Start position of the CapsuleCastAll. </param>
        /// <param name="capsuleEnd"> End position of the CapsuleCastAll. </param>
        /// <param name="castTransform"> The transform where the CapsuleCastAll gets casted. </param>
        /// <param name="objectType"> The type of the target object. </param>
        /// <param name="targetTag"> The tag of the target object. </param>
        /// <returns> Returns an Unity Object wich is used to pass the 'objectType'. </returns>
        public Object ObjectInInteractionRange(Vector3 capsuleStart, Vector3 capsuleEnd, Transform castTransform, Type objectType, string targetTag) 
        {
            //Casts a CapsuleCastAll and store the hitted objects in an RaycastHit Array.
            RaycastHit[] hits = Physics.CapsuleCastAll(capsuleStart, capsuleEnd, interactRadius, transform.forward, interactRange, layersToInteract, QueryTriggerInteraction.Collide);

            //Check if the CapsuleCast hits an object.
            if (hits.Length > 0)
            {
                //Loop trough all objects that are hit.
                for (int i = 0; i < hits.Length; i++)
                {
                    //Check if the object is the wanted object.
                    if (hits[i].transform.gameObject.tag == targetTag)
                    {
                        //Returns the type of object that is needed.
                        if (objectType == typeof(GameObject)) return hits[i].transform.gameObject;
                        else if (objectType == typeof(Transform)) return hits[i].transform;
                        else if (objectType == typeof(Collider)) return hits[i].collider;
                        else if (objectType == typeof(Rigidbody)) return hits[i].rigidbody;
                    }
                }
            }
            //If it hits no object return the hitObjectRef
            return hitObjectRef;
        }

        /// <summary>
        /// Casts an CapsuleCastAll and returns a class of type T from the first Object that will be hit.
        /// </summary>
        /// <typeparam name="T"> The type of the object. </typeparam>
        /// <param name="capsuleStart"> Start position of the CapsuleCastAll. </param>
        /// <param name="capsuleEnd"> End position of the CapsuleCastAll. </param>
        /// <param name="castTransform"> The transform where the CapsuleCastAll gets casted. </param>
        /// <param name="targetTag"> The tag of the target object. </param>
        /// <param name="hitTarget"> The type of the target class </param>
        /// <returns> returns the first hittet class of type T </returns>
        public T ObjectInInteractionRange<T>(Vector3 capsuleStart, Vector3 capsuleEnd, Transform castTransform,  string targetTag,T hitTarget) where T: class
        {
            //Casts a CapsuleCastAll and store the hitted objects in an RaycastHit Array.
            RaycastHit[] hits = Physics.CapsuleCastAll(capsuleStart, capsuleEnd, interactRadius, castTransform.forward, interactRange, layersToInteract, QueryTriggerInteraction.Collide);
            
            //Check if the CapsuleCast hits an object.
            if (hits.Length > 0)
            {
                //Loop trough all objects that are hit.
                for (int i = 0; i < hits.Length; i++)
                {
                    //Check if the object is the wanted object.
                    if (hits[i].transform.gameObject.tag == targetTag)
                    {
                        var Obj = hits[i].transform.gameObject;
                        var target = Obj.GetComponent(hitTarget.GetType());
                        return target as T;
                    }
                }
            }
            //If it hits no object return the hitObjectRef
            return hitObjectRef as T;
        }

        /// <summary> Method to set the start and end positions of the CapsuleCastAll and check if it hit's something. </summary>
        virtual public void CheckInteractionRange()
        {
            //Set the start point from the CapsuleCastAll
            capsuleStartPoint = transform.position + transform.forward * capsuleOffset;
        }
    }
}
