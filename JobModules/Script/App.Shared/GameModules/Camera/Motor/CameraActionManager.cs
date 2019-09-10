using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Core.Utils;
using XmlConfig;


namespace Core.CameraControl.NewMotor
{

    public enum CameraActionType
    {
        Enter,
        Leave,
        End
    }

    public class CameraActionManager
    {
        private BitVector32 EnterAction = new BitVector32();
        private BitVector32 LeaveAction = new BitVector32();

        private Dictionary<int, List<Action<PlayerEntity, ICameraMotorState>>> EnterActionList =
            new Dictionary<int, List<Action<PlayerEntity, ICameraMotorState>>>();

        private Dictionary<int, List<Action<PlayerEntity, ICameraMotorState>>> LeaveActionList =
            new Dictionary<int, List<Action<PlayerEntity, ICameraMotorState>>>();

        private int EnumToMask(int id)
        {
            return 1<<id;
        }

        public int CalcuMotorNum(SubCameraMotorType Mode ,int id)
        {
            int result = 0;
            switch (Mode)
            {
                case SubCameraMotorType.Pose:
                    return id;
                case SubCameraMotorType.Free:
                    return id + (int)ECameraPoseMode.End + 1;
                case SubCameraMotorType.Peek:
                    return id + (int)ECameraPoseMode.End + (int)ECameraFreeMode.End + 2;
                case SubCameraMotorType.View:
                    return id + (int)ECameraPoseMode.End + (int)ECameraFreeMode.End + (int)ECameraPeekMode.End + 3;
                default:
                    return 0;
            }
        }

        public void CopyActionCode(CameraActionType type, int data)
        {
            if (type == CameraActionType.Enter)
                EnterAction = new BitVector32(data);
            else if (type == CameraActionType.Leave)
                LeaveAction = new BitVector32(data);
        }

        public void SetActionCode(CameraActionType actionType, SubCameraMotorType motorType, int id)
        {
            if (actionType == CameraActionType.Enter)
            {
                EnterAction[EnumToMask(CalcuMotorNum(motorType,id))] = true;
            }
            else if (actionType == CameraActionType.Leave)
            {
                LeaveAction[EnumToMask(CalcuMotorNum(motorType,id))] = true;
            }
        }

        public int GetActionCode(CameraActionType type)
        {
            int result = 0;
            if (type == CameraActionType.Enter)
            {
                result = EnterAction.Data;
            }
            else if (type == CameraActionType.Leave)
            {
                result = LeaveAction.Data;
            }
            return result;
        }

        public void ClearActionCode()
        {
            EnterAction = new BitVector32();
            LeaveAction = new BitVector32();
        }

        public void AddAction(CameraActionType actionType, SubCameraMotorType motorType, int motorId,
            Action<PlayerEntity, ICameraMotorState> act)
        {
            if (actionType == CameraActionType.Enter)
            {
                int index = CalcuMotorNum(motorType, motorId);
                if(!EnterActionList.ContainsKey(index))
                    EnterActionList.Add(index,new List<Action<PlayerEntity, ICameraMotorState>>());
                var list = EnterActionList[index];
                list.Add(act);
            }
            else if (actionType == CameraActionType.Leave)
            {
                int index = CalcuMotorNum(motorType, motorId);
                if(!LeaveActionList.ContainsKey(index))
                    LeaveActionList.Add(index,new List<Action<PlayerEntity, ICameraMotorState>>());
                var list = LeaveActionList[index];
                list.Add(act);
            }
        }

        public void OnAction(PlayerEntity player, ICameraMotorState state)
        {
            for (int i = 1; i < 32; i++)
            {
                if (JudgeAction(CameraActionType.Enter, i))// && 
                    if (EnterActionList.ContainsKey(i))
                    {
                        foreach (var act in EnterActionList[i])
                        {
                            act(player, state);
                        }
//                        EnterActionList[i](player, state);
                    }
                if (JudgeAction(CameraActionType.Leave, i))// &&
                    if (LeaveActionList.ContainsKey(i))
                    {
                        foreach (var act in LeaveActionList[i])
                        {
                            act(player, state);
                        }           
                    }
//                        LeaveActionList[i](player, state);
            }
        }

        public bool TestChangeCurFrame(ICameraMotorState state, SubCameraMotorType type, int modeId)
        {
            var index = CalcuMotorNum(type, modeId);
            return EnterAction[EnumToMask(index)];
        }

        private bool JudgeAction(CameraActionType type, int id)
        {
            if (type == CameraActionType.Enter)
            {
                return EnterAction[EnumToMask(id)];
            }
            else if (type == CameraActionType.Leave)
            {
                return LeaveAction[EnumToMask(id)];
            }

            return false;
        }
    }

}
