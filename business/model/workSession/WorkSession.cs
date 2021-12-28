using System;

namespace TimeClock.business.model.workSession
{
    public class WorkSession
    {

        public WorkSessionType Type { get; private set; }
        public DateTime Date { get; private set; }

        public WorkSession(WorkSessionType workSessionType)
        {
            Type = workSessionType;
            Date = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            return obj is WorkSession session &&
                   Type == session.Type &&
                   Date == session.Date;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Date);
        }
    }
}
