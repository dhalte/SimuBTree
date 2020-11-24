using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimuBTree
{
  class BTreeClass
  {
    internal readonly int Order;
    internal int NbKeys = 0;
    internal int Deep;
    // On s'arrange pour que lors du split de la racine, root reste la racine.
    // Ce n'est pas obligatoire, mais sera utile lorsque on gèrera un BTree persistant.
    internal readonly BTreeNode Root = new BTreeNode();
    internal BTreeClass(int order)
    {
      Order = order;
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
      if (Root.Children.Count != 0)
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
      if (Root.Keys.Count >= Order)
      {
        Helper.Assert(Root.Keys.Count == Order);
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
      BTreeNode node = parent.Children[idxChild];
      (int idxK, bool exists) item = node.FindValue(value);
      int idxK = item.idxK;
      if (item.exists)
      {
        return (false, false, int.MinValue, null);
      }
      if (node.Children.Count != 0)
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

      if (node.Keys.Count >= Order)
      {
        Helper.Assert(node.Keys.Count == Order);
        return SplitNode(node);
      }

      return (true, false, int.MinValue, null);
    }

    private void SplitRoot()
    {
      Deep++;
      bool bChildren = Root.Children.Count > 0;
      int idxPivot = Order / 2;
      int pivot = Root.Keys[idxPivot];
      BTreeNode gauche = new BTreeNode();
      BTreeNode droite = new BTreeNode();
      for (int i = 0; i < idxPivot; i++)
      {
        gauche.Keys.Add(Root.Keys[i]);
      }
      for (int i = idxPivot + 1; i < Order; i++)
      {
        droite.Keys.Add(Root.Keys[i]);
      }
      if (bChildren)
      {
        for (int i = 0; i <= idxPivot; i++)
        {
          gauche.Children.Add(Root.Children[i]);
        }
        for (int i = idxPivot + 1; i <= Order; i++)
        {
          droite.Children.Add(Root.Children[i]);
        }
      }
      Root.Keys.Clear();
      Root.Keys.Add(pivot);
      Root.Children.Clear();
      Root.Children.Add(gauche);
      Root.Children.Add(droite);
    }

    private (bool success, bool split, int keyUP, BTreeNode newNode) SplitNode(BTreeNode node)
    {
      bool bChildren = node.Children.Count > 0;
      int idxPivot = Order / 2;
      int pivot = node.Keys[idxPivot];
      BTreeNode newNode = new BTreeNode();
      newNode.Keys.AddRange(node.Keys.GetRange(idxPivot + 1, Order - idxPivot - 1));
      node.Keys.RemoveRange(idxPivot, Order - idxPivot);
      if (bChildren)
      {
        newNode.Children.AddRange(node.Children.GetRange(idxPivot + 1, Order - idxPivot));
        node.Children.RemoveRange(idxPivot + 1, Order - idxPivot);
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
        if (node.Children.Count == 0)
        {
          // On est arrivé en bas de l'arbre sans avoir trouvé la clé
          return false;
        }
        ancetres.Add(new BTreeNodeParent(node, idx));
        // idx est l'index de la + grande clé < value (ou -1)
        // idx+1 est l'index du sous-arbre pouvant contenir value
        node = node.Children[idx + 1];
      }

      if (node.Children.Count > 0)
      {
        // On n'est pas dans une feuille. 
        // On va chercher la feuille contenant les plus grandes clés < value
        // node.Keys[idx] est la value à supprimer, node.Children[idx] est le sous-arbre qui contient la + grande clé < value
        BTreeNode nodeMax = node.Children[idx];
        // on ajoute à la liste des ancètres une indication semblable à celles 
        // ajoutées dans la boucle précédente, lorsqu'on ne trouvait pas value
        ancetres.Add(new BTreeNodeParent(node, idx - 1));
        int idxMax;
        while (nodeMax.Children.Count > 0)
        {
          // on simule le résultat de nodeMax.FindValue, pour cette value > à toute clé
          idxMax = nodeMax.Keys.Count - 1;
          ancetres.Add(new BTreeNodeParent(nodeMax, idxMax));
          nodeMax = nodeMax.Children[idxMax+1];
        }
        // On remplace value par la plus grande de ces clés
        idxMax = nodeMax.Keys.Count - 1;
        int valueMax = nodeMax.Keys[idxMax];
        node.Keys[idx] = valueMax;
        // Et maintenant, on va faire comme si on avait trouvé value dans nodeMax
        node = nodeMax;
        idx = idxMax;
      }
      node.Keys.RemoveAt(idx);
      NbKeys--;

      if (node.Keys.Count < (Order - 1) / 2 && node != Root)
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
        sibling = parent.Children[idx];
        if (sibling.Keys.Count > (Order - 1) / 2)
        {
          // sibling a suffisamment de clés, il fournit la plus grande qui remonte dans parent,
          // à la place d'une clé de parent qui descend dans node
          // Balance
          node.Keys.Insert(0, parent.Keys[idx]);
          parent.Keys[idx] = sibling.Keys[sibling.Keys.Count - 1];
          sibling.Keys.RemoveAt(sibling.Keys.Count - 1);
          if (node.Children.Count > 0)
          {
            node.Children.Insert(0, sibling.Children[sibling.Children.Count - 1]);
            sibling.Children.RemoveAt(sibling.Children.Count - 1);
          }
          return; // =====================================================> RETURN
        }
      }
      // Le voisin de gauche n'existe pas, ou il est dépeuplé
      if (idx < parent.Keys.Count - 1)
      {
        // Le voisin de droite existe
        sibling = parent.Children[idx + 2];
        if (sibling.Keys.Count > (Order - 1) / 2)
        {
          // sibling a suffisamment de clés, il fournit la plus petite qui remonte dans parent,
          // à la place d'une clé de parent qui descend dans node
          // Balance
          node.Keys.Add(parent.Keys[idx + 1]);
          parent.Keys[idx + 1] = sibling.Keys[0];
          sibling.Keys.RemoveAt(0);
          if (node.Children.Count > 0)
          {
            node.Children.Add(sibling.Children[0]);
            sibling.Children.RemoveAt(0);
          }
          return; // =====================================================> RETURN
        }
      }
      // aucun des voisins ne convient pour effectuer une balance, on réalise donc une fusion
      BTreeNode gauche, droite;
      if (idx >= 0)
      {
        // Le voisin de gauche existe, on va le fusionner avec node qui sera le voisin de droite, et la clé de parent qui les sépare
        gauche = parent.Children[idx];
        droite = node;
      }
      else
      {
        gauche = node;
        idx += 1;
        droite = parent.Children[idx + 1];
      }
      // On ajoute à "gauche" la clé qui descend du père
      gauche.Keys.Add(parent.Keys[idx]);
      // on enlève cette clé dans le père
      parent.Keys.RemoveAt(idx);
      // on ajoute à "gauche" les clés de "droite"
      gauche.Keys.AddRange(droite.Keys);
      // on retire le pointeur sur "droite"
      parent.Children.RemoveAt(idx + 1);
      // on ajoute à "gauche" les pointeurs qui étaient auparavant dans "droite"
      gauche.Children.AddRange(droite.Children);
      if (parent.Keys.Count < (Order - 1) / 2 && parent != Root)
      {
        ancetres.RemoveParent();
        BalanceOuFusionne(parent, ancetres);
      }
      // traiter cas particulier du parent == Root && parent.Keys.Count == 0 (après fusion)
      if (parent == Root && Root.Keys.Count == 0)
      {
        // On pourrait réaffecter Root = node;
        // Mais on veut conserver Root 
        Root.Keys = gauche.Keys;
        Root.Children = gauche.Children;
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
      public BTreeClass BTree;
      public bool previousValueSet = false;
      public int previousValue = int.MinValue;
      public bool bLgCheminSet = false;
      public int lgChemin = int.MinValue;
      public int nbKeysMax = 0;
      public int nbKeysActual = 0;

      public ScanDataClass(BTreeClass btree)
      {
        this.BTree = btree;
      }
    }
    internal void Scan()
    {
      if (Root.Keys.Count == 0)
      {
        return;
      }
      ScanDataClass scanData = new ScanDataClass(this);
      ScanBTree(Root, 0, scanData);
      Helper.Trace($"lgChemin={scanData.lgChemin}, nbkeysmax={scanData.nbKeysMax}, nbkeysactual={scanData.nbKeysActual}, remplissage={100.0 * scanData.nbKeysActual / scanData.nbKeysMax} %");
      Helper.Assert(scanData.nbKeysActual == NbKeys);
    }
    private void ScanBTree(BTreeNode node, int profondeur, ScanDataClass scanData)
    {
      scanData.nbKeysMax += Order - 1;
      scanData.nbKeysActual += node.Keys.Count;
      bool bchildren = node.Children.Count > 0;
      Helper.Assert(node == Root || node.Keys.Count >= (Order - 1) / 2);
      if (bchildren)
      {
        // On descend en bas à gauche
        Helper.Assert(node.Keys.Count == node.Children.Count - 1);
        ScanBTree(node.Children[0], profondeur + 1, scanData);
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
      for (int i = 0; i < node.Keys.Count; i++)
      {
        int value = node.Keys[i];
        if (!scanData.previousValueSet)
        {
          Helper.Assert(i == 0 && !bchildren);
          scanData.previousValue = node.Keys[0];
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
          ScanBTree(node.Children[i + 1], profondeur + 1, scanData);
        }
      }
    }
    #endregion Vérification arbre

  }
}
