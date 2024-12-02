using System.Text.Json.Serialization;

namespace BuildingBlocks.Common
{
    public class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.CreateVersion7();

        private readonly List<INotification> _domainEvents = [];
        
        [JsonIgnore]
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(INotification eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
