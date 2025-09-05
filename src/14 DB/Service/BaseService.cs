namespace EGGPLANT
{
    public abstract class BaseService
    {
        protected readonly ISqliteConnectionFactory Factory;
        protected BaseService(ISqliteConnectionFactory factory) => Factory = factory;
        protected System.Data.SQLite.SQLiteConnection Open() => Factory.CreateOpen();
    }
}
