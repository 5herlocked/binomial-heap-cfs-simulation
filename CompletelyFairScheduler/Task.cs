using System;
using System.Collections.Generic;
using System.Text;

namespace CompletelyFairScheduler
{
    public class Task : IComparable
    {
        readonly int defaultPriority = 1000;
        readonly TimeSpan defaultvRuntime = TimeSpan.FromMilliseconds(1);
        readonly int defaultPowerConsumption = 2;

        public Task(int processID, int priority, TimeSpan vruntime, int powerConsumption)
        {
            ProcessID = processID;
            Priority = priority;
            Vruntime = vruntime;
            PowerConsumption = powerConsumption;
        }

        public Task (int processID)
        {
            ProcessID = processID;
            Priority = defaultPriority;
            Vruntime = defaultvRuntime;
            PowerConsumption = defaultPowerConsumption;
        }

        public int ProcessID { get; private set; }
        public int Priority { get; private set; }
        public TimeSpan Vruntime { get; private set; }
        public int PowerConsumption { get; private set; }

        public void SetPriority (int newPriority)
        {
            Priority = newPriority;
        }

        public int CompareTo(Task obj)
        {
            // The priority modifiers to add faux bias
            SchedulingBias currentBias = Program.currentBias;
            switch(currentBias)
            {
                case SchedulingBias.Default:
                    return (Vruntime < obj.Vruntime) ? -1 : 1;
                case SchedulingBias.Energy:
                    // powerConsumption needs to be lowest
                    return (PowerConsumption < obj.PowerConsumption) ? -1 : 1;
                case SchedulingBias.Performance:
                    // Priority focused
                    return (Priority < obj.Priority) ? -1 : 1;
                case SchedulingBias.UserFocus:
                    // if this task is a user process
                    if (Priority < 1500 && Priority > 1250)
                        return -1;
                    // if the other task is a user process
                    else if (obj.Priority < 1500 && obj.Priority > 1250)
                        return 1;
                    // if neither is a user process
                    else
                        return (Vruntime < obj.Vruntime) ? -1 : 1;
            }
            return (this.Vruntime < obj.Vruntime) ? -1 : 1;
        }

        public int CompareTo(object obj)
        {
            return 0;
        }

        public override String ToString()
        {
            String output = new String("\t" + ProcessID + "\t\t" + Priority + "\t\t" + Vruntime.ToString(@"ss\.ffff") + "\t\t" + PowerConsumption);
            return output;
        }
    }
}
