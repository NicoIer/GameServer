using System;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Soccer.UI
{
    public class GamePlayPanel : MonoBehaviour
    // , IDragHandler , IPointerClickHandler
    {
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI rttText;
        public GameObject container;

        private void OnEnable()
        {
            container.SetActive(Application.isMobilePlatform);
        }

        private void Update()
        {
            rttText.text = $"RTT: {NetworkTime.Singleton.rttMs:F0} ms";
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