using JoltWrapper;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityToolkit;

namespace Soccer
{
    public class GameMgr : MonoSingleton<GameMgr>
    {
        [field: SerializeField] public JoltApplication jolt { get; private set; }
        [field: SerializeField] public JoltSceneBinder joltSceneBinder { get; private set; }
    }
}