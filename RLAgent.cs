namespace RL_Distillation;

/// <summary>
/// The learning agent that uses Q-learning to make decisions
/// </summary>
public class RLAgent
{
    private readonly Dictionary<int, double[]> qTable = new();
    private readonly double learningRate = 0.1; // How much the agent learns from new information, how fast does it update its knowledge
    private readonly double gamma = 0.99; // Discount factor, meaning by how much the future rewards are less valuable
    private readonly double epsilon = 0.1; // Exploration rate, meaning 10% of the time we will explore
    private readonly Random random = new();
    private int successfulEpisodes = 0;

    public int GetAction(int state)
    {
        // Exploration: Sometimes try random actions to discover new strategies
        if (random.NextDouble() < epsilon)
            return random.Next(2);

        // If we haven't seen this state, initialize it
        if (!qTable.ContainsKey(state))
            qTable[state] = new double[2];

        // Exploitation: Choose the action with the highest expected reward
        return qTable[state].ToList().IndexOf(qTable[state].Max());
    }

    public void Learn(int state, int action, double reward, int nextState)
    {
        if (!qTable.ContainsKey(state))
            qTable[state] = new double[2];

        if (!qTable.ContainsKey(nextState))
            qTable[nextState] = new double[2];

        var oldValue = qTable[state][action];
        var nextMax = qTable[nextState].Max();
        qTable[state][action] = oldValue + learningRate * (reward + gamma * nextMax - oldValue);
    }

    public void IncrementSuccess()
    {
        successfulEpisodes++;
    }

    public int GetSuccessCount()
    {
        return successfulEpisodes;
    }

    public string GetQTableVisualization(int state)
    {
        if (!qTable.ContainsKey(state))
            return "No data for this state";

        return $"Left: {qTable[state][0]:F2}, Right: {qTable[state][1]:F2}";
    }
}