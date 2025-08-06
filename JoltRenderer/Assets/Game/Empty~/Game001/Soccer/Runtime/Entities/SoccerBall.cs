using System;
using JoltWrapper;
using UnityEngine;

namespace Soccer
{
    public class SoccerBall : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.transform.TryGetComponent(out PlayerController player))
            {
                
            }
        }

        private void OnCollisionExit(Collision other)
        {
            
        }
    }
}