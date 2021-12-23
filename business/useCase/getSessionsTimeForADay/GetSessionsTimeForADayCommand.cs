using System;

namespace TimeClock.business.useCase.getSessionsTimeForADay
{
    public class GetSessionsTimeForADayCommand : ICommand
    {
        public DateTime Date;

        public GetSessionsTimeForADayCommand(DateTime date)
        {
            this.Date = date;
        }
    }
}
