using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState;
using Core.CharacterState.Posture;
using Core.Compare;
using Core.Components;
using Core.EntityComponent;
using Core;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
    [Player]
    
    public class StateComponent : AbstractStateComponent,IUpdateComponent
    {
        public virtual int GetComponentId()
        {
            return (int)EComponentIds.PlayerFsm;
        }

        public void CopyFrom(object rightComponent)
        {
           base.CopyFrom(rightComponent);
        }
    }

    public abstract class AbstractStateComponent
    {
        [DontInitilize][NetworkProperty] public short PostureStateId;
        [DontInitilize][NetworkProperty] public int PostureStateProgress;
        [DontInitilize][NetworkProperty] public short PostureTransitionId;
        [DontInitilize][NetworkProperty] public float PostureTransitionProgress;
        [DontInitilize][NetworkProperty] public short LeanStateId;
        [DontInitilize][NetworkProperty] public int LeanStateProgress;
        [DontInitilize][NetworkProperty] public short LeanTransitionId;
        [DontInitilize][NetworkProperty] public float LeanTransitionProgress;
        [DontInitilize][NetworkProperty] public short MovementStateId;
        [DontInitilize][NetworkProperty] public int MovementStateProgress;
        [DontInitilize][NetworkProperty] public short MovementTransitionId;
        [DontInitilize][NetworkProperty] public float MovementTransitionProgress;
        [DontInitilize][NetworkProperty] public short ActionStateId;
        [DontInitilize][NetworkProperty] public int ActionStateProgress;
        [DontInitilize][NetworkProperty] public short ActionTransitionId;
        [DontInitilize][NetworkProperty] public float ActionTransitionProgress;
        [DontInitilize] [NetworkProperty] public short KeepStateId;
        [DontInitilize] [NetworkProperty] public int KeepStateProgress;
        [DontInitilize] [NetworkProperty] public short KeepTransitionId;
        [DontInitilize] [NetworkProperty] public float KeepTransitionProgress;


        public virtual void CopyFrom(object rightComponent)
        {
            var right = rightComponent as AbstractStateComponent;
            if (right != null)
            {
                PostureStateId = right.PostureStateId;
                PostureStateProgress = right.PostureStateProgress;
                PostureTransitionId = right.PostureTransitionId;
                PostureTransitionProgress = right.PostureTransitionProgress;
                LeanStateId = right.LeanStateId;
                LeanStateProgress = right.LeanStateProgress;
                LeanTransitionId = right.LeanTransitionId;
                LeanTransitionProgress = right.LeanTransitionProgress;
                MovementStateId = right.MovementStateId;
                MovementStateProgress = right.MovementStateProgress;
                MovementTransitionId = right.MovementTransitionId;
                MovementTransitionProgress = right.MovementTransitionProgress;
                ActionStateId = right.ActionStateId;
                ActionStateProgress = right.ActionStateProgress;
                ActionTransitionId = right.ActionTransitionId;
                ActionTransitionProgress = right.ActionTransitionProgress;
                KeepStateId = right.KeepStateId;
                KeepStateProgress = right.KeepStateProgress;
                KeepTransitionId = right.KeepTransitionId;
                KeepTransitionProgress = right.KeepTransitionProgress;
            }
        }

        public virtual bool IsApproximatelyEqual(object right)
        {
            var rightObj = right as AbstractStateComponent;
            if (rightObj != null)
            {
                return CompareUtility.IsApproximatelyEqual(PostureStateId, rightObj.PostureStateId)
                       && CompareUtility.IsApproximatelyEqual(PostureStateProgress, rightObj.PostureStateProgress)
                       && CompareUtility.IsApproximatelyEqual(PostureTransitionId, rightObj.PostureTransitionId)
                       && CompareUtility.IsApproximatelyEqual(PostureTransitionProgress, rightObj.PostureTransitionProgress)
                       && CompareUtility.IsApproximatelyEqual(LeanStateId, rightObj.LeanStateId)
                       && CompareUtility.IsApproximatelyEqual(LeanStateProgress, rightObj.LeanStateProgress)
                       && CompareUtility.IsApproximatelyEqual(LeanTransitionId, rightObj.LeanTransitionId)
                       && CompareUtility.IsApproximatelyEqual(LeanTransitionProgress, rightObj.LeanTransitionProgress)
                       && CompareUtility.IsApproximatelyEqual(MovementStateId, rightObj.MovementStateId)
                       && CompareUtility.IsApproximatelyEqual(MovementStateProgress, rightObj.MovementStateProgress)
                       && CompareUtility.IsApproximatelyEqual(MovementTransitionId, rightObj.MovementTransitionId)
                       && CompareUtility.IsApproximatelyEqual(MovementTransitionProgress, rightObj.MovementTransitionProgress)
                       && CompareUtility.IsApproximatelyEqual(ActionStateId, rightObj.ActionStateId)
                       && CompareUtility.IsApproximatelyEqual(ActionStateProgress, rightObj.ActionStateProgress)
                       && CompareUtility.IsApproximatelyEqual(ActionTransitionId, rightObj.ActionTransitionId)
                       && CompareUtility.IsApproximatelyEqual(ActionTransitionProgress, rightObj.ActionTransitionProgress)
                       && CompareUtility.IsApproximatelyEqual(KeepStateId, rightObj.KeepStateId)
                       && CompareUtility.IsApproximatelyEqual(KeepStateProgress, rightObj.KeepStateProgress)
                       && CompareUtility.IsApproximatelyEqual(KeepTransitionId, rightObj.KeepTransitionId)
                       && CompareUtility.IsApproximatelyEqual(KeepTransitionProgress, rightObj.KeepTransitionProgress)
                       ;
            }
            return false;
        }
        
        
        public override string ToString()
        {
            return string.Format("PostureStateId: {0}, PostureStateProgress: {1}, PostureTransitionId: {2}, PostureTransitionProgress: {3}\nLeanStateId: {4}, LeanStateProgress: {5}, LeanTransitionId: {6}, LeanTransitionProgress: {7}\nMovementStateId: {8}, MovementStateProgress: {9}, MovementTransitionId: {10}, MovementTransitionProgress: {11}\n ActionStateId: {12}, ActionStateProgress: {13}, ActionTransitionId: {14}, ActionTransitionProgress: {15}\nKeepStateId: {16}, KeepStateProgress: {17}, KeepTransitionId: {18}, KeepTransitionProgress: {19}", PostureStateId, PostureStateProgress, PostureTransitionId, PostureTransitionProgress, LeanStateId, LeanStateProgress, LeanTransitionId, LeanTransitionProgress, MovementStateId, MovementStateProgress, MovementTransitionId, MovementTransitionProgress, ActionStateId, ActionStateProgress, ActionTransitionId, ActionTransitionProgress, KeepStateId, KeepStateProgress, KeepTransitionId, KeepTransitionProgress);
        }
    }

    [Player]
    
    public class StateBeforeComponent : AbstractStateComponent,IUpdateComponent
    {
        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerFsmBefore;
        }

        public void CopyFrom(object rightComponent)
        {
            base.CopyFrom(rightComponent);
        }
    }

    public abstract class AbstractStateInterVar
    {
        
        [DontInitilize][NetworkProperty] public float VerticalValue;
        [DontInitilize][NetworkProperty] public float HorizontalValue;
        [DontInitilize][NetworkProperty] public float UpDownValue;
        

        public virtual void CopyFrom(object rightComponent)
        {
            var right = rightComponent as AbstractStateInterVar;
            VerticalValue = right.VerticalValue;
            HorizontalValue = right.HorizontalValue;
            UpDownValue = right.UpDownValue;
        }

        public virtual bool IsApproximatelyEqual(object right)
        {
            var rightObj = right as AbstractStateInterVar;
            return 
                   CompareUtility.IsApproximatelyEqual(VerticalValue, rightObj.VerticalValue)
                   && CompareUtility.IsApproximatelyEqual(HorizontalValue, rightObj.HorizontalValue)
                   && CompareUtility.IsApproximatelyEqual(UpDownValue, rightObj.UpDownValue)
                ;
        }

        public override string ToString()
        {
            return string.Format("VerticalValue: {0}, HorizontalValue: {1}, UpDownValue: {2}\n", VerticalValue, HorizontalValue, UpDownValue);
        }
    }
    
    [Player]
    
    public class StateInterVar : AbstractStateInterVar, IUpdateComponent
    {

        [NetworkProperty] public StateInterCommands StateInterCommands;
        [NetworkProperty] public StateInterCommands AnimationCallbackCommands;
        [NetworkProperty] public UnityAnimationEventCommands FirstPersonAnimationEventCallBack;
        [NetworkProperty] public UnityAnimationEventCommands ThirdPersonAnimationEventCallBack;
        [NetworkProperty, DontInitilize] public bool IsJumpForSync;
        
     

//        bool IComparableComponent.IsApproximatelyEqual(object right)
//        {
//            return IsApproximatelyEqual(right);
//        }

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerFsmOut;
        }

        public override void CopyFrom(object rightComponent)
        {
            var right = rightComponent as StateInterVar;
            InitFileds();
            StateInterCommands.CopyFrom(right.StateInterCommands);
            AnimationCallbackCommands.CopyFrom(right.AnimationCallbackCommands);
            FirstPersonAnimationEventCallBack.CopyFrom(right.FirstPersonAnimationEventCallBack);
            ThirdPersonAnimationEventCallBack.CopyFrom(right.ThirdPersonAnimationEventCallBack);
            IsJumpForSync = right.IsJumpForSync;
            base.CopyFrom(rightComponent);
        }

        private void InitFileds()
        {
            if (StateInterCommands == null)
            {
                StateInterCommands = new StateInterCommands();
            }
            
            if (AnimationCallbackCommands == null)
            {
                AnimationCallbackCommands = new StateInterCommands();
            }
            
            if (FirstPersonAnimationEventCallBack == null)
            {
                FirstPersonAnimationEventCallBack = new UnityAnimationEventCommands();
            }
            
            if (ThirdPersonAnimationEventCallBack == null)
            {
                ThirdPersonAnimationEventCallBack = new UnityAnimationEventCommands();
            }
            
        }

        public override bool IsApproximatelyEqual(object right)
        {
            var rig = right as StateInterVar;
            return AnimationCallbackCommands.Equals(rig.AnimationCallbackCommands) 
                   && StateInterCommands.Equals(rig.StateInterCommands) 
                   && FirstPersonAnimationEventCallBack.Equals(rig.FirstPersonAnimationEventCallBack)
                   && ThirdPersonAnimationEventCallBack.Equals(rig.ThirdPersonAnimationEventCallBack)
                   && base.IsApproximatelyEqual(right)
                   && CompareUtility.IsApproximatelyEqual(IsJumpForSync, rig.IsJumpForSync)
                ;
        }

        public void Reset()
        {
            StateInterCommands.Reset();
            AnimationCallbackCommands.Reset();
            FirstPersonAnimationEventCallBack.Reset();
            ThirdPersonAnimationEventCallBack.Reset();
        }
        
        public override string ToString()
        {
            return string.Format("{0}, StateInterCommands: {1}, AnimationCallbackCommands: {2}, FirstPersonAnimationEventCallBack: {3}, ThirdPersonAnimationEventCallBack: {4}, IsJumpForSync: {5}", base.ToString(), StateInterCommands, AnimationCallbackCommands, FirstPersonAnimationEventCallBack, ThirdPersonAnimationEventCallBack, IsJumpForSync);
        }

        public string PrintCommandsCount()
        {
            return string.Format("total:{0}" + 
                "StateInterCommands: {1}, AnimationCallbackCommands: {2}, FirstPersonAnimationEventCallBack: {3}, ThirdPersonAnimationEventCallBack: {4}",
                StateInterCommands.Commands.Count+AnimationCallbackCommands.Commands.Count+FirstPersonAnimationEventCallBack.Commands.Count+ThirdPersonAnimationEventCallBack.Commands.Count,
                StateInterCommands.Commands.Count,
                AnimationCallbackCommands.Commands.Count,
                FirstPersonAnimationEventCallBack.Commands.Count,
                ThirdPersonAnimationEventCallBack.Commands.Count
            );
        }
    }

    [Player]
    
    public class StateInterVarBefore : AbstractStateInterVar, IUpdateComponent
    {
        public override void CopyFrom(object rightComponent)
        {
            var r = rightComponent as StateInterVarBefore;
            base.CopyFrom(rightComponent);
        }

        public override bool IsApproximatelyEqual(object right)
        {
            var r = right as StateInterVarBefore;
            return base.IsApproximatelyEqual(right);
        }

      

//        bool IComparableComponent.IsApproximatelyEqual(object right)
//        {
//            return IsApproximatelyEqual(right);
//        }

        
        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerFsmOutBefore;
        }

        public override string ToString()
        {
            return string.Format("{0}", base.ToString());
        }
    }
    
    
    
    [Player]
    public class StateInterfaceComponent : IComponent
    {
        public ICharacterState State;
    }
  
}
