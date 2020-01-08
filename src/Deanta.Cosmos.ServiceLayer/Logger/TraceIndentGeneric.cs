namespace Deanta.Cosmos.ServiceLayer.Logger
{
    public class TraceIndentGeneric<T> : TraceIdentBaseDto
    {
        public TraceIndentGeneric(string traceIdentifier, T result) 
            : base(traceIdentifier)
        {
            Result = result;
        }

        public T Result { get; }
    }
}