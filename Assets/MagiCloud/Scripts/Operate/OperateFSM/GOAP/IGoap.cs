using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.Common.GOAP
{
    public interface IGoap
    {
    }
    public class GoapNode
    {
        private static int maxId;
        public int id;
        public GoapAction action;
        public GoapNode parent;
        public float runningCost;
        public Dictionary<string,bool> state;
        public float weight;

        public GoapNode(GoapNode parent,float runningCost,float weight,Dictionary<string,bool> state,GoapAction action)
        {
            id=maxId++;
            Init(parent,runningCost,weight,state,action);
        }
        public void Init(GoapNode parent,float runningCost,float weighth,Dictionary<string,bool> state,GoapAction action)
        {
            Clear();
            this.action=action;
            this.parent=parent;
            this.runningCost=runningCost;
            this.state=state;
            this.weight=weighth;
        }

        private void Clear()
        {
            parent=null;
            runningCost=0;
            weight=0;
            state=null;
            action=null;
        }

        public bool BetterThen(GoapNode node)
        {
            if (weight>node.weight&&runningCost<node.runningCost) return true;
            if (weight<node.weight&&runningCost > node.runningCost) return false;
            var better = (weight/node.weight-1)>=(runningCost/node.runningCost-1);
            return better;
        }
    }

    public class Node
    {
        public Node parent;
        public float runningCost;
        public HashSet<KeyValuePair<string,object>> state;
        public GoapAction action;

        public Node(Node parent,float runningCost,HashSet<KeyValuePair<string,object>> state,GoapAction action)
        {
            this.parent=parent;
            this.runningCost=runningCost;
            this.state=state;
            this.action=action;
        }
    }

    public class GoapGraph
    {
        public Queue<GoapAction> Plan(GameObject agent,HashSet<GoapAction> availableActions,HashSet<KeyValuePair<string,object>> worldState,HashSet<KeyValuePair<string,object>> goal)
        {
            foreach (var item in availableActions)
            {
                item.DoReset();
            }
            HashSet<GoapAction> actions = new HashSet<GoapAction>();
            foreach (var item in availableActions)
            {
                if (item.CheckProceduraPrecondition(agent))
                    actions.Add(item);
            }
            List<Node> nodes = new List<Node>();
            Node head = new Node(null,0,worldState,null);
            bool success = BuildGraph(head,nodes,actions,goal);
            if (!success) return null;
            Node cheapest = null;
            foreach (var item in nodes)
            {
                if (cheapest==null)
                    cheapest=item;
                else
                {
                    if (item.runningCost<cheapest.runningCost)
                        cheapest=item;
                }
            }
            List<GoapAction> result = new List<GoapAction>();
            Node node = cheapest;
            while (node!=null)
            {
                if (node.action!=null)
                    result.Insert(0,node.action);
                node=node.parent;
            }
            Queue<GoapAction> queue = new Queue<GoapAction>();
            foreach (var item in result)
            {
                queue.Enqueue(item);
            }
            return queue;
        }

        private bool BuildGraph(Node parent,List<Node> nodes,HashSet<GoapAction> actions,HashSet<KeyValuePair<string,object>> goal)
        {
            bool found = false;
            foreach (var item in actions)
            {
                if (InState(item.Preconditions,parent.state))
                {
                    HashSet<KeyValuePair<string,object>> cur = PopulateState(parent.state,item.Effects);
                    Node node = new Node(parent,parent.runningCost+item.cost,cur,item);
                    if (InState(goal,cur))
                    {
                        nodes.Add(node);
                        found=true;
                    }
                    else
                    {
                        HashSet<GoapAction> subset = ActionSubset(actions,item);
                        bool f = BuildGraph(node,nodes,subset,goal);
                        if (f)
                            found=true;
                    }
                }
            }
            return found;
        }

        private HashSet<KeyValuePair<string,object>> PopulateState(HashSet<KeyValuePair<string,object>> curState,HashSet<KeyValuePair<string,object>> toState)
        {
            HashSet<KeyValuePair<string,object>> state = new HashSet<KeyValuePair<string,object>>();
            foreach (var item in curState)
            {
                state.Add(new KeyValuePair<string,object>(item.Key,item.Value));
            }
            foreach (var item in toState)
            {
                bool exist = false;
                foreach (var s in state)
                {
                    if (s.Equals(item))
                    {
                        exist=true;
                        break;
                    }
                }
                if (exist)
                {
                    state.RemoveWhere((KeyValuePair<string,object> kvp) => { return kvp.Value.Equals(item.Key); });
                    KeyValuePair<string,object> updated = new KeyValuePair<string,object>(item.Key,item.Value);
                    state.Add(updated);
                }
            }
            return state;
        }
        private HashSet<GoapAction> ActionSubset(HashSet<GoapAction> actions,GoapAction action)
        {
            HashSet<GoapAction> subset = new HashSet<GoapAction>();
            foreach (var item in actions)
            {
                if (!item.Equals(action))
                    subset.Add(item);
            }
            return subset;
        }

        private bool InState(HashSet<KeyValuePair<string,object>> aState,HashSet<KeyValuePair<string,object>> bState)
        {
            bool allMatch = true;       //匹配
            foreach (var a in aState)
            {
                bool match = false;
                foreach (var b in bState)
                {
                    if (b.Equals(a))
                    {
                        match=true;
                        break;
                    }
                }
                if (!match)
                    allMatch=false;
            }
            return allMatch;
        }
    }

    public class GoapAction
    {
        internal float cost;

        public HashSet<KeyValuePair<string,object>> Preconditions { get; internal set; }
        public HashSet<KeyValuePair<string,object>> Effects { get; internal set; }

        internal bool CheckProceduraPrecondition(GameObject agent)
        {
            throw new NotImplementedException();
        }

        internal void DoReset()
        {
            throw new NotImplementedException();
        }
    }
    public class GoapAgent
    {

    }
    public interface IAgent { }
}
