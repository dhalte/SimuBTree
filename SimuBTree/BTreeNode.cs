namespace SimuBTree
{
  public class BTreeNode
  {
    // L'ordre est égal à 1 + la taille du tableau Keys
    private int[] Keys; // Toujours alloué dans le constructeur, à la taille Ordre passée en paramètre
    // N'est alloué à sa création que lors d'un split de la racine, ou d'un noeud ayant déjà des enfants.
    // Dans ce cas, sa taille est égale à Ordre
    private BTreeNode[] Children;
    // Pour les tests :
    public void AlloueChildren() => Children = new BTreeNode[Order];

    public BTreeNode(int order)
    {
      Keys = new int[order - 1];
      // Children ne sera créé au besoin que lors du split 
    }

    internal int Order => Keys.Length + 1;
    internal bool Leaf { get => Children == null; }
    public int NbKeys { get; private set; }
    public int NbChildren => Children == null ? 0 : NbKeys + 1;
    internal bool IsFull => NbKeys == Keys.Length;
    public int Key(int idxKey) => Keys[idxKey];

    // Keys est trié, ne contient pas de doublons
    // si value trouvée en idx, alors renvoie (idx, true)
    // Si Keys vide ou value < Keys[0] return (-1, false)
    // sinon si Keys[Keys.count-1] < value, return (Keys.count-1, false)
    // sinon soit idx tel que Keys[idx] soit la plus grande clé < value, renvoie (idx, false)
    // Attention : si value non trouvée, le child à rechercher est à l'indice idx+1
    internal bool FindValue(int value, out int idx)
    {
      idx = -1;
      //Recherche dichotomique
      int borneMin = 0, borneMax = NbKeys - 1;
      // Pour le seul cas de la racine vide !
      if (borneMax < 0)
      {
        return false;
      }
      for (; ; )
      {
        int milieu = (borneMin + borneMax) / 2;
        int cmp = value.CompareTo(Keys[milieu]);
        if (cmp < 0)
        {
          if (borneMin == milieu)
          {
            idx = borneMin - 1;
            return false;
          }
          borneMax = milieu - 1;
        }
        else if (0 < cmp)
        {
          if (milieu == borneMax)
          {
            idx = borneMax;
            return false;
          }
          borneMin = milieu + 1;
        }
        else
        {
          idx = milieu;
          return true;
        }
      }
    }

    // Insertion d'une value à l'endroit indiqué, 
    // et de l'éventuel sous-arbre de clés > value issu de la scission en cas de débordement précédent
    // On doit avoir 0 <= idxK <= Keys.Count
    // On doit avoir idxK==0 || Keys[idxK-1] < value
    // On doit avoir idxK==Keys.Count || value < Keys[idxK]
    internal void Insert(int idxK, int key, BTreeNode newNode)
    {
      Helper.Assert(0 <= idxK && idxK <= NbKeys);
      Helper.Assert(idxK == 0 || Keys[idxK - 1] < key);
      Helper.Assert(idxK == NbKeys || key < Keys[idxK]);

      for (int i = NbKeys - 1; i >= idxK; i--)
      {
        Keys[i + 1] = Keys[i];
      }
      Keys[idxK] = key;
      NbKeys++;

      if (Children != null)
      {
        Helper.Assert(newNode != null);
        // Tip : l'ajout de la clé a automatiquement incrémenté NbChildren, qui est donc supérieur de 1 au nombre actuel
        // Ce qui justifie le fait de débuter la boucle à NbChildren-2
        int idxInsert = idxK + 1;
        for (int i = NbChildren - 2; i >= idxInsert; i--)
        {
          Children[i + 1] = Children[i];
        }
        Children[idxInsert] = newNode;
      }
      else
      {
        Helper.Assert(newNode == null);
      }
    }

    internal void SetKey(int idxKey, int value) => Keys[idxKey] = value;
    public BTreeNode Child(int idxChild) => Children[idxChild];
    public void AddKey(int key)
    {
      Keys[NbKeys++] = key;
    }
    internal void InsertKey(int idxKey, int key)
    {
      for (int i = NbKeys - 1; i >= idxKey; i--)
      {
        Keys[i + 1] = Keys[i];
      }
      Keys[idxKey] = key;
      NbKeys++;
    }
    public void AddChild(BTreeNode child) => InsertChild(NbChildren, child);
    
    // Le node this est full
    public void Split(int idxK, ref int key, ref BTreeNode child)
    {
      Helper.Assert(NbKeys == Keys.Length);
      int order = Order;
      int milieu = order / 2;
      int keyUp; // référence intermédiaire à la clé qui remonte, et qui sera finalement assignée au paramètre ref key
      int length;
      bool leaf = Leaf;
      BTreeNode newNode = new BTreeNode(order); // sera finalement assigné au paramètre ref child
      if (!leaf) newNode.Children = new BTreeNode[order];

      if (idxK == milieu)
      {
        keyUp = key;
        length = order - 1 - milieu;
        for (int i = 0; i < length; i++)
        {
          newNode.Keys[i] = Keys[milieu + i];
        }
        if (!leaf)
        {
          newNode.Children[0] = child;
          for (int i = 0; i < length; i++)
          {
            newNode.Children[1 + i] = Children[milieu + 1 + i];
          }
        }
      }
      else if (idxK < milieu)
      {
        keyUp = Keys[milieu - 1];
        length = Order - 1 - milieu;
        for (int i = 0; i < length; i++)
        {
          newNode.Keys[i] = Keys[milieu + i];
        }
        if (!leaf)
        {
          length = Order - milieu;
          for (int i = 0; i < length; i++)
          {
            newNode.Children[i] = Children[milieu + i];
          }
        }
        for (int i = milieu - 2; i >= idxK; i--)
        {
          Keys[i + 1] = Keys[i];
        }
        Keys[idxK] = key;
        if (!leaf)
        {
          for (int i = milieu - 1; i >= idxK + 1; i--)
          {
            Children[i + 1] = Children[i];
          }
          Children[idxK + 1] = child;
        }
      }
      else
      {
        // La clé à la moitié de this et newNode remontent vers le parent
        keyUp = Keys[milieu];
        length = idxK - 1 - milieu;
        for (int i = 0; i < length; i++)
        {
          newNode.Keys[i] = Keys[milieu + 1 + i];
        }
        newNode.Keys[idxK - 1 - milieu] = key;
        if (!leaf)
        {
          length = idxK - milieu;
          for (int i = 0; i < length; i++)
          {
            newNode.Children[i] = Children[milieu + 1 + i];
          }
          newNode.Children[idxK - milieu] = child;
        }
        length = order - 1 - idxK;
        for (int i = 0; i < length; i++)
        {
          newNode.Keys[idxK - milieu + i] = Keys[idxK + i];
        }
        if (!leaf)
        {
          for (int i = 0; i < length; i++)
          {
            newNode.Children[idxK + 1 - milieu + i] = Children[idxK + 1 + i];
          }
        }
      }
      NbKeys = milieu;
      newNode.NbKeys = order - 1 - milieu;
      key = keyUp;
      child = newNode;
    }

    internal void RemoveKey(int idxKey)
    {
      for (int i = idxKey; i < NbKeys - 1; i++)
      {
        Keys[i] = Keys[i + 1];
      }
      NbKeys--;
    }

    // Appelé lors du split de l'ancien Root
    internal void Init(int key, BTreeNode node, BTreeNode child)
    {
      Keys[0] = key;
      NbKeys = 1;
      Children = new BTreeNode[Order];
      Children[0] = node;
      Children[1] = child;
      for (int i = 2; i < Order; i++)
      {
        Children[i] = new BTreeNode(Order);
      }
    }

    internal void InsertChild(int idxInsert, BTreeNode btreeNode)
    {
      for (int i = NbChildren - 1; i >= idxInsert; i--)
      {
        Children[i + 1] = Children[i];
      }
      Children[idxInsert] = btreeNode;
    }

    internal void RemoveChild(int idxChild)
    {
      for (int i = idxChild; i < NbChildren - 1; i++)
      {
        Children[i] = Children[i + 1];
      }
    }

    internal void AddKeys(BTreeNode droite)
    {
      for (int i = 0; i < droite.NbKeys; i++)
      {
        Keys[NbKeys + i] = droite.Keys[i];
      }
      NbKeys += droite.NbKeys;
    }

    internal void AddChildren(BTreeNode droite)
    {
      for (int i = 0; i < droite.NbChildren; i++)
      {
        Children[NbChildren + i] = droite.Children[i];
      }
    }

    internal void InitNewRoot(BTreeNode gauche)
    {
      Keys = gauche.Keys;
      Children = gauche.Children;
    }
  }
}
