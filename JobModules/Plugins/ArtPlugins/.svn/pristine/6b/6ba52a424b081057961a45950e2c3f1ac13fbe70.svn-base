using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SplineTools
{
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : Editor
    {
        [MenuItem("GameObject/3D Object/Spline")]
        private static void CreateSpline(MenuCommand command)
        {
            GameObject newGo = new GameObject("Spline", typeof(Spline));
            Transform tr = newGo.transform;

            Transform parent = null;
            if (command.context != null)
            {
                parent = (command.context as GameObject).transform;
            }

            if (parent != null)
            {
                Undo.SetTransformParent(tr, parent, "Reparenting Spline");
                tr.localPosition = Vector3.zero;
                tr.localScale = Vector3.zero;
                tr.localRotation = Quaternion.identity;
                newGo.layer = parent.gameObject.layer;
            }
            else if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.MoveToView(tr);
            }

            Selection.activeGameObject = newGo;
        }

        [MenuItem("CONTEXT/Spline/RebuildCurves")]
        private static void RebuildCurves(MenuCommand command)
        {
            if (command.context == null) return;

            Spline spline = command.context as Spline;
            spline.RebuildCurves();
        }

        private Spline spline;

        private SerializedProperty nodesProp;
        private SerializedProperty alwaysShowProp;
        private SerializedProperty gizmoRadiusProp;
        private SerializedProperty altKeyProp;
        private SerializedProperty distanceProp;
        private SerializedProperty directionProp;

        private int selectNodeIndex = -1;

        private string helpMsg = null;

        private GUIStyle _nodeInfoStyle = null;
        private GUIStyle nodeInfoStyle
        {
            get
            {
                if (_nodeInfoStyle == null)
                {
                    _nodeInfoStyle = new GUIStyle(EditorStyles.miniTextField);
                    _nodeInfoStyle.alignment = TextAnchor.MiddleCenter;
                }
                return _nodeInfoStyle;
            }
        }

        private bool isExpandCurves = false;

        private bool addNodeToEnd = true;

        private void OnEnable()
        {
            spline = target as Spline;
            var curves = spline.GetCurves();
            if (curves.Count == 0) spline.RebuildCurves();

            nodesProp = serializedObject.FindProperty("nodes");
            alwaysShowProp = serializedObject.FindProperty("alwaysShow");
            gizmoRadiusProp = serializedObject.FindProperty("gizmoRadius");
            altKeyProp = serializedObject.FindProperty("altKey");
            distanceProp = serializedObject.FindProperty("distance");
            directionProp = serializedObject.FindProperty("direction");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Node Front"))
            {
                var nodes = spline.GetNodes();
                SplineNode firstNode = nodes[0];
                SplineNode secondNode = nodes[1];
                Vector3 firstNodePos = firstNode.GetPosition();
                Vector3 secondNodePos = secondNode.GetPosition();
                firstNodePos += (firstNodePos - secondNodePos);
                Vector3 preDir = firstNodePos + Vector3.left + Vector3.forward;
                Vector3 nextDir = firstNodePos + Vector3.right + Vector3.forward;
                SplineNode newNode = new SplineNode(firstNodePos, preDir, nextDir);
                spline.AddNode(newNode, false);

                EditorUtility.SetDirty(spline);
            }
            if (GUILayout.Button("Insert Node"))
            {
                if (selectNodeIndex == -1 || selectNodeIndex % 10 == 0)
                {
                    helpMsg = "please select the curve that needs insert a node";
                }
                else
                {
                    helpMsg = null;

                    int index = selectNodeIndex / 10;
                    int offset = selectNodeIndex % 10;

                    int insertIndex = index;
                    if (offset == 2) insertIndex = index + 1;       // next direction

                    int curveIndex = index;
                    if (offset == 1) curveIndex = index - 1;        // pre direction

                    var curves = spline.GetCurves();
                    Vector3 pos = curves[curveIndex].GetLocation(0.5f);
                    Vector3 preDir = pos + Vector3.left;
                    Vector3 nextDir = pos + Vector3.right;
                    SplineNode newNode = new SplineNode(pos, preDir, nextDir);
                    spline.InsertNode(insertIndex, newNode);

                    EditorUtility.SetDirty(spline);
                }
            }
            if (GUILayout.Button("Delete Node"))
            {
                var nodes = spline.GetNodes();
                if (nodes.Count <= 2)
                {
                    helpMsg = "spline node count can't be less than 2";
                }
                else if (selectNodeIndex == -1)
                {
                    helpMsg = "please select the node that needs to be deleted";
                }
                else if (selectNodeIndex % 10 != 0)             // pre direction or next direction
                {
                    helpMsg = "please select the node that needs to be deleted";
                }
                else                                            // delete the node
                {
                    helpMsg = null;
                    int index = selectNodeIndex / 10;
                    SplineNode node = nodes[index];
                    spline.RemoveNode(node);

                    if (selectNodeIndex == 0)
                        selectNodeIndex += 10;
                    else
                        selectNodeIndex -= 10;

                    EditorUtility.SetDirty(spline);
                }
            }
            if (GUILayout.Button("Add Node Back"))
            {
                var nodes = spline.GetNodes();
                SplineNode lastNode = nodes[nodes.Count - 1];
                SplineNode preLastNode = nodes[nodes.Count - 2];
                Vector3 lastNodePos = lastNode.GetPosition();
                Vector3 preLastNodePos = preLastNode.GetPosition();
                lastNodePos += (lastNodePos - preLastNodePos);
                Vector3 preDir = lastNodePos + Vector3.left + Vector3.forward;
                Vector3 nextDir = lastNodePos + Vector3.right + Vector3.forward;
                SplineNode newNode = new SplineNode(lastNodePos, preDir, nextDir);
                spline.AddNode(newNode);

                EditorUtility.SetDirty(spline);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Align With First Curve"))
            {
                List<Vector3> curveDirs = new List<Vector3>();
                var nodes = spline.GetNodes();
                for (int i = 2; i < nodes.Count; i++)
                {
                    Vector3 delta = nodes[i].GetPosition() - nodes[i - 1].GetPosition();
                    curveDirs.Add(delta.normalized);
                }

                // caculate new position
                float dis = Vector3.Distance(spline.GetNodes()[1].GetPosition(), spline.GetNodes()[0].GetPosition());
                SerializedProperty preNodeProp = nodesProp.GetArrayElementAtIndex(1);
                SerializedProperty preNodePosProp = preNodeProp.FindPropertyRelative("position");
                Vector3 preNodePos = preNodePosProp.vector3Value;
                for (int i = 2; i < nodes.Count; i++)
                {
                    SerializedProperty nodeProp = nodesProp.GetArrayElementAtIndex(i);
                    SerializedProperty nodePosProp = nodeProp.FindPropertyRelative("position");
                    SerializedProperty nodePreDirProp = nodeProp.FindPropertyRelative("preDirection");

                    Vector3 preDirDelta = nodePreDirProp.vector3Value - nodePosProp.vector3Value;
                    nodePosProp.vector3Value = preNodePos + curveDirs[i - 2] * dis;
                    nodePreDirProp.vector3Value = nodePosProp.vector3Value + preDirDelta;

                    preNodePos = nodePosProp.vector3Value;
                }

                serializedObject.ApplyModifiedProperties();

                // update curves
                for (int i = 2; i < nodes.Count; i++)
                {
                    SplineNode node = spline.GetNodes()[i];
                    if (node.changed != null) node.changed.Invoke();
                }
            }

            if (!string.IsNullOrEmpty(helpMsg))
            {
                EditorGUILayout.HelpBox(helpMsg, MessageType.Warning);
            }

            // nodes
            EditorGUILayout.PropertyField(nodesProp);
            if (nodesProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(true);
                for (int i = 0; i < nodesProp.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(nodesProp.GetArrayElementAtIndex(i), new GUIContent("node" + i), true);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }

            // curves 
            EditorGUILayout.BeginHorizontal();
            isExpandCurves = EditorGUILayout.Foldout(isExpandCurves, "Curves");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("All Splines", GUILayout.MaxWidth(80f)))
            {
                List<int> updateNodes = new List<int>();
                for (int i = 0; i < nodesProp.arraySize - 1; i++)
                {
                    SerializedProperty nodeProp = nodesProp.GetArrayElementAtIndex(i);
                    SerializedProperty curveTypeProp = nodeProp.FindPropertyRelative("curveType");
                    if (curveTypeProp.enumValueIndex != (int)CurveType.Spline)
                    {
                        curveTypeProp.enumValueIndex = (int)CurveType.Spline;
                        updateNodes.Add(i);
                    }
                }
                serializedObject.ApplyModifiedProperties();
                foreach (var nodeIndex in updateNodes)
                {
                    SplineNode node = spline.GetNodes()[nodeIndex];
                    if (node.changed != null) node.changed.Invoke();
                }
            }
            if (GUILayout.Button("All Lines", GUILayout.MaxWidth(80f)))
            {
                List<int> updateNodes = new List<int>();
                for (int i = 0; i < nodesProp.arraySize - 1; i++)
                {
                    SerializedProperty nodeProp = nodesProp.GetArrayElementAtIndex(i);
                    SerializedProperty curveTypeProp = nodeProp.FindPropertyRelative("curveType");
                    if (curveTypeProp.enumValueIndex != (int)CurveType.Line)
                    {
                        curveTypeProp.enumValueIndex = (int)CurveType.Line;
                        updateNodes.Add(i);
                    }
                }
                serializedObject.ApplyModifiedProperties();
                foreach (var nodeIndex in updateNodes)
                {
                    SplineNode node = spline.GetNodes()[nodeIndex];
                    if (node.changed != null) node.changed.Invoke();
                }
            }
            EditorGUILayout.EndHorizontal();
            if (isExpandCurves)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < nodesProp.arraySize - 1; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    SerializedProperty nodeProp = nodesProp.GetArrayElementAtIndex(i);
                    SerializedProperty curveTypeProp = nodeProp.FindPropertyRelative("curveType");
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(curveTypeProp, new GUIContent("curve" + i.ToString()));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        SplineNode node = spline.GetNodes()[i];
                        if (node.changed != null) node.changed.Invoke();
                    }
                    if (curveTypeProp.enumValueIndex == (int)CurveType.Spline)
                    {
                        if (GUILayout.Button("Line", GUILayout.MaxWidth(50f)))
                        {
                            SerializedProperty firstNodeProp = nodesProp.GetArrayElementAtIndex(i);
                            SerializedProperty firstPosProp = firstNodeProp.FindPropertyRelative("position");
                            SerializedProperty firstNextDirProp = firstNodeProp.FindPropertyRelative("nextDirection");
                            SerializedProperty twoNodeProp = nodesProp.GetArrayElementAtIndex(i + 1);
                            SerializedProperty twoPosProp = twoNodeProp.FindPropertyRelative("position");
                            SerializedProperty twoPreDirProp = twoNodeProp.FindPropertyRelative("preDirection");
                            Vector3 delta = twoPosProp.vector3Value - firstPosProp.vector3Value;
                            firstNextDirProp.vector3Value = firstPosProp.vector3Value + delta * 0.2f;
                            twoPreDirProp.vector3Value = twoPosProp.vector3Value - delta * 0.2f;

                            serializedObject.ApplyModifiedProperties();
                            SplineNode node = spline.GetNodes()[i];
                            if (node.changed != null) node.changed.Invoke();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }

            addNodeToEnd = EditorGUILayout.Toggle("Add Node To End", addNodeToEnd);
            gizmoRadiusProp.floatValue = EditorGUILayout.Slider("Gizmo Radius", gizmoRadiusProp.floatValue, 0f, 1f);
            EditorGUILayout.PropertyField(alwaysShowProp);
            EditorGUILayout.PropertyField(altKeyProp);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!altKeyProp.boolValue);
            EditorGUILayout.PropertyField(distanceProp);
            EditorGUILayout.PropertyField(directionProp);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            // respond to Undo
            if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
            {
                spline.RebuildCurves();
                EditorUtility.SetDirty(spline);
            }

            // draw
            Matrix4x4 oldMatrix = Handles.matrix;
            Handles.matrix = spline.transform.localToWorldMatrix;
            for (int i = 0; i < nodesProp.arraySize; i++)
            {
                var nodeProp = nodesProp.GetArrayElementAtIndex(i);
                Vector3 position = nodeProp.FindPropertyRelative("position").vector3Value;
                Vector3 preDir = nodeProp.FindPropertyRelative("preDirection").vector3Value;
                Vector3 nextDir = nodeProp.FindPropertyRelative("nextDirection").vector3Value;

                // when click draw position handle
                if (Handles.Button(position, Quaternion.identity, 0f, gizmoRadiusProp.floatValue, Handles.DotHandleCap))
                {
                    selectNodeIndex = i * 10;
                }
                if (Handles.Button(preDir, Quaternion.identity, 0f, gizmoRadiusProp.floatValue, Handles.DotHandleCap))
                {
                    selectNodeIndex = i * 10 + 1;
                }
                if (Handles.Button(nextDir, Quaternion.identity, 0f, gizmoRadiusProp.floatValue, Handles.DotHandleCap))
                {
                    selectNodeIndex = i * 10 + 2;
                }
            }
            if (selectNodeIndex != -1)
            {
                int index = selectNodeIndex / 10;
                int offset = selectNodeIndex % 10;
                var nodeProp = nodesProp.GetArrayElementAtIndex(index);

                SerializedProperty posProp = null;
                if (offset == 0) posProp = nodeProp.FindPropertyRelative("position");
                else if (offset == 1) posProp = nodeProp.FindPropertyRelative("preDirection");
                else posProp = nodeProp.FindPropertyRelative("nextDirection");

                EditorGUI.BeginChangeCheck();
                Vector3 oldValue = posProp.vector3Value;
                posProp.vector3Value = Handles.PositionHandle(posProp.vector3Value, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    if (selectNodeIndex % 10 == 0)                              // node position
                    {
                        Vector3 delta = posProp.vector3Value - oldValue;

                        SerializedProperty preDirProp = nodeProp.FindPropertyRelative("preDirection");
                        SerializedProperty nextDirProp = nodeProp.FindPropertyRelative("nextDirection");
                        preDirProp.vector3Value += delta;
                        nextDirProp.vector3Value += delta;
                    }

                    serializedObject.ApplyModifiedProperties();

                    // spline node change event
                    SplineNode node = spline.GetNodes()[index];
                    if (node.changed != null) node.changed.Invoke();
                }

                if (offset == 1) spline.HighlightCurve(index - 1);
                else if (offset == 2) spline.HighlightCurve(index);
                else spline.HighlightCurve(-1);
            }
            else
            {
                spline.HighlightCurve(-1);
            }
            Handles.matrix = oldMatrix;

            // press alt to display nodes info
            if (altKeyProp.boolValue && Event.current.alt)
            {
                // nodes name
                for (int i = 0; i < nodesProp.arraySize; i++)
                {
                    var nodeProp = nodesProp.GetArrayElementAtIndex(i);
                    Vector3 position = nodeProp.FindPropertyRelative("position").vector3Value;
                    Vector3 preDir = nodeProp.FindPropertyRelative("preDirection").vector3Value;
                    Vector3 nextDir = nodeProp.FindPropertyRelative("nextDirection").vector3Value;

                    int kLimit = directionProp.boolValue ? 3 : 1;
                    for (int k = 0; k < kLimit; k++)
                    {
                        string nodeName = null;
                        Vector3 pos;
                        switch (k)
                        {
                            case 0:
                                nodeName = string.Format("node{0}", i);
                                pos = spline.transform.TransformPoint(position);
                                break;
                            case 1:
                                nodeName = string.Format("node{0} pre", i);
                                pos = spline.transform.TransformPoint(preDir);
                                break;
                            default:
                                nodeName = string.Format("node{0} next", i);
                                pos = spline.transform.TransformPoint(nextDir);
                                break;
                        }
                        Handles.Label(pos, nodeName, nodeInfoStyle);
                    }
                }

                // curve length
                if (distanceProp.boolValue)
                {
                    var curves = spline.GetCurves();
                    for (int i = 0; i < curves.Count; i++)
                    {
                        var curve = curves[i];
                        Vector3 position = curve.GetLocation(0.5f);
                        float length = curve.GetLength();
                        position = spline.transform.TransformPoint(position);
                        Handles.Label(position, length.ToString(), nodeInfoStyle);
                    }

                    // spline length
                    {
                        float length = spline.GetLength();
                        Vector3 position = spline.transform.position;
                        Handles.Label(position, length.ToString(), nodeInfoStyle);
                    }
                }
            }

            // press ctrl key to add new node
            if (Event.current.control)
            {
                // left button to add click
                if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        Vector3 pos = spline.transform.InverseTransformPoint(hit.point);
                        Vector3 preDir = pos + Vector3.left + Vector3.forward;
                        Vector3 nextDir = pos + Vector3.right + Vector3.forward;
                        SplineNode newNode = new SplineNode(pos, preDir, nextDir);
                        spline.AddNode(newNode, addNodeToEnd);
                    }

                    // ignore select scene object
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();
                }
            }

            // press end key to align the node
            if (selectNodeIndex != -1 && Event.current.keyCode == KeyCode.End && Event.current.type == EventType.KeyUp)
            {
                int index = selectNodeIndex / 10;
                int offset = selectNodeIndex % 10;
                var nodeProp = nodesProp.GetArrayElementAtIndex(index);

                SerializedProperty posProp = null;
                if (offset == 0) posProp = nodeProp.FindPropertyRelative("position");
                else if (offset == 1) posProp = nodeProp.FindPropertyRelative("preDirection");
                else posProp = nodeProp.FindPropertyRelative("nextDirection");

                Vector3 origin = spline.transform.TransformPoint(posProp.vector3Value);
                Ray ray = new Ray(origin, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 oldValue = posProp.vector3Value;
                    Vector3 newValue = spline.transform.InverseTransformPoint(hit.point);
                    posProp.vector3Value = newValue;

                    if (selectNodeIndex % 10 == 0)                              // node position
                    {
                        Vector3 delta = posProp.vector3Value - oldValue;
                        SerializedProperty preDirProp = nodeProp.FindPropertyRelative("preDirection");
                        SerializedProperty nextDirProp = nodeProp.FindPropertyRelative("nextDirection");
                        preDirProp.vector3Value += delta;
                        nextDirProp.vector3Value += delta;
                    }

                    serializedObject.ApplyModifiedProperties();

                    // spline node change event
                    SplineNode node = spline.GetNodes()[index];
                    if (node.changed != null) node.changed.Invoke();
                }
            }
        }
    }
}