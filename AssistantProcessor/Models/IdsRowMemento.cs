using System.Collections.Generic;
using System.Linq;

namespace AssistantProcessor.Models
{
    public class IdsRowMemento : ObjectMemento
    {
        public List<string> IdsRList { get; }
        public List<string> IdsTList { get; }

        public IdsRowMemento(IEnumerable<string> IdsRList, IEnumerable<string> IdsTList)
        {
            this.IdsRList = IdsRList.ToList();
            this.IdsTList = IdsTList.ToList();
        }
    }
}
