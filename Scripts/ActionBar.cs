// Creates an instance of an action bar for each player who joins the game. Brad 19/5/2020
using UnityEngine;

namespace NetworkInv_Interaction
{
    public class ActionBar : MonoBehaviour
    {
        public static ActionBar instance;
        public PlayerController player;

        void Awake()
        {
            instance = this;
        }
    }
}

