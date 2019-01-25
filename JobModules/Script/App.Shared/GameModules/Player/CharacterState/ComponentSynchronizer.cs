using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player.Appearance;
using BehaviorDesigner.Runtime;
using Core.Appearance;
using Core.CharacterState;
using Utils.Appearance;

namespace App.Shared.GameModules.Player.CharacterState
{
    public static class ComponentSynchronizer
    {
        private static void SyncToStateInterVarAbstractComponent(AbstractStateInterVar component, ICharacterState state)
        {
            component.VerticalValue = state.VerticalValue;
            component.HorizontalValue = state.HorizontalValue;
            component.UpDownValue = state.UpDownValue;
        }
        
        public static void SyncToStateInterVarComponent(StateInterVar component, ICharacterState state)
        {
            component.IsJumpForSync = state.IsNeedJumpForSync;
            SyncToStateInterVarAbstractComponent(component, state);
        }
        
        public static void SyncToStateInterVarBeforeComponent(StateInterVarBefore component, ICharacterState state)
        {
            SyncToStateInterVarAbstractComponent(component, state);
        }
        
        private static void SyncToStateAbstractComponent(AbstractStateComponent component, ICharacterState state)
        {
            var snapshots = state.GetSnapshots();
            component.PostureStateId = snapshots[0].StateId;
            component.PostureStateProgress = snapshots[0].StateProgress;
            component.PostureTransitionId = snapshots[0].TransitoinId;
            component.PostureTransitionProgress = snapshots[0].TransitionProgress;
            component.LeanStateId = snapshots[1].StateId;
            component.LeanStateProgress = snapshots[1].StateProgress;
            component.LeanTransitionId = snapshots[1].TransitoinId;
            component.LeanTransitionProgress = snapshots[1].TransitionProgress;
            component.MovementStateId = snapshots[2].StateId;
            component.MovementStateProgress = snapshots[2].StateProgress;
            component.MovementTransitionId = snapshots[2].TransitoinId;
            component.MovementTransitionProgress = snapshots[2].TransitionProgress;
            component.ActionStateId = snapshots[3].StateId;
            component.ActionStateProgress = snapshots[3].StateProgress;
            component.ActionTransitionId = snapshots[3].TransitoinId;
            component.ActionTransitionProgress = snapshots[3].TransitionProgress;
            component.KeepStateId = snapshots[4].StateId;
            component.KeepStateProgress = snapshots[4].StateProgress;
            component.KeepTransitionId = snapshots[4].TransitoinId;
            component.KeepTransitionProgress = snapshots[4].TransitionProgress;
        }

        public static void SyncToStateComponent(StateComponent component, ICharacterState state)
        {
            SyncToStateAbstractComponent(component, state);
        }
        
        public static void SyncToStateBeforeComponent(StateBeforeComponent component, ICharacterState state)
        {
            SyncToStateAbstractComponent(component, state);
        }

        private static void SyncFromStateInterVarAbstractComponent(AbstractStateInterVar component, ICharacterState state)
        {
            state.UpdateAxis(component.HorizontalValue, component.VerticalValue, component.UpDownValue);
        }
        
        public static void SyncFromStateInterVarComponent(StateInterVar component, ICharacterState state)
        {
            state.IsNeedJumpForSync = component.IsJumpForSync;
            SyncFromStateInterVarAbstractComponent(component, state);
        }
        
        public static void SyncFromStateInterVarBeforeComponent(StateInterVarBefore component, ICharacterState state)
        {
            
            SyncFromStateInterVarAbstractComponent(component, state);
        }

        private static void SyncFromStateAbstractComponent(AbstractStateComponent component, ICharacterState state)
        {
            var snapshots = state.GetSnapshots();
            snapshots[0].StateId = component.PostureStateId;
            snapshots[0].StateProgress = component.PostureStateProgress;
            snapshots[0].TransitoinId = component.PostureTransitionId;
            snapshots[0].TransitionProgress = component.PostureTransitionProgress;
            snapshots[1].StateId = component.LeanStateId;
            snapshots[1].StateProgress = component.LeanStateProgress;
            snapshots[1].TransitoinId = component.LeanTransitionId;
            snapshots[1].TransitionProgress = component.LeanTransitionProgress;
            snapshots[2].StateId = component.MovementStateId;
            snapshots[2].StateProgress = component.MovementStateProgress;
            snapshots[2].TransitoinId = component.MovementTransitionId;
            snapshots[2].TransitionProgress = component.MovementTransitionProgress;
            snapshots[3].StateId = component.ActionStateId;
            snapshots[3].StateProgress = component.ActionStateProgress;
            snapshots[3].TransitoinId = component.ActionTransitionId;
            snapshots[3].TransitionProgress = component.ActionTransitionProgress;
            snapshots[4].StateId = component.KeepStateId;
            snapshots[4].StateProgress = component.KeepStateProgress;
            snapshots[4].TransitoinId = component.KeepTransitionId;
            snapshots[4].TransitionProgress = component.KeepTransitionProgress;
            state.TryRewind();
        }

        public static void SyncFromStateComponent(StateComponent component, ICharacterState state)
        {
            SyncFromStateAbstractComponent(component, state);
        }

        public static void SyncFromStateBeforeComponent(StateBeforeComponent component, ICharacterState state)
        {
            SyncFromStateAbstractComponent(component, state);
        }
        
        
    }
}
