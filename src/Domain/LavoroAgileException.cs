using System;

namespace Domain
{
    /// <summary>
    /// Rappresenta una eccezione applicativa.
    /// </summary>
    [Serializable]
    public class LavoroAgileException : Exception
    {
        public LavoroAgileException() { }
        public LavoroAgileException(string message) : base(message) { }
        public LavoroAgileException(string message, Exception inner) : base(message, inner) { }
        protected LavoroAgileException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
