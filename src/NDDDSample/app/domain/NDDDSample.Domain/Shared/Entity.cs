namespace NDDDSample.Domain.Shared
{
    public abstract class Entity<TId> : IEntity<TId>
    {
        private const int HashMultiplier = 911;

        private int? cachedHashCode;

        public virtual TId ID { get; protected internal set; }

        public virtual bool IsTransient()
        {
            return ID == null || ID.Equals(default(TId));
        }

        public override bool Equals(object obj)
        {
            var other = obj as IEntity<TId>;

            if (other == null) return false;

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (IsTransient() && other.IsTransient())
            {
                return ReferenceEquals(this, other);
            }

            return other.ID.Equals(ID);
        }

        public override int GetHashCode()
        {
            if (cachedHashCode.HasValue)
            {
                return cachedHashCode.Value;
            }

            if (IsTransient())
            {
                cachedHashCode = base.GetHashCode();
            }
            else
            {
                cachedHashCode = (HashMultiplier * GetType().GetHashCode()) ^ ID.GetHashCode();
            }
            return cachedHashCode.Value;
        }

        public static bool operator ==(Entity<TId> left, Entity<TId> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity<TId> left, Entity<TId> right)
        {
            return !Equals(left, right);
        }
    }
}
