using System.Collections.Generic;
using AssistantProcessor.Enums;

namespace AssistantProcessor.Models
{
    public class ActionBlock
    {
        public List<ObjectMemento> Mementoes { get; }
        public List<EditorAction> EditorActions { get; }

        private ActionBlock(List<ObjectMemento> Mementoes, List<EditorAction> EditorActions)
        {
            this.EditorActions = EditorActions;
            this.Mementoes = Mementoes;
        }
        public ActionBlock()
        {
            EditorActions = new List<EditorAction>();
            Mementoes = new List<ObjectMemento>();
        }

        public void AddAction(EditorAction editorAction, ObjectMemento objectMemento)
        {
            Mementoes.Add(objectMemento);
            EditorActions.Add(editorAction);
        }

        public ActionBlock Clone()
        {
            return new ActionBlock(Mementoes, EditorActions);
        }
    }
}
