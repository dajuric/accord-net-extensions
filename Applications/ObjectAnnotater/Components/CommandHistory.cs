using System;
using System.Collections.Generic;

namespace ObjectAnnotater
{
    public class CommandHistory<T>
    {
        List<T> actions;
        int currentAction;

        public CommandHistory()
        {
            this.actions = new List<T>();
            currentAction = -1;
        }

        public CommandHistory(IEnumerable<T> actions)
        {
            this.actions = new List<T>(actions);
            currentAction = this.actions.Count - 1;
        }

        public void AddOrUpdate(T action)
        {
            currentAction++;

            if (currentAction == actions.Count)
            {
                actions.Add(action);
            }
            else //if undo, then add
            {
                actions[currentAction] = action;
            }
        }

        public void Undo()
        {
            currentAction = Math.Max(-1, currentAction - 1);
        }

        public void Redo() 
        {
            currentAction = Math.Min(actions.Count - 1, currentAction + 1);
        }

        public void Clear()
        {
            actions.Clear();
            currentAction = -1;
        }

        public IEnumerable<T> GetValid()
        {
            for (int i = 0; i <= currentAction; i++)
            {
                yield return actions[i];
            }
        }

        public T Current 
        {
            get 
            {
                if (currentAction < 0)
                    return default(T);
                else
                    return actions[currentAction]; 
            } 
        }
    }
}
