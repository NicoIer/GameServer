// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MLAgents;
// using MLAgents.Policies;
// using MLAgents.Sensors;
//
// public class SoccerAgent : Agent
// {
//     public Rigidbody ball;
//     public float speed = 20.0f;
//     public Team team;
//     Rigidbody player;
//     float m_KickPower;
//
//     Vector3 startPosition;
//     Quaternion startRotation;
//     
//
//     public enum Team
//     {
//         Blue = 0,
//         Red = 1
//     }
//
//     public override void Initialize()
//     {
//         player = this.GetComponent<Rigidbody>();
//         startPosition = this.transform.localPosition;
//         startRotation = this.transform.rotation;
//         var behaviorParameters = this.GetComponent<BehaviorParameters>();
//
//         if(behaviorParameters.TeamId == (int)Team.Blue)
//         {
//             team = Team.Blue;
//         }
//         else
//         {
//             team = Team.Red;
//         }
//
//     }
//
//
//     public override void OnActionReceived(float[] vectorAction)
//     {
//         var dirToGo = Vector3.zero;
//         var rotateDir = Vector3.zero;
//
//         m_KickPower = 0f;
//
//         var forwardAxis = (int)vectorAction[0];
//         var rightAxis = (int)vectorAction[1];
//         var rotateAxis = (int)vectorAction[2];
//
//         switch (forwardAxis)
//         {
//             case 1:
//                 dirToGo = transform.forward * 1f;
//                 m_KickPower = 1f;
//                 break;
//             case 2:
//                 dirToGo = transform.forward * -1f;
//                 break;
//         }
//
//         switch (rightAxis)
//         {
//             case 1:
//                 dirToGo = transform.right * 0.3f;
//                 break;
//             case 2:
//                 dirToGo = transform.right * -0.3f;
//                 break;
//         }
//
//         switch (rotateAxis)
//         {
//             case 1:
//                 rotateDir = transform.up * -1f;
//                 break;
//             case 2:
//                 rotateDir = transform.up * 1f;
//                 break;
//         }
//
//         transform.Rotate(rotateDir, Time.fixedDeltaTime * 100f);
//         player.AddForce(dirToGo * speed, ForceMode.VelocityChange);
//         AddReward(-1.0f / 3000);
//     }
//
//     public override void OnEpisodeBegin()
//     {
//         player.transform.localPosition = startPosition;
//         player.transform.rotation = startRotation;
//         player.velocity = Vector3.zero;
//         player.angularVelocity = Vector3.zero;
//     }
//
//     private void OnCollisionEnter(Collision collision)
//     {
//         if (collision.gameObject.CompareTag("ball"))
//         {
//             // Vector3 direction = collision.gameObject.transform.localPosition - this.transform.localPosition;
//             Vector3 direction = collision.contacts[0].point - this.transform.localPosition;
//             direction = direction.normalized;
//             float force = 2000.0f * m_KickPower;
//             collision.rigidbody.AddForce(direction * force);
//         }
//     }
//
//     public override float[] Heuristic()
//     {
//         var action = new float[3];
//         //forward
//         if (Input.GetKey(KeyCode.W))
//         {
//             action[0] = 1f;
//         }
//         if (Input.GetKey(KeyCode.S))
//         {
//             action[0] = 2f;
//         }
//         //rotate
//         if (Input.GetKey(KeyCode.A))
//         {
//             action[2] = 1f;
//         }
//         if (Input.GetKey(KeyCode.D))
//         {
//             action[2] = 2f;
//         }
//         //right
//         if (Input.GetKey(KeyCode.E))
//         {
//             action[1] = 1f;
//         }
//         if (Input.GetKey(KeyCode.Q))
//         {
//             action[1] = 2f;
//         }
//         return action;
//     }
// }
