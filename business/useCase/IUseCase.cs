namespace TimeClock.business.useCase
{
    internal interface IUseCase<T, C> where C : ICommand
    {
        T Handle(C command);
    }
}