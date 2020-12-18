using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimuBTree
{
  class BTreeClass
  {
    internal readonly int Order, HalfFull;
    internal int NbKeys = 0;
    internal int Deep;
    internal bool IsFull(BTreeNode node) => node.NbKeys >= Order;
    internal bool IsHalfFull(BTreeNode node) => node.NbKeys >= HalfFull;

    // On s'arrange pour que lors du split de la racine, root reste la racine.
    // Ce n'est pas obligatoire, mais sera utile lorsque on gèrera un BTree persistant.
    internal readonly BTreeNode Root = new BTreeNode();
    internal BTreeClass(int order)
    {
      Order = order;
      HalfFull = (order - 1) / 2;
    }

    #region Ajout clé
    // recherche value dans Root
    // si trouvé, return false
    // si Root n'a pas d'enfant, 
    //   insertion dans Root de value
    // sinon 
    //   recherche dans les enfants
    //   si value trouvée dans enfants, return false
    //   si enfant a été scindé, insertion dans Root de keyUp et newNode retourné par la recherche
    // si Root sursaturé, split root
    // return true
    public bool Add(int value)
    {
      (int idxK, bool exists) item = Root.FindValue(value);
      if (item.exists)
      {
        return false;
      }
      int idxK = item.idxK;
      if (!Root.Leaf)
      {
        (bool success, bool split, int keyUP, BTreeNode newNode) result = Add(idxK + 1, value, Root);
        if (!result.success)
        {
          return false;
        }
        if (result.split)
        {
          Root.InsertValue(idxK + 1, result.keyUP, result.newNode);
        }
      }
      else
      {
        Root.InsertValue(idxK + 1, value, null);
      }
      if (IsFull(Root))
      {
        SplitRoot();
      }
      NbKeys++;
      return true;
    }

    // recherche de value dans node = parent.Children[idxChild]
    // si trouvé, return false
    // si node a des children, return recherche de value dans le child de node concerné
    // insertion de value dans node
    // si node sursaturé, return resultat du split de node
    // return true
    private (bool success, bool split, int keyUP, BTreeNode newNode) Add(int idxChild, int value, BTreeNode parent)
    {
      BTreeNode node = parent.Child(idxChild);
      (int idxK, bool exists) item = node.FindValue(value);
      int idxK = item.idxK;
      if (item.exists)
      {
        return (false, false, int.MinValue, null);
      }
      if (!node.Leaf)
      {
        (bool success, bool split, int keyUP, BTreeNode newNode) result = Add(idxK + 1, value, node);
        if (!result.success)
        {
          return (false, false, int.MinValue, null);
        }
        if (result.split)
        {
          node.InsertValue(idxK + 1, result.keyUP, result.newNode);
        }
      }
      else
      {
        node.InsertValue(idxK + 1, value, null);
      }

      if (IsFull(node))
      {
        return SplitNode(node);
      }

      return (true, false, int.MinValue, null);
    }

    private void SplitRoot()
    {
      Deep++;
      bool bChildren = !Root.Leaf;
      int idxPivot = Order / 2;
      int pivot = Root.Key(idxPivot);
      BTreeNode gauche = new BTreeNode();
      BTreeNode droite = new BTreeNode();
      for (int i = 0; i < idxPivot; i++)
      {
        gauche.AddKey(Root.Key(i));
      }
      for (int i = idxPivot + 1; i < Order; i++)
      {
        droite.AddKey(Root.Key(i));
      }
      if (bChildren)
      {
        for (int i = 0; i <= idxPivot; i++)
        {
          gauche.AddChild(Root.Child(i));
        }
        for (int i = idxPivot + 1; i <= Order; i++)
        {
          droite.AddChild(Root.Child(i));
        }
      }
      Root.InitNewRoot(pivot, gauche, droite);
    }

    private (bool success, bool split, int keyUP, BTreeNode newNode) SplitNode(BTreeNode node)
    {
      bool bChildren = !node.Leaf;
      int idxPivot = Order / 2;
      int pivot = node.Key(idxPivot);
      BTreeNode newNode = new BTreeNode();
      newNode.AddKeys(node, idxPivot, Order);
      node.RemoveKeys(idxPivot, Order);
      if (bChildren)
      {
        newNode.AddChildren(node, idxPivot, Order);
        node.RemoveChildren(idxPivot, Order);
      }
      return (true, true, pivot, newNode);
    }
    #endregion Ajout clé

    #region Suppression clé
    public bool Remove(int value)
    {
      // Recherche de la valeur
      BTreeAncetres ancetres = new BTreeAncetres();
      // Recevra le noeud qui contient la clé
      BTreeNode node = Root;
      // index de la clé cherchée, ou du sous-arbre à suivre pour la trouver
      int idx;
      for (; ; )
      {
        (int idx, bool exists) resultFindValue = node.FindValue(value);
        idx = resultFindValue.idx;
        if (resultFindValue.exists)
        {
          // On a trouvé la clé dans node
          break;
        }
        if (node.Leaf)
        {
          // On est arrivé en bas de l'arbre sans avoir trouvé la clé
          return false;
        }
        ancetres.Add(new BTreeNodeParent(node, idx));
        // idx est l'index de la + grande clé < value (ou -1)
        // idx+1 est l'index du sous-arbre pouvant contenir value
        node = node.Child(idx + 1);
      }

      if (!node.Leaf)
      {
        // On n'est pas dans une feuille. 
        // On va chercher la feuille contenant les plus grandes clés < value
        // node.Keys[idx] est la value à supprimer, node.Children[idx] est le sous-arbre qui contient la + grande clé < value
        BTreeNode nodeMax = node.Child(idx);
        // on ajoute à la liste des ancètres une indication semblable à celles 
        // ajoutées dans la boucle précédente, lorsqu'on ne trouvait pas value
        ancetres.Add(new BTreeNodeParent(node, idx - 1));
        int idxMax;
        while (!nodeMax.Leaf)
        {
          // on simule le résultat de nodeMax.FindValue, pour cette value > à toute clé
          idxMax = nodeMax.NbKeys - 1;
          ancetres.Add(new BTreeNodeParent(nodeMax, idxMax));
          nodeMax = nodeMax.Child(idxMax + 1);
        }
        // On remplace value par la plus grande de ces clés
        idxMax = nodeMax.NbKeys - 1;
        int valueMax = nodeMax.Key(idxMax);
        node.SetKey(idx, valueMax);
        // Et maintenant, on va faire comme si on avait trouvé value dans nodeMax
        node = nodeMax;
        idx = idxMax;
      }
      node.RemoveKey(idx);
      NbKeys--;

      if (!IsHalfFull(node) && node != Root)
      {
        // La feuille est dépeuplée, débute le processus de balance ou de fusion
        BalanceOuFusionne(node, ancetres);
      }
      return true;
    }

    // Le node n'est pas la racine (il a un parent) et a un déficit de clés
    // On cherche un sibling qui pourrait en fournir un (balance), 
    // ou sinon avec lequel il pourrait fusionner.
    // quatre possibilités (sachant que node a au moins un voisin, à gauche ou à droite) : 
    // le voisin de gauche de node existe et a suffisamment de clés : 
    //   il fournit sa + grande clé pour compenser node
    // le voisin de droite de node existe et a suffisamment de clés : 
    //   il fournit sa + petite clé pour compenser node
    // le voisin de gauche de node existe, il fusionne avec node
    // le voisin de droite de node existe, il fusionne avec node
    private void BalanceOuFusionne(BTreeNode node, BTreeAncetres ancetres)
    {
      // Il nous faut le père de ce noeud
      BTreeNodeParent parentCoord = ancetres.Parent;
      // le parent de node
      BTreeNode parent = parentCoord.parent;
      // idx est le résultat de parent.FindValue lors de la recherche de la valeur à supprimer.
      // idx = -1 ou désigne la case de la clé dans parent juste inférieure à la value à supprimer.
      // idx désigne aussi l'arbre des clés < value
      // idx+1 est l'arbre suivi lors de la recherche de value, c'est node
      int idx = parentCoord.idx;
      BTreeNode sibling;
      if (idx >= 0)
      {
        // Le voisin de gauche existe
        sibling = parent.Child(idx);
        if (IsHalfFull(sibling))
        {
          // sibling a suffisamment de clés, il fournit la plus grande qui remonte dans parent,
          // à la place d'une clé de parent qui descend dans node
          // Balance
          node.InsertKey(0, parent.Key(idx));
          parent.SetKey(idx, sibling.Key(sibling.NbKeys - 1));
          sibling.RemoveKey(sibling.NbKeys - 1);
          if (!node.Leaf)
          {
            node.InsertChild(0, sibling.Child(sibling.NbChildren - 1));
            sibling.RemoveChild(sibling.NbChildren - 1);
          }
          return; // =====================================================> RETURN
        }
      }
      // Le voisin de gauche n'existe pas, ou il est dépeuplé
      if (idx < parent.NbKeys - 1)
      {
        // Le voisin de droite existe
        sibling = parent.Child(idx + 2);
        if (IsHalfFull(sibling))
        {
          // sibling a suffisamment de clés, il fournit la plus petite qui remonte dans parent,
          // à la place d'une clé de parent qui descend dans node
          // Balance
          node.AddKey(parent.Key(idx + 1));
          parent.SetKey(idx + 1, sibling.Key(0));
          sibling.RemoveKey(0);
          if (!node.Leaf)
          {
            node.AddChild(sibling.Child(0));
            sibling.RemoveChild(0);
          }
          return; // =====================================================> RETURN
        }
      }
      // aucun des voisins ne convient pour effectuer une balance, on réalise donc une fusion
      BTreeNode gauche, droite;
      if (idx >= 0)
      {
        // Le voisin de gauche existe, on va le fusionner avec node qui sera le voisin de droite, et la clé de parent qui les sépare
        gauche = parent.Child(idx);
        droite = node;
      }
      else
      {
        gauche = node;
        idx += 1;
        droite = parent.Child(idx + 1);
      }
      // On ajoute à "gauche" la clé qui descend du père
      gauche.AddKey(parent.Key(idx));
      // on enlève cette clé dans le père
      parent.RemoveKey(idx);
      // on ajoute à "gauche" les clés de "droite"
      gauche.AddKeys(droite);
      // on retire le pointeur sur "droite"
      parent.RemoveChild(idx + 1);
      // on ajoute à "gauche" les pointeurs qui étaient auparavant dans "droite"
      gauche.AddChildren(droite);
      if (!IsHalfFull(parent) && parent != Root)
      {
        ancetres.RemoveParent();
        BalanceOuFusionne(parent, ancetres);
      }
      // traiter cas particulier du parent == Root && parent.Keys.Count == 0 (après fusion)
      if (parent == Root && Root.NbKeys == 0)
      {
        // On pourrait réaffecter Root = node;
        // Mais on veut conserver Root 
        Root.InitNewRoot(gauche);
        Deep--;
      }

    }
    #endregion Suppression clé

    #region Vérification arbre
    // vérification que les valeurs sont bien triées
    // vérification que tous les chemins sont de même longueur
    // décompte du nombre de keys max, du nombre de keys actuel
    // On parcourt le btree en profondeur
    private class ScanDataClass
    {
      public bool previousValueSet = false;
      public int previousValue = int.MinValue;
      public bool bLgCheminSet = false;
      public int lgChemin = int.MinValue;
      public int nbKeysMax = 0;
      public int nbKeysActual = 0;
    }
    internal void Scan()
    {
      if (Root.NbKeys == 0)
      {
        return;
      }
      ScanDataClass scanData = new ScanDataClass();
      ScanBTree(Root, 0, scanData);
      Helper.Trace($"lgChemin={scanData.lgChemin}, nbkeysmax={scanData.nbKeysMax}, nbkeysactual={scanData.nbKeysActual}, remplissage={100.0 * scanData.nbKeysActual / scanData.nbKeysMax} %");
      Helper.Assert(scanData.nbKeysActual == NbKeys);
    }
    private void ScanBTree(BTreeNode node, int profondeur, ScanDataClass scanData)
    {
      scanData.nbKeysMax += Order - 1;
      scanData.nbKeysActual += node.NbKeys;
      bool bchildren = !node.Leaf;
      Helper.Assert(node == Root || IsHalfFull(node));
      if (bchildren)
      {
        // On descend en bas à gauche
        Helper.Assert(node.NbKeys == node.NbChildren - 1);
        ScanBTree(node.Child(0), profondeur + 1, scanData);
      }
      else if (scanData.bLgCheminSet)
      {
        Helper.Assert(profondeur == scanData.lgChemin);
      }
      else
      {
        scanData.lgChemin = profondeur;
        scanData.bLgCheminSet = true;
      }
      for (int i = 0; i < node.NbKeys; i++)
      {
        int value = node.Key(i);
        if (!scanData.previousValueSet)
        {
          Helper.Assert(i == 0 && !bchildren);
          scanData.previousValue = node.Key(0);
          scanData.previousValueSet = true;
        }
        else
        {
          // previousValue est censée être la plus grande key dans le sous-arbre à gauche 
          Helper.Assert(scanData.previousValue < value);
          scanData.previousValue = value;
        }
        if (bchildren)
        {
          ScanBTree(node.Child(i + 1), profondeur + 1, scanData);
        }
      }
    }
    #endregion Vérification arbre

  }
}
