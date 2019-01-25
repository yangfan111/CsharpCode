using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtPlugins
{
    /// <summary>
    /// 层次凝聚聚类算法实现 Agglomerative Hierarchical Cluster
    /// 用于室外物件的框选合并
    /// </summary>
    public class AggHierarchicalCluster : MonoBehaviour
    {
        /// <summary>
        /// 叶子结构体，层次树的每一个叶子节点表示一个具体的gameObject
        /// </summary> 
        [System.Serializable]
        public class LeafST
        {
            public GameObject gameObject;
            public Bounds bounds;
        }

        /// <summary>
        /// 树的节点结构体
        /// </summary>
        [System.Serializable]
        public class NodeST
        {
            public NodeST left;
            public NodeST right;
            public Bounds bounds;

            /// <summary>
            /// 仅对叶子节点有意义，叶子节点所代表的gameobject
            /// </summary>
            public GameObject leafGo = null;

            public bool IsLeaf()
            {
                return left == null && right == null;
            }
        }

        /// <summary>
        /// 待合并堆结构体
        /// </summary>
        [System.Serializable]
        public class CombineST
        {
            /// <summary>
            /// 记录待合并的物体列表
            /// </summary>
            public List<GameObject> gameObjects;

            /// <summary>
            /// 记录待合并的节点
            /// </summary>
            public NodeST node;

            /// <summary>
            /// 标识待合并物体是否为多个物体
            /// </summary>
            public bool multiGoesFlag;
        }

        public bool enableGizmo = true;
        public Color gizmoColor = Color.yellow;
        public bool wireMode = true;
        public bool showSingle = false;

        public string saveDir = "Assets/Maps/maps/0001/CombineMeshesResults";

        /// <summary>
        /// 记录所有的叶子节点
        /// </summary>
        [SerializeField]
        private List<LeafST> leaves = new List<LeafST>();

        /// <summary>
        /// 记录ui界面的显示是否展开
        /// </summary>
        public bool isLeavesGuiFold = false;

        /// <summary>
        /// 当组合的物体bounds尺寸不高于该数值时进行网格合并
        /// </summary>
        public float maxSize = 40f;

        /// <summary>
        /// 记录构建树的根节点
        /// </summary>
        [SerializeField]
        private NodeST root;

        /// <summary>
        /// 记录分配后需要网格合并的堆
        /// </summary>
        public List<CombineST> combines = new List<CombineST>();

        /// <summary>
        /// 清理树叶列表
        /// </summary>
        public void ClearLeaves()
        {
            if (leaves != null) leaves.Clear();
        }

        /// <summary>
        /// 添加叶子
        /// </summary>
        public void AddLeaf(GameObject go)
        {
            if (go == null) return;

            MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer>();
            if (mrs.Length <= 0) return;

            //Bounds bounds = mrs[0].bounds;
            //for (int j = 1; j < mrs.Length - 1; j++)
            //{
            //    bounds.Encapsulate(mrs[j].bounds);
            //}

            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bool first = true;
            for (int j = 0; j < mrs.Length; j++)
            {
                MeshRenderer mr = mrs[j];
                if (mr == null) continue;

                MeshFilter mf = mr.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;

                if (first)
                {
                    bounds = mr.bounds;
                    first = false;
                }
                else
                {
                    bounds.Encapsulate(mr.bounds);
                }
            }
            if (first) return;

            LeafST leaf = new LeafST() { gameObject = go, bounds = bounds };
            leaves.Add(leaf);
        }

        /// <summary>
        /// 根据凝聚原则构建二叉树
        /// </summary>
        public void BuildBinaryTree()
        {
            if (leaves.Count <= 0) return;

            System.DateTime now = System.DateTime.Now;
            System.TimeSpan startTp = new System.TimeSpan(now.Hour, now.Minute, now.Second);
            Debug.LogFormat("start build binary tree");

            root = null;
            List<NodeST> nodes = new List<NodeST>();
            for (int i = 0; i < leaves.Count; i++)
            {
                NodeST node = new NodeST() { left = null, right = null, bounds = leaves[i].bounds, leafGo = leaves[i].gameObject };
                nodes.Add(node);
            }

            // 循环处理nodes列表直到剩余一个根节点
            while (nodes.Count > 1)
            {
                int leftIndex, rightIndex;
                GetMinDistanceTwoNodes(nodes, out leftIndex, out rightIndex);
                Bounds bounds = nodes[leftIndex].bounds;
                bounds.Encapsulate(nodes[rightIndex].bounds);
                NodeST parentNode = new NodeST() { left = nodes[leftIndex], right = nodes[rightIndex], bounds = bounds };
                nodes[leftIndex] = parentNode;
                nodes.RemoveAt(rightIndex);
            }

            root = nodes[0];

            now = System.DateTime.Now;
            System.TimeSpan finishTp = new System.TimeSpan(now.Hour, now.Minute, now.Second);
            Debug.LogFormat("finish build binary tree, diff time:{1}", finishTp.TotalSeconds, finishTp.TotalSeconds - startTp.TotalSeconds);
        }

        /// <summary>
        /// 在给定集合中查找相距最近的两个点
        /// </summary>
        private void GetMinDistanceTwoNodes(List<NodeST> nodes, out int leftIndex, out int rightIndex)
        {
            leftIndex = -1;
            rightIndex = -1;

            float minDis = float.MaxValue;
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    float dis = (nodes[j].bounds.center - nodes[i].bounds.center).sqrMagnitude;
                    if (dis < minDis)
                    {
                        minDis = dis;
                        leftIndex = i;
                        rightIndex = j;
                    }
                }
            }
        }

        /// <summary>
        /// 确定合并的物体堆
        /// </summary>
        public void Split()
        {
            if (root == null)
            {
                Debug.LogErrorFormat("AggHierarchicalCluster.Split error, root is null");
                return;
            }

            if (maxSize < 1f)
            {
                Debug.LogErrorFormat("AggHierarchicallCluster.Split error, maxSize is too small");
                return;
            }

            combines.Clear();

            // 对tree进行前序遍历检测，记录需要合并的节点
            List<NodeST> nodes = new List<NodeST>();
            Stack<NodeST> stack = new Stack<NodeST>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                NodeST node = stack.Pop();
                if (node == null) continue;

                // check node bounds size
                if (node.bounds.size.x <= maxSize && node.bounds.size.y <= maxSize && node.bounds.size.z <= maxSize)
                {
                    nodes.Add(node);
                    continue;
                }

                // if node is leaf
                if (node.IsLeaf())
                {
                    nodes.Add(node);
                    continue;
                }

                if (node.right != null) stack.Push(node.right);
                if (node.left != null) stack.Push(node.left);
            }

            // 遍历每一个节点下需要合并的物体
            for (int i = 0; i < nodes.Count; i++)
            {
                NodeST node = nodes[i];

                // directly add to the groups if node is leaf
                if (node.IsLeaf())
                {
                    if (node.leafGo != null)
                    {
                        combines.Add(new CombineST { gameObjects = new List<GameObject> { node.leafGo }, node = node, multiGoesFlag = node.leafGo.GetComponentsInChildren<MultiTag>().Length > 1 });
                    }
                    continue;
                }

                // pre-traverse the node tree
                List<GameObject> goes = new List<GameObject>();
                Stack<NodeST> tempStack = new Stack<NodeST>();
                tempStack.Push(node);
                while (tempStack.Count > 0)
                {
                    NodeST tempNode = tempStack.Pop();
                    if (tempNode == null) continue;

                    if (tempNode.IsLeaf())
                    {
                        if (tempNode.leafGo != null) goes.Add(tempNode.leafGo);
                        continue;
                    }

                    if (tempNode.right != null) tempStack.Push(tempNode.right);
                    if (tempNode.left != null) tempStack.Push(tempNode.left);
                }
                if (goes.Count > 0) combines.Add(new CombineST { gameObjects = goes, node = node, multiGoesFlag = goes.Count > 1 || goes[0].GetComponentsInChildren<MultiTag>().Length > 1 });
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!enableGizmo) return;

            Gizmos.color = gizmoColor;
            for (int i = 0; i < combines.Count; i++)
            {
                Vector3 center = combines[i].node.bounds.center;
                Vector3 size = combines[i].node.bounds.size;
                if (!combines[i].multiGoesFlag && !showSingle) continue;

                if (wireMode) Gizmos.DrawWireCube(center, size);
                else Gizmos.DrawCube(center, size);
            }
        }
    }
}
