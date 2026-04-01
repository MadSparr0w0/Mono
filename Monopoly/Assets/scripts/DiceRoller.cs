using UnityEngine;

namespace MonopolyLAN
{
    public class DiceRoller : MonoBehaviour
    {
        public int RollTwoDice(out int dieA, out int dieB)
        {
            dieA = Random.Range(1, 7);
            dieB = Random.Range(1, 7);
            return dieA + dieB;
        }
    }
}
