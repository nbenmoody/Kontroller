using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.Todos;

internal static class TodoEndpoints
{
    internal static List<Todo> fakeTodoDb =
    [
        new Todo(1, "Make Tacos", true),
        new Todo(2, "Eat Tacos", true),
        new Todo(3, "Remember Eating Tacos", false),
        new Todo(4, "Plan to make more tacos", false)
    ];

    internal static void RegisterEndpoints(WebApplication app)
    {
        var todoitems = app.MapGroup("/todoitems");
        todoitems.MapGet("/", GetTodoItems);
        todoitems.MapGet("/complete", GetCompletedTodoItems);
        todoitems.MapGet("/{id:int}", GetTodoById);
        todoitems.MapPost("/", CreateTodoItem);
        todoitems.MapPut("/{id:int}", UpdateTodoItem);
        todoitems.MapDelete("/{id:int}", DeleteTodoItemById);
    }

    private static Results<Ok<Todo[]>, NotFound> GetTodoItems()
    {
        return fakeTodoDb.Count > 0
            ? TypedResults.Ok(fakeTodoDb.ToArray())
            : TypedResults.NotFound();
    }

    private static Results<Ok<Todo[]>, NotFound> GetCompletedTodoItems()
    {
        return fakeTodoDb.Count > 0
            ? TypedResults.Ok(fakeTodoDb.Where(todo => todo.IsComplete).ToArray())
            : TypedResults.NotFound();
    }

    private static Results<Ok<Todo>, NotFound> GetTodoById(int id)
    {
        var todo = fakeTodoDb.FirstOrDefault(todo => todo.Id == id);
        return todo != null
            ? TypedResults.Ok(todo)
            : TypedResults.NotFound();
    }

    private static Results<Created<Todo>, Conflict> CreateTodoItem(Todo todo)
    {
        if (fakeTodoDb.Exists(t => t.Id == todo.Id))
        {
            return TypedResults.Conflict();
        }

        fakeTodoDb.Add(todo);
        return TypedResults.Created($"/todoitems/{todo.Id}", todo);
    }

    private static Results<NoContent, NotFound> UpdateTodoItem(Todo todo)
    {
        var index = fakeTodoDb.FindIndex(t => t.Id == todo.Id);

        if (index == -1)
        {
            return TypedResults.NotFound();
        }

        fakeTodoDb[index] = todo;
        return TypedResults.NoContent();
    }

    private static Results<NoContent, NotFound> DeleteTodoItemById(int id)
    {
        var index = fakeTodoDb.FindIndex(t => t.Id == id);

        if (index == -1)
        {
            return TypedResults.NotFound();
        }

        fakeTodoDb.RemoveAt(index);
        return TypedResults.NoContent();
    }
}
