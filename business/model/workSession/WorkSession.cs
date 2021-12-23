using System;

namespace TimeClock.business.model.workSession
{
    public class WorkSession
    {

        public WorkSessionType Type { get; private set; }
        public DateTime Date { get; private set; }

        public WorkSession(WorkSessionType workSessionType)
        {
            this.Type = workSessionType;
            this.Date = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            return obj is WorkSession session &&
                   Type == session.Type &&
                   Date == session.Date;
        }

        public override int GetHashCode()
        {
            int hashCode = -1250556434;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Date.GetHashCode();
            return hashCode;
        }
    }
}
