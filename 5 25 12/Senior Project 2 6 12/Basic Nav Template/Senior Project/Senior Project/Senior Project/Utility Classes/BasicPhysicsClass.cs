using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senior_Project
{
    class BasicPhysicsClass
    {
        //basic physics constants

        //should be used to increment cycles of the Update() Method
        private int Cntr;
        private TimerClass StopWatch;
        //X,Y,Z initial position

        //class that can do very basic physic-related mathz and functions...to be expanded (maybe) later
        public BasicPhysicsClass()
        {
            Cntr = 0;
            //time measured in seconds
            StopWatch = new TimerClass(1000);
        }
        //returns an integer value of height that will be updated in the Update() method
        public float Jump(float InitialGroundHeight,float Velocity)
        {
            //time in seconds but can be changed later
            float Time = StopWatch.Seconds;
            float NewHeight=0;
            //Basic outline: we have used this in math class many times before
            //H= -16t ^2 + vt + s?
            //h= height, t= time, s=original height, v= initial velocity
            NewHeight = (-16*(Time*Time))+(Velocity*Time)+InitialGroundHeight;
            return(NewHeight);
        }
    }
}
