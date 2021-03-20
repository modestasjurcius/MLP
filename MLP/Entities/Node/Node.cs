using System.Collections.Generic;

namespace MLP.Entities.Node
{
    public class Node
    {
        public string Name { get; set; }
        public List<AttributeWeight> Weights { get; set; }
    }
}
