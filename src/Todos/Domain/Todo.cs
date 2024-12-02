using Contracts;

namespace Todos.Domain
{
    public class Todo : BaseEntity
    {
        protected Todo()
        {
        }
        
        public string Title { get; protected set; } = default!;

        public bool Completed { get; protected set; }
        
        public Todo(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            AddDomainEvent(new TodoCreatedEvent(Id));
            Title = title;
        }

        public void UpdateTitle(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            Title = title;
        }

        public void CompleteTodo()
        {
            if (Completed)
            {
                return;
            }
            
            Completed = true;
            AddDomainEvent(new TodoCompletedEvent(Id, Title));
        }
    }
}