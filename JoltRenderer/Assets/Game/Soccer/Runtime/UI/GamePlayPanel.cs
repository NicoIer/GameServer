using System;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Soccer.UI
{
    public class GamePlayPanel : MonoBehaviour
    // , IDragHandler , IPointerClickHandler
    {
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI rttText;
        public GameObject container;
        // DynamicJoystick dynamicJoystick;

        private void OnEnable()
        {
            container.SetActive(Application.isMobilePlatform);
        }

        private void Update()
        {
            rttText.text = $"Ping: {NetworkTime.Singleton.rttMs:F0} ms,FPS:{1 / Time.deltaTime:F0}";
        }

        public void UpdateWorldData(in WorldData message)
        {
            scoreText.text = $"Red: {message.redScore} - Blue: {message.blueScore}";
        }
        //
        // public void OnDrag(PointerEventData eventData)
        // {
        // }
        //
        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     
        // }
    }
}