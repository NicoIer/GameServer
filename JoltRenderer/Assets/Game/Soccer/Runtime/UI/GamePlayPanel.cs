using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Soccer.UI
{
    public class GamePlayPanel : MonoBehaviour 
        // , IDragHandler , IPointerClickHandler
    {
        public TextMeshProUGUI scoreText;

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