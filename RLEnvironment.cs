using System.Text;

namespace RL_Distillation;

public class RLEnvironment
{
    private int currentState;
    private bool isDone;
    private double reward;

    public void SetState(int state)
    {
        currentState = Math.Max(0, Math.Min(10, state));
    }

    public (int state, double reward, bool done) Step(int action)
    {
        if (action == 1) currentState++;
        else if (action == 0) currentState--;

        currentState = Math.Max(0, Math.Min(10, currentState));
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