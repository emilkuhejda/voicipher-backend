namespace Voicipher.Domain.Infrastructure
{
    public sealed class None
    {
        public static readonly None Value = new None();

        private None() { }
    }
}
