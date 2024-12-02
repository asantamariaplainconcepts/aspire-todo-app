namespace FunctionalTests.Features
{
    public static class ApiDefinition
    {
        public static class V1
        {
            public static class Todo
            {
                public static string GetTodos()
                {
                    return $"/todos";
                }
                
                public static string CreateTodo()
                {
                    return $"/todos";
                }
                
                public static string GetTodo(Guid id)
                {
                    return $"/todos/{id}";
                }
                
                public static string DeleteTodo(Guid id)
                {
                    return $"/todos/{id}";
                }

                public static string UpdateTodo(Guid id)
                {
                    return $"/todos/{id}";
                }
                
                public static string CompleteTodo(Guid id)
                {
                    return $"/todos/{id}/complete";
                }
            }
        }

    }
}
