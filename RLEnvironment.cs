using System.Text;

namespace RL_Distillation;

/// <summary>
/// Represents the environment where learning takes place.
/// Think of this as a 1D world with positions 0-10 where position 5 is the goal.
/// </summary>
public class RLEnvironment
{
    private int currentState;
    private bool isDone;
    private double reward;

    /// <summary>
    /// Sets the current state, ensuring it stays within valid bounds (0-10)
    /// </summary>
    public void SetState(int state)
    {
        currentState = Math.Max(0, Math.Min(10, state));
    }

    /// <summary>
    /// Takes an action (0=left, 1=right) and returns the new state, reward, and whether goal is reached
    /// </summary>
    public (int state, double reward, bool done) Step(int action)
    {
        // Move left or right based on action
        if (action == 1) currentState++;
        else if (action == 0) currentState--;

        // Keep within bounds
        currentState = Math.Max(0, Math.Min(10, currentState));

        // Reward structure: +1 for reaching goal, small penalty otherwise
        reward = currentState == 5 ? 1.0 : -0.1;
        isDone = currentState == 5;

        return (currentState, reward, isDone);
    }

    public void Reset()
    {
        currentState = 0;
        reward = 0;
        isDone = false;
    }

    public string Visualize()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (int i = 0; i <= 10; i++)
        {
            if (i == currentState) sb.Append('A');
            else if (i == 5) sb.Append('G');
            else sb.Append('_');
        }
        sb.Append(']');
        return sb.ToString();
    }
}