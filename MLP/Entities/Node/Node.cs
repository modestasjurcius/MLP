using System.Collections.Generic;

namespace MLP.Entities.Node
{
    public class Node
    {
        public int NodeId { get; set; }
        public string Name { get; set; }
        public List<AttributeWeight> Weights { get; set; }
    }
}
