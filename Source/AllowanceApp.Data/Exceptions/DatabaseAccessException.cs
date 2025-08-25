namespace AllowanceApp.Data.Exceptions
{
    [Serializable]
    public class DatabaseUpdateFailure : Exception
    {
        public DatabaseUpdateFailure() { }
        public DatabaseUpdateFailure(string message) : base(message) { }
        public DatabaseUpdateFailure(string message, Exception inner) : base(message, inner) { }
    }
}
