namespace AiRagMovieApp;

public class ConversationMemory
{
    private readonly List<string> _conversationMemory;
    private readonly int _maxCapacity;

    public ConversationMemory(int maxCapacity = 10)
    {
        _maxCapacity = maxCapacity;
        _conversationMemory = [];
    }

    public void AddMessage(string message)
    {
        _conversationMemory.Add(message);
        if (_conversationMemory.Count > _maxCapacity)
            _conversationMemory.RemoveAt(0);
    }
    
    public IEnumerable<string> GetConversationHistory()
    {
        return _conversationMemory.AsReadOnly();
    }
}