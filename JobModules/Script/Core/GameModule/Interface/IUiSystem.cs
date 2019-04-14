using UnityEngine;

namespace Core.GameModule.Interface
{
    public interface IUiSystem:IUserSystem
    {
        void OnUiRender(float intervalTime);
    } 
    
    public interface IUiHfrSystem:IUserSystem
    {
        void OnUiRender(float intervalTime);
    }
   
}