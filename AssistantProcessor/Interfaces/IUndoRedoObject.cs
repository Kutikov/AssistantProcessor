using AssistantProcessor.Models;

namespace AssistantProcessor.Interfaces
{
    public interface IUndoRedoObject
    {
        public ObjectMemento SaveState();
        public ObjectMemento RestoreState(ObjectMemento objectMemento);
    }
}
