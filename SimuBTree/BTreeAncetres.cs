using System;
using System.Collections.Generic;
using System.Text;

namespace SimuBTree
{
  class BTreeAncetres : List<BTreeNodeParent>
  {
    public BTreeNodeParent Parent { get { return this[Count - 1]; } }
    public void RemoveParent()
    {
      this.RemoveAt(this.Count - 1);
    }
  }
}
