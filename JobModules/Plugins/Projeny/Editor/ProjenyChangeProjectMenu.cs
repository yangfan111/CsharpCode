
using UnityEditor;
using Projeny.Internal;

namespace Projeny
{
    public static class ProjenyChangeProjectMenu
    {
        
        [MenuItem("Projeny/Change Project/App.Client.Art-Windows", false, 8)]
        public static void ChangeProject1()
        {
            PrjHelper.ChangeProject("App.Client.Art", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/App.Client.Unity-Windows", false, 8)]
        public static void ChangeProject2()
        {
            PrjHelper.ChangeProject("App.Client.Unity", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/App.Client.Unity-Windows", true, 8)]
        public static bool ChangeProject2Validate()
        {
            return false;
        }
        
        [MenuItem("Projeny/Change Project/App.Client.Unity2-Windows", false, 8)]
        public static void ChangeProject3()
        {
            PrjHelper.ChangeProject("App.Client.Unity2", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/App.Hall.Test.Unity-Windows", false, 8)]
        public static void ChangeProject4()
        {
            PrjHelper.ChangeProject("App.Hall.Test.Unity", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/App.Hall.Unity-Windows", false, 8)]
        public static void ChangeProject5()
        {
            PrjHelper.ChangeProject("App.Hall.Unity", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/App.HallClient.Unity-Windows", false, 8)]
        public static void ChangeProject6()
        {
            PrjHelper.ChangeProject("App.HallClient.Unity", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/App.Robot.Unity-Windows", false, 8)]
        public static void ChangeProject7()
        {
            PrjHelper.ChangeProject("App.Robot.Unity", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/App.Server.Art-Windows", false, 8)]
        public static void ChangeProject8()
        {
            PrjHelper.ChangeProject("App.Server.Art", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/App.Server.Unity-Windows", false, 8)]
        public static void ChangeProject9()
        {
            PrjHelper.ChangeProject("App.Server.Unity", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/App.Test.Unity-Windows", false, 8)]
        public static void ChangeProject10()
        {
            PrjHelper.ChangeProject("App.Test.Unity", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/App.Tools.Unity-Windows", false, 8)]
        public static void ChangeProject11()
        {
            PrjHelper.ChangeProject("App.Tools.Unity", "Windows");
        }
        
        [MenuItem("Projeny/Change Project/AssetBundleOnly-Windows", false, 8)]
        public static void ChangeProject12()
        {
            PrjHelper.ChangeProject("AssetBundleOnly", "Windows");
        }
        
    }
}
