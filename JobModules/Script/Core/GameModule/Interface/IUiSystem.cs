using UnityEngine;

namespace Core.GameModule.Interface
{
    public interface IUiSystem
    {
        void OnUiRender(float intervalTime);
    } 
    
    public interface IUiHfrSystem
    {
        void OnUiRender(float intervalTime);
    }
   
}