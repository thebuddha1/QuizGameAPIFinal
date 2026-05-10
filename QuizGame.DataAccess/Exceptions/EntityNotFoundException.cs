namespace QuizGame.DataAccess.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string name) : base($"{name} entity not found.") { }
}