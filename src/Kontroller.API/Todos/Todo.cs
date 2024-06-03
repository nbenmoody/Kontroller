namespace Kontroller.API.Todos;

internal sealed class Todo(int id, string name, bool isComplete)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public bool IsComplete { get; set; } = isComplete;
}
