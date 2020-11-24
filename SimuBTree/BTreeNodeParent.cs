using System;
using System.Collections.Generic;
using System.Text;

namespace SimuBTree
{
  class BTreeNodeParent
  {
    internal BTreeNode parent;
    internal int idx;

    internal BTreeNodeParent(BTreeNode parent, int idx)
    {
      this.parent = parent;
      this.idx = idx;
    }
  }
}
