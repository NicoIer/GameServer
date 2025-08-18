// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class GoalDetect : MonoBehaviour
// {
//     SoccerAgent.Team team;
//     public SoccerEnv env;
//
//     private void Awake()
//     {
//         if (this.CompareTag("blueGoal"))
//         {
//             team = SoccerAgent.Team.Blue;
//         }else if (this.CompareTag("redGoal"))
//         {
//             team = SoccerAgent.Team.Red;
//         }
//     }
//
//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.gameObject.CompareTag("ball"))
//         {
//             env.GoalTouched(team);
//         }
//     }
// }
