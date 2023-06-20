namespace Core.Db
{
    public interface IUserService
    {
        string GetMyName();
        string GetMyId();
        string GetCookiesRefreshToken();
    }
}
