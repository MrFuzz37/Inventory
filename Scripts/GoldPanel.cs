// Creates an instance of a gold panel for each player who joins the game. Brad 8/6/2020
using UnityEngine;

namespace NetworkInv_Interaction
{
    public class GoldPanel : MonoBehaviour
    {
        public static GoldPanel instance;
        public PlayerController player;

        void Awake()
        {
            instance = this;
        }
    }
}

