using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using App.Shared.GameModules.Vehicle;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CharacterController))]
public class MyFirstPerson : MonoBehaviour
{

#if UNITY_EDITOR
    [MenuItem("CONTEXT/PlayerScript/AddMyFirstPerson")]
    private static void AddMyFirstPerson(MenuCommand command)
    {
        if (command.context == null) return;
        MonoBehaviour mb = command.context as MonoBehaviour;
        if (mb == null) return;
        MyFirstPerson fp = mb.GetComponent<MyFirstPerson>();
        if (fp == null) fp = mb.gameObject.AddComponent<MyFirstPerson>();
    }
#endif

    // public enum RoleState
    // {
    //     Walk,
    //     Fly,
    // }

    // private new Rigidbody rigidbody;
    // private CharacterController controller;
    // private bool cursorLocked;

    // private RoleState roleState = RoleState.Walk;

    // public bool ignoreCollide = false;

    // // fly state
    // public float flySpeed = 100f;

    // // walk state
    // public float walkSpeed = 10f;
    // public float jumpSpeed = 10f;
    // public float gravityMultiplier = 2f;
    // public bool isJumping;

    // private Vector3 moveDir = Vector3.zero;

    // // mouse move
    // public float xSensitivity = 40f;
    // public float ySensitivity = 40f;

    // whole prefab
    public bool putOnTerrainWhole = false;
    public bool pivotOnCenterWhole = false;
    public GameObject modelWhole;

    // half prefab
    public bool putOnTerrainHalf = false;
    public bool pivotOnCenterHalf = false;
    public GameObject modelHalf;


    // private void Awake()
    // {
    //     rigidbody = GetComponent<Rigidbody>();
    //     controller = GetComponent<CharacterController>();
    // }

    // private void OnEnable()
    // {
    //     SetCursorState(false);
    // }

    // private void Update()
    // {
    //     // exit from cursor lock state
    //     if (Input.GetKeyUp(KeyCode.Escape))
    //     {
    //         SetCursorState(false);
    //     }
    //     else if (Input.GetMouseButtonUp(0))
    //     {
    //         SetCursorState(true);
    //     }

    //     // walk and fly switch
    //     RoleState oldState = roleState;
    //     if (Input.GetKeyUp(KeyCode.F1))
    //     {
    //         if (oldState == RoleState.Walk)
    //             roleState = RoleState.Fly;
    //         else
    //             roleState = RoleState.Walk;
    //     }
    //     if (roleState != oldState)
    //     {
    //         isJumping = false;
    //         moveDir = Vector3.zero;
    //     }

    //     // rotation
    //     Vector3 oldRot = transform.eulerAngles;
    //     float rotY = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
    //     float rotX = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
    //     oldRot += new Vector3(-rotX, rotY, 0f);
    //     if (oldRot.x > 180f) oldRot.x -= 360f;
    //     oldRot = new Vector3(Mathf.Clamp(oldRot.x, -70f, 70f), oldRot.y, oldRot.z);
    //     transform.eulerAngles = oldRot;

    //     // move 
    //     float moveX = Input.GetAxis("Horizontal");
    //     float moveY = Input.GetAxis("Vertical");
    //     float speed = roleState == RoleState.Walk ? walkSpeed : flySpeed;
    //     Vector3 moveDelta = new Vector3(moveX * speed, 0f, moveY * speed);
    //     moveDelta = transform.TransformDirection(moveDelta);

    //     // jump
    //     bool needJump = false;
    //     if (Input.GetKeyUp(KeyCode.Space))
    //     {
    //         if (roleState == RoleState.Walk && !isJumping)
    //         {
    //             needJump = true;
    //             isJumping = true;
    //             moveDelta.y = jumpSpeed;
    //         }
    //     }
    //     if (!needJump && roleState == RoleState.Walk)
    //     {
    //         moveDelta.y = moveDir.y;
    //     }

    //     // fall to the ground
    //     if (roleState == RoleState.Walk && isJumping && controller.isGrounded)
    //     {
    //         isJumping = false;
    //     }

    //     if (roleState == RoleState.Walk)
    //     {
    //         moveDelta += Physics.gravity * gravityMultiplier * Time.deltaTime;
    //     }
    //     moveDir = moveDelta;
    //     controller.Move(moveDir * Time.deltaTime);

    //     // put the model
    //     if (Input.GetKeyUp(KeyCode.M))
    //     {
    //         PutOneModel(false);
    //     }
    //     else if (Input.GetKeyUp(KeyCode.N))
    //     {
    //         PutOneModel(true);
    //     }
    // }

    // private void OnDisable()
    // {
    //     SetCursorState(false);
    // }

    // private void OnControllerColliderHit(ControllerColliderHit hit)
    // {
    //     if (!ignoreCollide) return;

    //     if (hit.collider == null) return;

    //     Terrain terrain = hit.collider.GetComponent<Terrain>();
    //     if (terrain != null) return;

    //     Physics.IgnoreCollision(controller, hit.collider, true);
    // }

    // private void SetCursorState(bool needLock)
    // {
    //     cursorLocked = needLock;

    //     Cursor.visible = !cursorLocked;
    //     Cursor.lockState = needLock ? CursorLockMode.Locked : CursorLockMode.None;
    // }

    private void Update()
    {
        // put the model
        if (Input.GetKeyUp(KeyCode.P))
        {
            PutOneModel(false);
        }
        else if (Input.GetKeyUp(KeyCode.O))
        {
            PutOneModel(true);
        }
    }

    private void PutOneModel(bool half)
    {
        GameObject model = half ? modelHalf : modelWhole;
        if (model == null)
        {
            Debug.LogError("PutOneModel error,model is null");
            return;
        }

        CharacterController controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("PutOneModel error, controller is null");
            return;
        }

        GameObject go = Instantiate<GameObject>(model, Vector3.zero, Quaternion.identity);
        go.transform.position = transform.position;
        go.transform.localScale = half ? modelHalf.transform.localScale : modelWhole.transform.localScale;

        Collider[] cs = go.GetComponentsInChildren<Collider>();
        foreach (var c in cs)
        {
            if (c == null) continue;
            Physics.IgnoreCollision(controller, c, true);
        }

        Ray ray = new Ray(go.transform.position, Vector3.down);
        var hits = Physics.RaycastAll(ray);
        if (hits.Length > 0)
        {
            int index = -1;
            float minDis = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                GameObject hitGo = hit.collider.gameObject;
                if (hitGo == null || hitGo == gameObject || hitGo == go) continue;

                if (hit.distance < minDis)
                {
                    // if (!putOnTerrainWhole)
                    if (!(half ? putOnTerrainHalf : putOnTerrainWhole))
                    {
                        minDis = hit.distance;
                        index = i;
                    }
                    else
                    {
                        Terrain t = hitGo.GetComponent<Terrain>();
                        if (t != null)
                        {
                            minDis = hit.distance;
                            index = i;
                        }
                    }
                }
            }
            if (index != -1)
            {
                var hit = hits[index];
                Vector3 pos = (half ? pivotOnCenterHalf : pivotOnCenterWhole) ? hit.point + new Vector3(0f, go.transform.localScale.y / 2f, 0f) : hit.point;
                go.transform.localPosition = pos;
            }
        }
    }

}
