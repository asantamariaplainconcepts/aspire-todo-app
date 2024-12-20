﻿using Todos.Features.Todo.Queries;

namespace FunctionalTests.Features.Todo.Queries;

public class GetTodoTestsShould(ApiServiceFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task response_ok_when_id_is_valid_one()
    {
        // Arrange
        var todo = await Given.CreateDefaultTodo();

        // Act

        var response = await Given.Client.GetAsync(ApiDefinition.V1.Todo.GetTodo(todo.Id));

        // Assert
        response.StatusCode
            .Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsJsonAsync<GetTodo.Response>();

        content.Should()
            .NotBeNull();

        content!.Id.Should().Be(todo.Id);
        content!.Title.Should().Be(todo.Title);
        content!.IsCompleted.Should().Be(todo.Completed);
    }

    [Fact]
    public async Task response_not_found_when_id_is_not_existing_one()
    {
        // Arrange
        // Act
        var response = await Given.Client.GetAsync(ApiDefinition.V1.Todo.GetTodo(Guid.Empty));

        // Assert
        response.StatusCode
            .Should().Be(HttpStatusCode.NotFound);
    }
}