using System;

namespace TimeClock.business.model.workSession
{
    public class WorkSession
    {
        public string Id { get; private set; }
        public WorkSessionType Type { get; private set; }
        public DateTime Date { get; private set; }

        public WorkSession(WorkSessionType workSessionType)
        {
            Type = workSessionType;
            Date = DateTime.Now;
        }

        public WorkSession(string id, WorkSessionType type, DateTime date)
        {
            Id = id;
            Type = type;
            Date = date;
        }

        public WorkSession()
        {
        }

        public WorkSession(WorkSessionType type, DateTime date) : this(type)
        {
            Date = date;
        }

        public override bool Equals(object obj)
        {
            return obj is WorkSession session &&
                   Id == session.Id &&
                   Type == session.Type &&
                   Date == session.Date;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Type, Date);
        }
    }
}
