using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinderKinect.Utils
{
    class PlayerProfile
    {
        private int score;
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }

        public PlayerProfile()
        {
            score = 0;
        }
    }


}
