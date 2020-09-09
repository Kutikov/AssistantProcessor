using AssistantProcessor.Models;

namespace AssistantProcessor.Interfaces
{
    public interface IUndoRedoObject
    {
        public ObjectMemento SaveState();
        public void RestoreState(ObjectMemento objectMemento);
    }
}
