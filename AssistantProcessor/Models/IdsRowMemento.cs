using System.Collections.Generic;

namespace AssistantProcessor.Models
{
    public class IdsRowMemento : ObjectMemento
    {
        public List<string> IdsRList { get; }
        public List<string> IdsTList { get; }

        public IdsRowMemento(List<string> IdsRList, List<string> IdsTList)
        {
            this.IdsRList = IdsRList;
            this.IdsTList = IdsTList;
        }
    }
}
