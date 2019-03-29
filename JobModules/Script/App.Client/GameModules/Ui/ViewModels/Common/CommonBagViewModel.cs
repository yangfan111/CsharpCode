using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Assets.UiFramework.Libs;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.ViewModels.Common
{
    public class CommonBagViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonBagView : UIView
        {
            public GameObject ShowGameObjectActiveSelf;
            [HideInInspector]
            public bool oriShowGameObjectActiveSelf;
            public GameObject ShowPlayer;
            [HideInInspector]
            public bool oriShowPlayer;
            public RawImage PlayerTexture;
            [HideInInspector]
            public Texture oriPlayerTexture;
            public DropZone InventoryBgDragTarget;
            [HideInInspector]
            public IDragTarget oriInventoryBgDragTarget;
            public DropZone InventoryBgDropSlot;
            [HideInInspector]
            public int oriInventoryBgDropSlot;
            public GameObject ShowPrimeWeapon;
            [HideInInspector]
            public bool oriShowPrimeWeapon;
            public DropZone PrimeWeaponDragTarget;
            [HideInInspector]
            public IDragTarget oriPrimeWeaponDragTarget;
            public DropZone PrimeWeaponSlot;
            [HideInInspector]
            public int oriPrimeWeaponSlot;
            public Dragable PrimeWeaponDragSource;
            [HideInInspector]
            public IDragSource oriPrimeWeaponDragSource;
            public Dragable PrimeWeaponDragSourceId;
            [HideInInspector]
            public int oriPrimeWeaponDragSourceId;
            public Dragable PrimeWeaponDragSourceSlot;
            [HideInInspector]
            public int oriPrimeWeaponDragSourceSlot;
            public Text PrimeWeaponName;
            [HideInInspector]
            public string oriPrimeWeaponName;
            public Image PrimeWeaponIcon;
            public Image PrimeMuzzle;
            public Image PrimeUpperRail;
            public Image PrimeLowerRail;
            public Image PrimeMagzine;
            public Image PrimeStock;
            public GameObject ShowSubWeapon;
            [HideInInspector]
            public bool oriShowSubWeapon;
            public DropZone SubWeaponDragTarget;
            [HideInInspector]
            public IDragTarget oriSubWeaponDragTarget;
            public DropZone SubWeaponSlot;
            [HideInInspector]
            public int oriSubWeaponSlot;
            public Text SubWeaponName;
            [HideInInspector]
            public string oriSubWeaponName;
            public Image SubWeaponIcon;
            public DropZone PistolDragTarget;
            [HideInInspector]
            public IDragTarget oriPistolDragTarget;
            public DropZone PistolSlot;
            [HideInInspector]
            public int oriPistolSlot;
            public GameObject ShowPistol;
            [HideInInspector]
            public bool oriShowPistol;
            public Image PistolIcon;
            public Text PistolName;
            [HideInInspector]
            public string oriPistolName;
            public GameObject ShowMelee;
            [HideInInspector]
            public bool oriShowMelee;
            public DropZone MeleeDragTarget;
            [HideInInspector]
            public IDragTarget oriMeleeDragTarget;
            public DropZone MeleeSlot;
            [HideInInspector]
            public int oriMeleeSlot;
            public Text MeleeName;
            [HideInInspector]
            public string oriMeleeName;
            public Image MeleeIcon;
            public GameObject ShowGrenade;
            [HideInInspector]
            public bool oriShowGrenade;
            public DropZone GrenadeDragTarget;
            [HideInInspector]
            public IDragTarget oriGrenadeDragTarget;
            public DropZone GrenadeSlot;
            [HideInInspector]
            public int oriGrenadeSlot;
            public Text GrenadeName;
            [HideInInspector]
            public string oriGrenadeName;
            public Image GrenadeIcon;
            public DropZone NearByBgDragTarget;
            [HideInInspector]
            public IDragTarget oriNearByBgDragTarget;
            public DropZone NearByBgDropSlot;
            [HideInInspector]
            public int oriNearByBgDropSlot;
            public GameObject ShowNearbyItem1;
            public Dragable NearByDragSource1;
            public Dragable NearByDragSourceId1;
            public Dragable NearByDragSourceSlot1;
            public Image NearbyItemIcon1;
            public Text NearbyItemName1;
            public Text NearbyItemNum1;
            public GameObject ShowNearbyItem2;
            public Dragable NearByDragSource2;
            public Dragable NearByDragSourceId2;
            public Dragable NearByDragSourceSlot2;
            public Image NearbyItemIcon2;
            public Text NearbyItemName2;
            public Text NearbyItemNum2;
            public GameObject ShowNearbyItem3;
            public Dragable NearByDragSource3;
            public Dragable NearByDragSourceId3;
            public Dragable NearByDragSourceSlot3;
            public Image NearbyItemIcon3;
            public Text NearbyItemName3;
            public Text NearbyItemNum3;
            public GameObject ShowNearbyItem4;
            public Dragable NearByDragSource4;
            public Dragable NearByDragSourceId4;
            public Dragable NearByDragSourceSlot4;
            public Image NearbyItemIcon4;
            public Text NearbyItemName4;
            public Text NearbyItemNum4;
            public GameObject ShowNearbyItem5;
            public Dragable NearByDragSource5;
            public Dragable NearByDragSourceId5;
            public Dragable NearByDragSourceSlot5;
            public Image NearbyItemIcon5;
            public Text NearbyItemName5;
            public Text NearbyItemNum5;
            public GameObject ShowNearbyItem6;
            public Dragable NearByDragSource6;
            public Dragable NearByDragSourceId6;
            public Dragable NearByDragSourceSlot6;
            public Image NearbyItemIcon6;
            public Text NearbyItemName6;
            public Text NearbyItemNum6;
            public GameObject ShowNearbyItem7;
            public Dragable NearByDragSource7;
            public Dragable NearByDragSourceId7;
            public Dragable NearByDragSourceSlot7;
            public Image NearbyItemIcon7;
            public Text NearbyItemName7;
            public Text NearbyItemNum7;
            public GameObject ShowNearbyItem8;
            public Dragable NearByDragSource8;
            public Dragable NearByDragSourceId8;
            public Dragable NearByDragSourceSlot8;
            public Image NearbyItemIcon8;
            public Text NearbyItemName8;
            public Text NearbyItemNum8;
            public GameObject ShowNearbyItem9;
            public Dragable NearByDragSource9;
            public Dragable NearByDragSourceId9;
            public Dragable NearByDragSourceSlot9;
            public Image NearbyItemIcon9;
            public Text NearbyItemName9;
            public Text NearbyItemNum9;
            public GameObject ShowNearbyItem10;
            public Dragable NearByDragSource10;
            public Dragable NearByDragSourceId10;
            public Dragable NearByDragSourceSlot10;
            public Image NearbyItemIcon10;
            public Text NearbyItemName10;
            public Text NearbyItemNum10;
            public GameObject ShowNearbyItem11;
            public Dragable NearByDragSource11;
            public Dragable NearByDragSourceId11;
            public Dragable NearByDragSourceSlot11;
            public Image NearbyItemIcon11;
            public Text NearbyItemName11;
            public Text NearbyItemNum11;
            public GameObject ShowNearbyItem12;
            public Dragable NearByDragSource12;
            public Dragable NearByDragSourceId12;
            public Dragable NearByDragSourceSlot12;
            public Image NearbyItemIcon12;
            public Text NearbyItemName12;
            public Text NearbyItemNum12;
            public GameObject ShowNearbyItem13;
            public Dragable NearByDragSource13;
            public Dragable NearByDragSourceId13;
            public Dragable NearByDragSourceSlot13;
            public Image NearbyItemIcon13;
            public Text NearbyItemName13;
            public Text NearbyItemNum13;
            public GameObject ShowNearbyItem14;
            public Dragable NearByDragSource14;
            public Dragable NearByDragSourceId14;
            public Dragable NearByDragSourceSlot14;
            public Image NearbyItemIcon14;
            public Text NearbyItemName14;
            public Text NearbyItemNum14;
            public GameObject ShowNearbyItem15;
            public Dragable NearByDragSource15;
            public Dragable NearByDragSourceId15;
            public Dragable NearByDragSourceSlot15;
            public Image NearbyItemIcon15;
            public Text NearbyItemName15;
            public Text NearbyItemNum15;
            public GameObject ShowNearbyItem16;
            public Dragable NearByDragSource16;
            public Dragable NearByDragSourceId16;
            public Dragable NearByDragSourceSlot16;
            public Image NearbyItemIcon16;
            public Text NearbyItemName16;
            public Text NearbyItemNum16;
            public GameObject ShowNearbyItem17;
            public Dragable NearByDragSource17;
            public Dragable NearByDragSourceId17;
            public Dragable NearByDragSourceSlot17;
            public Image NearbyItemIcon17;
            public Text NearbyItemName17;
            public Text NearbyItemNum17;
            public GameObject ShowNearbyItem18;
            public Dragable NearByDragSource18;
            public Dragable NearByDragSourceId18;
            public Dragable NearByDragSourceSlot18;
            public Image NearbyItemIcon18;
            public Text NearbyItemName18;
            public Text NearbyItemNum18;
            public GameObject ShowNearbyItem19;
            public Dragable NearByDragSource19;
            public Dragable NearByDragSourceId19;
            public Dragable NearByDragSourceSlot19;
            public Image NearbyItemIcon19;
            public Text NearbyItemName19;
            public Text NearbyItemNum19;
            public GameObject ShowNearbyItem20;
            public Dragable NearByDragSource20;
            public Dragable NearByDragSourceId20;
            public Dragable NearByDragSourceSlot20;
            public Image NearbyItemIcon20;
            public Text NearbyItemName20;
            public Text NearbyItemNum20;
            public GameObject ShowInventoryItem1;
            public DropZone InventoryDragTarget1;
            public DropZone InventoryDropSlot1;
            public Dragable InventoryDragSource1;
            public Dragable InventoryDragSourceId1;
            public Dragable InventoryDragSourceSlot1;
            public Image InventoryItemIcon1;
            public Text InventoryItemName1;
            public Text InventoryItemNum1;
            public GameObject ShowInventoryItem2;
            public DropZone InventoryDragTarget2;
            public DropZone InventoryDropSlot2;
            public Dragable InventoryDragSource2;
            public Dragable InventoryDragSourceId2;
            public Dragable InventoryDragSourceSlot2;
            public Image InventoryItemIcon2;
            public Text InventoryItemName2;
            public Text InventoryItemNum2;
            public GameObject ShowInventoryItem3;
            public DropZone InventoryDragTarget3;
            public DropZone InventoryDropSlot3;
            public Dragable InventoryDragSource3;
            public Dragable InventoryDragSourceId3;
            public Dragable InventoryDragSourceSlot3;
            public Image InventoryItemIcon3;
            public Text InventoryItemName3;
            public Text InventoryItemNum3;
            public GameObject ShowInventoryItem4;
            public DropZone InventoryDragTarget4;
            public DropZone InventoryDropSlot4;
            public Dragable InventoryDragSource4;
            public Dragable InventoryDragSourceId4;
            public Dragable InventoryDragSourceSlot4;
            public Image InventoryItemIcon4;
            public Text InventoryItemName4;
            public Text InventoryItemNum4;
            public GameObject ShowInventoryItem5;
            public DropZone InventoryDragTarget5;
            public DropZone InventoryDropSlot5;
            public Dragable InventoryDragSource5;
            public Dragable InventoryDragSourceId5;
            public Dragable InventoryDragSourceSlot5;
            public Image InventoryItemIcon5;
            public Text InventoryItemName5;
            public Text InventoryItemNum5;
            public GameObject ShowInventoryItem6;
            public DropZone InventoryDragTarget6;
            public DropZone InventoryDropSlot6;
            public Dragable InventoryDragSource6;
            public Dragable InventoryDragSourceId6;
            public Dragable InventoryDragSourceSlot6;
            public Image InventoryItemIcon6;
            public Text InventoryItemName6;
            public Text InventoryItemNum6;
            public GameObject ShowInventoryItem7;
            public DropZone InventoryDragTarget7;
            public DropZone InventoryDropSlot7;
            public Dragable InventoryDragSource7;
            public Dragable InventoryDragSourceId7;
            public Dragable InventoryDragSourceSlot7;
            public Image InventoryItemIcon7;
            public Text InventoryItemName7;
            public Text InventoryItemNum7;
            public GameObject ShowInventoryItem8;
            public DropZone InventoryDragTarget8;
            public DropZone InventoryDropSlot8;
            public Dragable InventoryDragSource8;
            public Dragable InventoryDragSourceId8;
            public Dragable InventoryDragSourceSlot8;
            public Image InventoryItemIcon8;
            public Text InventoryItemName8;
            public Text InventoryItemNum8;
            public GameObject ShowInventoryItem9;
            public DropZone InventoryDragTarget9;
            public DropZone InventoryDropSlot9;
            public Dragable InventoryDragSource9;
            public Dragable InventoryDragSourceId9;
            public Dragable InventoryDragSourceSlot9;
            public Image InventoryItemIcon9;
            public Text InventoryItemName9;
            public Text InventoryItemNum9;
            public GameObject ShowInventoryItem10;
            public DropZone InventoryDragTarget10;
            public DropZone InventoryDropSlot10;
            public Dragable InventoryDragSource10;
            public Dragable InventoryDragSourceId10;
            public Dragable InventoryDragSourceSlot10;
            public Image InventoryItemIcon10;
            public Text InventoryItemName10;
            public Text InventoryItemNum10;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Show":
                            ShowGameObjectActiveSelf = v.gameObject;
                            break;
                        case "Player":
                            ShowPlayer = v.gameObject;
                            break;
                        case "PrimeWeapon":
                            ShowPrimeWeapon = v.gameObject;
                            break;
                        case "SubWeapon":
                            ShowSubWeapon = v.gameObject;
                            break;
                        case "Pistol":
                            ShowPistol = v.gameObject;
                            break;
                        case "Melee":
                            ShowMelee = v.gameObject;
                            break;
                        case "Grenade":
                            ShowGrenade = v.gameObject;
                            break;
                        case "NearbyItem1":
                            ShowNearbyItem1 = v.gameObject;
                            break;
                        case "NearbyItem2":
                            ShowNearbyItem2 = v.gameObject;
                            break;
                        case "NearbyItem3":
                            ShowNearbyItem3 = v.gameObject;
                            break;
                        case "NearbyItem4":
                            ShowNearbyItem4 = v.gameObject;
                            break;
                        case "NearbyItem5":
                            ShowNearbyItem5 = v.gameObject;
                            break;
                        case "NearbyItem6":
                            ShowNearbyItem6 = v.gameObject;
                            break;
                        case "NearbyItem7":
                            ShowNearbyItem7 = v.gameObject;
                            break;
                        case "NearbyItem8":
                            ShowNearbyItem8 = v.gameObject;
                            break;
                        case "NearbyItem9":
                            ShowNearbyItem9 = v.gameObject;
                            break;
                        case "NearbyItem10":
                            ShowNearbyItem10 = v.gameObject;
                            break;
                        case "NearbyItem11":
                            ShowNearbyItem11 = v.gameObject;
                            break;
                        case "NearbyItem12":
                            ShowNearbyItem12 = v.gameObject;
                            break;
                        case "NearbyItem13":
                            ShowNearbyItem13 = v.gameObject;
                            break;
                        case "NearbyItem14":
                            ShowNearbyItem14 = v.gameObject;
                            break;
                        case "NearbyItem15":
                            ShowNearbyItem15 = v.gameObject;
                            break;
                        case "NearbyItem16":
                            ShowNearbyItem16 = v.gameObject;
                            break;
                        case "NearbyItem17":
                            ShowNearbyItem17 = v.gameObject;
                            break;
                        case "NearbyItem18":
                            ShowNearbyItem18 = v.gameObject;
                            break;
                        case "NearbyItem19":
                            ShowNearbyItem19 = v.gameObject;
                            break;
                        case "NearbyItem20":
                            ShowNearbyItem20 = v.gameObject;
                            break;
                        case "InventoryItem1":
                            ShowInventoryItem1 = v.gameObject;
                            break;
                        case "InventoryItem2":
                            ShowInventoryItem2 = v.gameObject;
                            break;
                        case "InventoryItem3":
                            ShowInventoryItem3 = v.gameObject;
                            break;
                        case "InventoryItem4":
                            ShowInventoryItem4 = v.gameObject;
                            break;
                        case "InventoryItem5":
                            ShowInventoryItem5 = v.gameObject;
                            break;
                        case "InventoryItem6":
                            ShowInventoryItem6 = v.gameObject;
                            break;
                        case "InventoryItem7":
                            ShowInventoryItem7 = v.gameObject;
                            break;
                        case "InventoryItem8":
                            ShowInventoryItem8 = v.gameObject;
                            break;
                        case "InventoryItem9":
                            ShowInventoryItem9 = v.gameObject;
                            break;
                        case "InventoryItem10":
                            ShowInventoryItem10 = v.gameObject;
                            break;
                    }
                }

                RawImage[] rawimages = gameObject.GetComponentsInChildren<RawImage>(true);
                foreach (var v in rawimages)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Player":
                            PlayerTexture = v;
                            break;
                    }
                }

                DropZone[] dropzones = gameObject.GetComponentsInChildren<DropZone>(true);
                foreach (var v in dropzones)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Inventory":
                            InventoryBgDragTarget = v;
                            InventoryBgDropSlot = v;
                            break;
                        case "PrimeWeaponBg":
                            PrimeWeaponDragTarget = v;
                            PrimeWeaponSlot = v;
                            break;
                        case "SubWeaponBg":
                            SubWeaponDragTarget = v;
                            SubWeaponSlot = v;
                            break;
                        case "PistolBg":
                            PistolDragTarget = v;
                            PistolSlot = v;
                            break;
                        case "MeleeBg":
                            MeleeDragTarget = v;
                            MeleeSlot = v;
                            break;
                        case "GrenadeBg":
                            GrenadeDragTarget = v;
                            GrenadeSlot = v;
                            break;
                        case "NearBy":
                            NearByBgDragTarget = v;
                            NearByBgDropSlot = v;
                            break;
                        case "InventoryItem1":
                            InventoryDragTarget1 = v;
                            InventoryDropSlot1 = v;
                            break;
                        case "InventoryItem2":
                            InventoryDragTarget2 = v;
                            InventoryDropSlot2 = v;
                            break;
                        case "InventoryItem3":
                            InventoryDragTarget3 = v;
                            InventoryDropSlot3 = v;
                            break;
                        case "InventoryItem4":
                            InventoryDragTarget4 = v;
                            InventoryDropSlot4 = v;
                            break;
                        case "InventoryItem5":
                            InventoryDragTarget5 = v;
                            InventoryDropSlot5 = v;
                            break;
                        case "InventoryItem6":
                            InventoryDragTarget6 = v;
                            InventoryDropSlot6 = v;
                            break;
                        case "InventoryItem7":
                            InventoryDragTarget7 = v;
                            InventoryDropSlot7 = v;
                            break;
                        case "InventoryItem8":
                            InventoryDragTarget8 = v;
                            InventoryDropSlot8 = v;
                            break;
                        case "InventoryItem9":
                            InventoryDragTarget9 = v;
                            InventoryDropSlot9 = v;
                            break;
                        case "InventoryItem10":
                            InventoryDragTarget10 = v;
                            InventoryDropSlot10 = v;
                            break;
                    }
                }

                Dragable[] dragables = gameObject.GetComponentsInChildren<Dragable>(true);
                foreach (var v in dragables)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "PrimeWeaponBg":
                            PrimeWeaponDragSource = v;
                            PrimeWeaponDragSourceId = v;
                            PrimeWeaponDragSourceSlot = v;
                            break;
                        case "NearbyItem1":
                            NearByDragSource1 = v;
                            NearByDragSourceId1 = v;
                            NearByDragSourceSlot1 = v;
                            break;
                        case "NearbyItem2":
                            NearByDragSource2 = v;
                            NearByDragSourceId2 = v;
                            NearByDragSourceSlot2 = v;
                            break;
                        case "NearbyItem3":
                            NearByDragSource3 = v;
                            NearByDragSourceId3 = v;
                            NearByDragSourceSlot3 = v;
                            break;
                        case "NearbyItem4":
                            NearByDragSource4 = v;
                            NearByDragSourceId4 = v;
                            NearByDragSourceSlot4 = v;
                            break;
                        case "NearbyItem5":
                            NearByDragSource5 = v;
                            NearByDragSourceId5 = v;
                            NearByDragSourceSlot5 = v;
                            break;
                        case "NearbyItem6":
                            NearByDragSource6 = v;
                            NearByDragSourceId6 = v;
                            NearByDragSourceSlot6 = v;
                            break;
                        case "NearbyItem7":
                            NearByDragSource7 = v;
                            NearByDragSourceId7 = v;
                            NearByDragSourceSlot7 = v;
                            break;
                        case "NearbyItem8":
                            NearByDragSource8 = v;
                            NearByDragSourceId8 = v;
                            NearByDragSourceSlot8 = v;
                            break;
                        case "NearbyItem9":
                            NearByDragSource9 = v;
                            NearByDragSourceId9 = v;
                            NearByDragSourceSlot9 = v;
                            break;
                        case "NearbyItem10":
                            NearByDragSource10 = v;
                            NearByDragSourceId10 = v;
                            NearByDragSourceSlot10 = v;
                            break;
                        case "NearbyItem11":
                            NearByDragSource11 = v;
                            NearByDragSourceId11 = v;
                            NearByDragSourceSlot11 = v;
                            break;
                        case "NearbyItem12":
                            NearByDragSource12 = v;
                            NearByDragSourceId12 = v;
                            NearByDragSourceSlot12 = v;
                            break;
                        case "NearbyItem13":
                            NearByDragSource13 = v;
                            NearByDragSourceId13 = v;
                            NearByDragSourceSlot13 = v;
                            break;
                        case "NearbyItem14":
                            NearByDragSource14 = v;
                            NearByDragSourceId14 = v;
                            NearByDragSourceSlot14 = v;
                            break;
                        case "NearbyItem15":
                            NearByDragSource15 = v;
                            NearByDragSourceId15 = v;
                            NearByDragSourceSlot15 = v;
                            break;
                        case "NearbyItem16":
                            NearByDragSource16 = v;
                            NearByDragSourceId16 = v;
                            NearByDragSourceSlot16 = v;
                            break;
                        case "NearbyItem17":
                            NearByDragSource17 = v;
                            NearByDragSourceId17 = v;
                            NearByDragSourceSlot17 = v;
                            break;
                        case "NearbyItem18":
                            NearByDragSource18 = v;
                            NearByDragSourceId18 = v;
                            NearByDragSourceSlot18 = v;
                            break;
                        case "NearbyItem19":
                            NearByDragSource19 = v;
                            NearByDragSourceId19 = v;
                            NearByDragSourceSlot19 = v;
                            break;
                        case "NearbyItem20":
                            NearByDragSource20 = v;
                            NearByDragSourceId20 = v;
                            NearByDragSourceSlot20 = v;
                            break;
                        case "InventoryItem1":
                            InventoryDragSource1 = v;
                            InventoryDragSourceId1 = v;
                            InventoryDragSourceSlot1 = v;
                            break;
                        case "InventoryItem2":
                            InventoryDragSource2 = v;
                            InventoryDragSourceId2 = v;
                            InventoryDragSourceSlot2 = v;
                            break;
                        case "InventoryItem3":
                            InventoryDragSource3 = v;
                            InventoryDragSourceId3 = v;
                            InventoryDragSourceSlot3 = v;
                            break;
                        case "InventoryItem4":
                            InventoryDragSource4 = v;
                            InventoryDragSourceId4 = v;
                            InventoryDragSourceSlot4 = v;
                            break;
                        case "InventoryItem5":
                            InventoryDragSource5 = v;
                            InventoryDragSourceId5 = v;
                            InventoryDragSourceSlot5 = v;
                            break;
                        case "InventoryItem6":
                            InventoryDragSource6 = v;
                            InventoryDragSourceId6 = v;
                            InventoryDragSourceSlot6 = v;
                            break;
                        case "InventoryItem7":
                            InventoryDragSource7 = v;
                            InventoryDragSourceId7 = v;
                            InventoryDragSourceSlot7 = v;
                            break;
                        case "InventoryItem8":
                            InventoryDragSource8 = v;
                            InventoryDragSourceId8 = v;
                            InventoryDragSourceSlot8 = v;
                            break;
                        case "InventoryItem9":
                            InventoryDragSource9 = v;
                            InventoryDragSourceId9 = v;
                            InventoryDragSourceSlot9 = v;
                            break;
                        case "InventoryItem10":
                            InventoryDragSource10 = v;
                            InventoryDragSourceId10 = v;
                            InventoryDragSourceSlot10 = v;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "PrimeWeaponName":
                            PrimeWeaponName = v;
                            break;
                        case "SubWeaponName":
                            SubWeaponName = v;
                            break;
                        case "PistolName":
                            PistolName = v;
                            break;
                        case "MeleeName":
                            MeleeName = v;
                            break;
                        case "GrenadeName":
                            GrenadeName = v;
                            break;
                        case "NearbyItemName1":
                            NearbyItemName1 = v;
                            break;
                        case "NearbyItemNum1":
                            NearbyItemNum1 = v;
                            break;
                        case "NearbyItemName2":
                            NearbyItemName2 = v;
                            break;
                        case "NearbyItemNum2":
                            NearbyItemNum2 = v;
                            break;
                        case "NearbyItemName3":
                            NearbyItemName3 = v;
                            break;
                        case "NearbyItemNum3":
                            NearbyItemNum3 = v;
                            break;
                        case "NearbyItemName4":
                            NearbyItemName4 = v;
                            break;
                        case "NearbyItemNum4":
                            NearbyItemNum4 = v;
                            break;
                        case "NearbyItemName5":
                            NearbyItemName5 = v;
                            break;
                        case "NearbyItemNum5":
                            NearbyItemNum5 = v;
                            break;
                        case "NearbyItemName6":
                            NearbyItemName6 = v;
                            break;
                        case "NearbyItemNum6":
                            NearbyItemNum6 = v;
                            break;
                        case "NearbyItemName7":
                            NearbyItemName7 = v;
                            break;
                        case "NearbyItemNum7":
                            NearbyItemNum7 = v;
                            break;
                        case "NearbyItemName8":
                            NearbyItemName8 = v;
                            break;
                        case "NearbyItemNum8":
                            NearbyItemNum8 = v;
                            break;
                        case "NearbyItemName9":
                            NearbyItemName9 = v;
                            break;
                        case "NearbyItemNum9":
                            NearbyItemNum9 = v;
                            break;
                        case "NearbyItemName10":
                            NearbyItemName10 = v;
                            break;
                        case "NearbyItemNum10":
                            NearbyItemNum10 = v;
                            break;
                        case "NearbyItemName11":
                            NearbyItemName11 = v;
                            break;
                        case "NearbyItemNum11":
                            NearbyItemNum11 = v;
                            break;
                        case "NearbyItemName12":
                            NearbyItemName12 = v;
                            break;
                        case "NearbyItemNum12":
                            NearbyItemNum12 = v;
                            break;
                        case "NearbyItemName13":
                            NearbyItemName13 = v;
                            break;
                        case "NearbyItemNum13":
                            NearbyItemNum13 = v;
                            break;
                        case "NearbyItemName14":
                            NearbyItemName14 = v;
                            break;
                        case "NearbyItemNum14":
                            NearbyItemNum14 = v;
                            break;
                        case "NearbyItemName15":
                            NearbyItemName15 = v;
                            break;
                        case "NearbyItemNum15":
                            NearbyItemNum15 = v;
                            break;
                        case "NearbyItemName16":
                            NearbyItemName16 = v;
                            break;
                        case "NearbyItemNum16":
                            NearbyItemNum16 = v;
                            break;
                        case "NearbyItemName17":
                            NearbyItemName17 = v;
                            break;
                        case "NearbyItemNum17":
                            NearbyItemNum17 = v;
                            break;
                        case "NearbyItemName18":
                            NearbyItemName18 = v;
                            break;
                        case "NearbyItemNum18":
                            NearbyItemNum18 = v;
                            break;
                        case "NearbyItemName19":
                            NearbyItemName19 = v;
                            break;
                        case "NearbyItemNum19":
                            NearbyItemNum19 = v;
                            break;
                        case "NearbyItemName20":
                            NearbyItemName20 = v;
                            break;
                        case "NearbyItemNum20":
                            NearbyItemNum20 = v;
                            break;
                        case "InventoryItemName1":
                            InventoryItemName1 = v;
                            break;
                        case "InventoryItemNum1":
                            InventoryItemNum1 = v;
                            break;
                        case "InventoryItemName2":
                            InventoryItemName2 = v;
                            break;
                        case "InventoryItemNum2":
                            InventoryItemNum2 = v;
                            break;
                        case "InventoryItemName3":
                            InventoryItemName3 = v;
                            break;
                        case "InventoryItemNum3":
                            InventoryItemNum3 = v;
                            break;
                        case "InventoryItemName4":
                            InventoryItemName4 = v;
                            break;
                        case "InventoryItemNum4":
                            InventoryItemNum4 = v;
                            break;
                        case "InventoryItemName5":
                            InventoryItemName5 = v;
                            break;
                        case "InventoryItemNum5":
                            InventoryItemNum5 = v;
                            break;
                        case "InventoryItemName6":
                            InventoryItemName6 = v;
                            break;
                        case "InventoryItemNum6":
                            InventoryItemNum6 = v;
                            break;
                        case "InventoryItemName7":
                            InventoryItemName7 = v;
                            break;
                        case "InventoryItemNum7":
                            InventoryItemNum7 = v;
                            break;
                        case "InventoryItemName8":
                            InventoryItemName8 = v;
                            break;
                        case "InventoryItemNum8":
                            InventoryItemNum8 = v;
                            break;
                        case "InventoryItemName9":
                            InventoryItemName9 = v;
                            break;
                        case "InventoryItemNum9":
                            InventoryItemNum9 = v;
                            break;
                        case "InventoryItemName10":
                            InventoryItemName10 = v;
                            break;
                        case "InventoryItemNum10":
                            InventoryItemNum10 = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "PrimeWeaponIcon":
                            PrimeWeaponIcon = v;
                            break;
                        case "PrimeMuzzle":
                            PrimeMuzzle = v;
                            break;
                        case "PrimeUpperRail":
                            PrimeUpperRail = v;
                            break;
                        case "PrimeLowerRail":
                            PrimeLowerRail = v;
                            break;
                        case "PrimeMagzine":
                            PrimeMagzine = v;
                            break;
                        case "PrimeStock":
                            PrimeStock = v;
                            break;
                        case "SubWeaponIcon":
                            SubWeaponIcon = v;
                            break;
                        case "PistolIcon":
                            PistolIcon = v;
                            break;
                        case "MeleeIcon":
                            MeleeIcon = v;
                            break;
                        case "GrenadeIcon":
                            GrenadeIcon = v;
                            break;
                        case "NearbyItemIcon1":
                            NearbyItemIcon1 = v;
                            break;
                        case "NearbyItemIcon2":
                            NearbyItemIcon2 = v;
                            break;
                        case "NearbyItemIcon3":
                            NearbyItemIcon3 = v;
                            break;
                        case "NearbyItemIcon4":
                            NearbyItemIcon4 = v;
                            break;
                        case "NearbyItemIcon5":
                            NearbyItemIcon5 = v;
                            break;
                        case "NearbyItemIcon6":
                            NearbyItemIcon6 = v;
                            break;
                        case "NearbyItemIcon7":
                            NearbyItemIcon7 = v;
                            break;
                        case "NearbyItemIcon8":
                            NearbyItemIcon8 = v;
                            break;
                        case "NearbyItemIcon9":
                            NearbyItemIcon9 = v;
                            break;
                        case "NearbyItemIcon10":
                            NearbyItemIcon10 = v;
                            break;
                        case "NearbyItemIcon11":
                            NearbyItemIcon11 = v;
                            break;
                        case "NearbyItemIcon12":
                            NearbyItemIcon12 = v;
                            break;
                        case "NearbyItemIcon13":
                            NearbyItemIcon13 = v;
                            break;
                        case "NearbyItemIcon14":
                            NearbyItemIcon14 = v;
                            break;
                        case "NearbyItemIcon15":
                            NearbyItemIcon15 = v;
                            break;
                        case "NearbyItemIcon16":
                            NearbyItemIcon16 = v;
                            break;
                        case "NearbyItemIcon17":
                            NearbyItemIcon17 = v;
                            break;
                        case "NearbyItemIcon18":
                            NearbyItemIcon18 = v;
                            break;
                        case "NearbyItemIcon19":
                            NearbyItemIcon19 = v;
                            break;
                        case "NearbyItemIcon20":
                            NearbyItemIcon20 = v;
                            break;
                        case "InventoryItemIcon1":
                            InventoryItemIcon1 = v;
                            break;
                        case "InventoryItemIcon2":
                            InventoryItemIcon2 = v;
                            break;
                        case "InventoryItemIcon3":
                            InventoryItemIcon3 = v;
                            break;
                        case "InventoryItemIcon4":
                            InventoryItemIcon4 = v;
                            break;
                        case "InventoryItemIcon5":
                            InventoryItemIcon5 = v;
                            break;
                        case "InventoryItemIcon6":
                            InventoryItemIcon6 = v;
                            break;
                        case "InventoryItemIcon7":
                            InventoryItemIcon7 = v;
                            break;
                        case "InventoryItemIcon8":
                            InventoryItemIcon8 = v;
                            break;
                        case "InventoryItemIcon9":
                            InventoryItemIcon9 = v;
                            break;
                        case "InventoryItemIcon10":
                            InventoryItemIcon10 = v;
                            break;
                    }
                }

            }
        }


        private bool _showGameObjectActiveSelf;
        private bool _showPlayer;
        private Texture _playerTexture;
        private IDragTarget _inventoryBgDragTarget;
        private int _inventoryBgDropSlot;
        private bool _showPrimeWeapon;
        private IDragTarget _primeWeaponDragTarget;
        private int _primeWeaponSlot;
        private IDragSource _primeWeaponDragSource;
        private int _primeWeaponDragSourceId;
        private int _primeWeaponDragSourceSlot;
        private string _primeWeaponName;
        private Sprite _primeWeaponIcon;
        private Sprite _primeMuzzle;
        private Sprite _primeUpperRail;
        private Sprite _primeLowerRail;
        private Sprite _primeMagzine;
        private Sprite _primeStock;
        private bool _showSubWeapon;
        private IDragTarget _subWeaponDragTarget;
        private int _subWeaponSlot;
        private string _subWeaponName;
        private Sprite _subWeaponIcon;
        private IDragTarget _pistolDragTarget;
        private int _pistolSlot;
        private bool _showPistol;
        private Sprite _pistolIcon;
        private string _pistolName;
        private bool _showMelee;
        private IDragTarget _meleeDragTarget;
        private int _meleeSlot;
        private string _meleeName;
        private Sprite _meleeIcon;
        private bool _showGrenade;
        private IDragTarget _grenadeDragTarget;
        private int _grenadeSlot;
        private string _grenadeName;
        private Sprite _grenadeIcon;
        private IDragTarget _nearByBgDragTarget;
        private int _nearByBgDropSlot;
        private bool _showNearbyItem1;
        private IDragSource _nearByDragSource1;
        private int _nearByDragSourceId1;
        private int _nearByDragSourceSlot1;
        private Sprite _nearbyItemIcon1;
        private string _nearbyItemName1;
        private string _nearbyItemNum1;
        private bool _showNearbyItem2;
        private IDragSource _nearByDragSource2;
        private int _nearByDragSourceId2;
        private int _nearByDragSourceSlot2;
        private Sprite _nearbyItemIcon2;
        private string _nearbyItemName2;
        private string _nearbyItemNum2;
        private bool _showNearbyItem3;
        private IDragSource _nearByDragSource3;
        private int _nearByDragSourceId3;
        private int _nearByDragSourceSlot3;
        private Sprite _nearbyItemIcon3;
        private string _nearbyItemName3;
        private string _nearbyItemNum3;
        private bool _showNearbyItem4;
        private IDragSource _nearByDragSource4;
        private int _nearByDragSourceId4;
        private int _nearByDragSourceSlot4;
        private Sprite _nearbyItemIcon4;
        private string _nearbyItemName4;
        private string _nearbyItemNum4;
        private bool _showNearbyItem5;
        private IDragSource _nearByDragSource5;
        private int _nearByDragSourceId5;
        private int _nearByDragSourceSlot5;
        private Sprite _nearbyItemIcon5;
        private string _nearbyItemName5;
        private string _nearbyItemNum5;
        private bool _showNearbyItem6;
        private IDragSource _nearByDragSource6;
        private int _nearByDragSourceId6;
        private int _nearByDragSourceSlot6;
        private Sprite _nearbyItemIcon6;
        private string _nearbyItemName6;
        private string _nearbyItemNum6;
        private bool _showNearbyItem7;
        private IDragSource _nearByDragSource7;
        private int _nearByDragSourceId7;
        private int _nearByDragSourceSlot7;
        private Sprite _nearbyItemIcon7;
        private string _nearbyItemName7;
        private string _nearbyItemNum7;
        private bool _showNearbyItem8;
        private IDragSource _nearByDragSource8;
        private int _nearByDragSourceId8;
        private int _nearByDragSourceSlot8;
        private Sprite _nearbyItemIcon8;
        private string _nearbyItemName8;
        private string _nearbyItemNum8;
        private bool _showNearbyItem9;
        private IDragSource _nearByDragSource9;
        private int _nearByDragSourceId9;
        private int _nearByDragSourceSlot9;
        private Sprite _nearbyItemIcon9;
        private string _nearbyItemName9;
        private string _nearbyItemNum9;
        private bool _showNearbyItem10;
        private IDragSource _nearByDragSource10;
        private int _nearByDragSourceId10;
        private int _nearByDragSourceSlot10;
        private Sprite _nearbyItemIcon10;
        private string _nearbyItemName10;
        private string _nearbyItemNum10;
        private bool _showNearbyItem11;
        private IDragSource _nearByDragSource11;
        private int _nearByDragSourceId11;
        private int _nearByDragSourceSlot11;
        private Sprite _nearbyItemIcon11;
        private string _nearbyItemName11;
        private string _nearbyItemNum11;
        private bool _showNearbyItem12;
        private IDragSource _nearByDragSource12;
        private int _nearByDragSourceId12;
        private int _nearByDragSourceSlot12;
        private Sprite _nearbyItemIcon12;
        private string _nearbyItemName12;
        private string _nearbyItemNum12;
        private bool _showNearbyItem13;
        private IDragSource _nearByDragSource13;
        private int _nearByDragSourceId13;
        private int _nearByDragSourceSlot13;
        private Sprite _nearbyItemIcon13;
        private string _nearbyItemName13;
        private string _nearbyItemNum13;
        private bool _showNearbyItem14;
        private IDragSource _nearByDragSource14;
        private int _nearByDragSourceId14;
        private int _nearByDragSourceSlot14;
        private Sprite _nearbyItemIcon14;
        private string _nearbyItemName14;
        private string _nearbyItemNum14;
        private bool _showNearbyItem15;
        private IDragSource _nearByDragSource15;
        private int _nearByDragSourceId15;
        private int _nearByDragSourceSlot15;
        private Sprite _nearbyItemIcon15;
        private string _nearbyItemName15;
        private string _nearbyItemNum15;
        private bool _showNearbyItem16;
        private IDragSource _nearByDragSource16;
        private int _nearByDragSourceId16;
        private int _nearByDragSourceSlot16;
        private Sprite _nearbyItemIcon16;
        private string _nearbyItemName16;
        private string _nearbyItemNum16;
        private bool _showNearbyItem17;
        private IDragSource _nearByDragSource17;
        private int _nearByDragSourceId17;
        private int _nearByDragSourceSlot17;
        private Sprite _nearbyItemIcon17;
        private string _nearbyItemName17;
        private string _nearbyItemNum17;
        private bool _showNearbyItem18;
        private IDragSource _nearByDragSource18;
        private int _nearByDragSourceId18;
        private int _nearByDragSourceSlot18;
        private Sprite _nearbyItemIcon18;
        private string _nearbyItemName18;
        private string _nearbyItemNum18;
        private bool _showNearbyItem19;
        private IDragSource _nearByDragSource19;
        private int _nearByDragSourceId19;
        private int _nearByDragSourceSlot19;
        private Sprite _nearbyItemIcon19;
        private string _nearbyItemName19;
        private string _nearbyItemNum19;
        private bool _showNearbyItem20;
        private IDragSource _nearByDragSource20;
        private int _nearByDragSourceId20;
        private int _nearByDragSourceSlot20;
        private Sprite _nearbyItemIcon20;
        private string _nearbyItemName20;
        private string _nearbyItemNum20;
        private bool _showInventoryItem1;
        private IDragTarget _inventoryDragTarget1;
        private int _inventoryDropSlot1;
        private IDragSource _inventoryDragSource1;
        private int _inventoryDragSourceId1;
        private int _inventoryDragSourceSlot1;
        private Sprite _inventoryItemIcon1;
        private string _inventoryItemName1;
        private string _inventoryItemNum1;
        private bool _showInventoryItem2;
        private IDragTarget _inventoryDragTarget2;
        private int _inventoryDropSlot2;
        private IDragSource _inventoryDragSource2;
        private int _inventoryDragSourceId2;
        private int _inventoryDragSourceSlot2;
        private Sprite _inventoryItemIcon2;
        private string _inventoryItemName2;
        private string _inventoryItemNum2;
        private bool _showInventoryItem3;
        private IDragTarget _inventoryDragTarget3;
        private int _inventoryDropSlot3;
        private IDragSource _inventoryDragSource3;
        private int _inventoryDragSourceId3;
        private int _inventoryDragSourceSlot3;
        private Sprite _inventoryItemIcon3;
        private string _inventoryItemName3;
        private string _inventoryItemNum3;
        private bool _showInventoryItem4;
        private IDragTarget _inventoryDragTarget4;
        private int _inventoryDropSlot4;
        private IDragSource _inventoryDragSource4;
        private int _inventoryDragSourceId4;
        private int _inventoryDragSourceSlot4;
        private Sprite _inventoryItemIcon4;
        private string _inventoryItemName4;
        private string _inventoryItemNum4;
        private bool _showInventoryItem5;
        private IDragTarget _inventoryDragTarget5;
        private int _inventoryDropSlot5;
        private IDragSource _inventoryDragSource5;
        private int _inventoryDragSourceId5;
        private int _inventoryDragSourceSlot5;
        private Sprite _inventoryItemIcon5;
        private string _inventoryItemName5;
        private string _inventoryItemNum5;
        private bool _showInventoryItem6;
        private IDragTarget _inventoryDragTarget6;
        private int _inventoryDropSlot6;
        private IDragSource _inventoryDragSource6;
        private int _inventoryDragSourceId6;
        private int _inventoryDragSourceSlot6;
        private Sprite _inventoryItemIcon6;
        private string _inventoryItemName6;
        private string _inventoryItemNum6;
        private bool _showInventoryItem7;
        private IDragTarget _inventoryDragTarget7;
        private int _inventoryDropSlot7;
        private IDragSource _inventoryDragSource7;
        private int _inventoryDragSourceId7;
        private int _inventoryDragSourceSlot7;
        private Sprite _inventoryItemIcon7;
        private string _inventoryItemName7;
        private string _inventoryItemNum7;
        private bool _showInventoryItem8;
        private IDragTarget _inventoryDragTarget8;
        private int _inventoryDropSlot8;
        private IDragSource _inventoryDragSource8;
        private int _inventoryDragSourceId8;
        private int _inventoryDragSourceSlot8;
        private Sprite _inventoryItemIcon8;
        private string _inventoryItemName8;
        private string _inventoryItemNum8;
        private bool _showInventoryItem9;
        private IDragTarget _inventoryDragTarget9;
        private int _inventoryDropSlot9;
        private IDragSource _inventoryDragSource9;
        private int _inventoryDragSourceId9;
        private int _inventoryDragSourceSlot9;
        private Sprite _inventoryItemIcon9;
        private string _inventoryItemName9;
        private string _inventoryItemNum9;
        private bool _showInventoryItem10;
        private IDragTarget _inventoryDragTarget10;
        private int _inventoryDropSlot10;
        private IDragSource _inventoryDragSource10;
        private int _inventoryDragSourceId10;
        private int _inventoryDragSourceSlot10;
        private Sprite _inventoryItemIcon10;
        private string _inventoryItemName10;
        private string _inventoryItemNum10;
        public bool ShowGameObjectActiveSelf { get { return _showGameObjectActiveSelf; } set {if(_showGameObjectActiveSelf != value) Set(ref _showGameObjectActiveSelf, value, "ShowGameObjectActiveSelf"); } }
        public bool ShowPlayer { get { return _showPlayer; } set {if(_showPlayer != value) Set(ref _showPlayer, value, "ShowPlayer"); } }
        public Texture PlayerTexture { get { return _playerTexture; } set {if(_playerTexture != value) Set(ref _playerTexture, value, "PlayerTexture"); } }
        public IDragTarget InventoryBgDragTarget { get { return _inventoryBgDragTarget; } set {if(_inventoryBgDragTarget != value) Set(ref _inventoryBgDragTarget, value, "InventoryBgDragTarget"); } }
        public int InventoryBgDropSlot { get { return _inventoryBgDropSlot; } set {if(_inventoryBgDropSlot != value) Set(ref _inventoryBgDropSlot, value, "InventoryBgDropSlot"); } }
        public bool ShowPrimeWeapon { get { return _showPrimeWeapon; } set {if(_showPrimeWeapon != value) Set(ref _showPrimeWeapon, value, "ShowPrimeWeapon"); } }
        public IDragTarget PrimeWeaponDragTarget { get { return _primeWeaponDragTarget; } set {if(_primeWeaponDragTarget != value) Set(ref _primeWeaponDragTarget, value, "PrimeWeaponDragTarget"); } }
        public int PrimeWeaponSlot { get { return _primeWeaponSlot; } set {if(_primeWeaponSlot != value) Set(ref _primeWeaponSlot, value, "PrimeWeaponSlot"); } }
        public IDragSource PrimeWeaponDragSource { get { return _primeWeaponDragSource; } set {if(_primeWeaponDragSource != value) Set(ref _primeWeaponDragSource, value, "PrimeWeaponDragSource"); } }
        public int PrimeWeaponDragSourceId { get { return _primeWeaponDragSourceId; } set {if(_primeWeaponDragSourceId != value) Set(ref _primeWeaponDragSourceId, value, "PrimeWeaponDragSourceId"); } }
        public int PrimeWeaponDragSourceSlot { get { return _primeWeaponDragSourceSlot; } set {if(_primeWeaponDragSourceSlot != value) Set(ref _primeWeaponDragSourceSlot, value, "PrimeWeaponDragSourceSlot"); } }
        public string PrimeWeaponName { get { return _primeWeaponName; } set {if(_primeWeaponName != value) Set(ref _primeWeaponName, value, "PrimeWeaponName"); } }
        public Sprite PrimeWeaponIcon { get { return _primeWeaponIcon; } set {if(_primeWeaponIcon != value) Set(ref _primeWeaponIcon, value, "PrimeWeaponIcon"); if(null != _view && null != _view.PrimeWeaponIcon && null == value) _view.PrimeWeaponIcon.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite PrimeMuzzle { get { return _primeMuzzle; } set {if(_primeMuzzle != value) Set(ref _primeMuzzle, value, "PrimeMuzzle"); if(null != _view && null != _view.PrimeMuzzle && null == value) _view.PrimeMuzzle.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite PrimeUpperRail { get { return _primeUpperRail; } set {if(_primeUpperRail != value) Set(ref _primeUpperRail, value, "PrimeUpperRail"); if(null != _view && null != _view.PrimeUpperRail && null == value) _view.PrimeUpperRail.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite PrimeLowerRail { get { return _primeLowerRail; } set {if(_primeLowerRail != value) Set(ref _primeLowerRail, value, "PrimeLowerRail"); if(null != _view && null != _view.PrimeLowerRail && null == value) _view.PrimeLowerRail.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite PrimeMagzine { get { return _primeMagzine; } set {if(_primeMagzine != value) Set(ref _primeMagzine, value, "PrimeMagzine"); if(null != _view && null != _view.PrimeMagzine && null == value) _view.PrimeMagzine.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite PrimeStock { get { return _primeStock; } set {if(_primeStock != value) Set(ref _primeStock, value, "PrimeStock"); if(null != _view && null != _view.PrimeStock && null == value) _view.PrimeStock.sprite = ViewModelUtil.EmptySprite; } }
        public bool ShowSubWeapon { get { return _showSubWeapon; } set {if(_showSubWeapon != value) Set(ref _showSubWeapon, value, "ShowSubWeapon"); } }
        public IDragTarget SubWeaponDragTarget { get { return _subWeaponDragTarget; } set {if(_subWeaponDragTarget != value) Set(ref _subWeaponDragTarget, value, "SubWeaponDragTarget"); } }
        public int SubWeaponSlot { get { return _subWeaponSlot; } set {if(_subWeaponSlot != value) Set(ref _subWeaponSlot, value, "SubWeaponSlot"); } }
        public string SubWeaponName { get { return _subWeaponName; } set {if(_subWeaponName != value) Set(ref _subWeaponName, value, "SubWeaponName"); } }
        public Sprite SubWeaponIcon { get { return _subWeaponIcon; } set {if(_subWeaponIcon != value) Set(ref _subWeaponIcon, value, "SubWeaponIcon"); if(null != _view && null != _view.SubWeaponIcon && null == value) _view.SubWeaponIcon.sprite = ViewModelUtil.EmptySprite; } }
        public IDragTarget PistolDragTarget { get { return _pistolDragTarget; } set {if(_pistolDragTarget != value) Set(ref _pistolDragTarget, value, "PistolDragTarget"); } }
        public int PistolSlot { get { return _pistolSlot; } set {if(_pistolSlot != value) Set(ref _pistolSlot, value, "PistolSlot"); } }
        public bool ShowPistol { get { return _showPistol; } set {if(_showPistol != value) Set(ref _showPistol, value, "ShowPistol"); } }
        public Sprite PistolIcon { get { return _pistolIcon; } set {if(_pistolIcon != value) Set(ref _pistolIcon, value, "PistolIcon"); if(null != _view && null != _view.PistolIcon && null == value) _view.PistolIcon.sprite = ViewModelUtil.EmptySprite; } }
        public string PistolName { get { return _pistolName; } set {if(_pistolName != value) Set(ref _pistolName, value, "PistolName"); } }
        public bool ShowMelee { get { return _showMelee; } set {if(_showMelee != value) Set(ref _showMelee, value, "ShowMelee"); } }
        public IDragTarget MeleeDragTarget { get { return _meleeDragTarget; } set {if(_meleeDragTarget != value) Set(ref _meleeDragTarget, value, "MeleeDragTarget"); } }
        public int MeleeSlot { get { return _meleeSlot; } set {if(_meleeSlot != value) Set(ref _meleeSlot, value, "MeleeSlot"); } }
        public string MeleeName { get { return _meleeName; } set {if(_meleeName != value) Set(ref _meleeName, value, "MeleeName"); } }
        public Sprite MeleeIcon { get { return _meleeIcon; } set {if(_meleeIcon != value) Set(ref _meleeIcon, value, "MeleeIcon"); if(null != _view && null != _view.MeleeIcon && null == value) _view.MeleeIcon.sprite = ViewModelUtil.EmptySprite; } }
        public bool ShowGrenade { get { return _showGrenade; } set {if(_showGrenade != value) Set(ref _showGrenade, value, "ShowGrenade"); } }
        public IDragTarget GrenadeDragTarget { get { return _grenadeDragTarget; } set {if(_grenadeDragTarget != value) Set(ref _grenadeDragTarget, value, "GrenadeDragTarget"); } }
        public int GrenadeSlot { get { return _grenadeSlot; } set {if(_grenadeSlot != value) Set(ref _grenadeSlot, value, "GrenadeSlot"); } }
        public string GrenadeName { get { return _grenadeName; } set {if(_grenadeName != value) Set(ref _grenadeName, value, "GrenadeName"); } }
        public Sprite GrenadeIcon { get { return _grenadeIcon; } set {if(_grenadeIcon != value) Set(ref _grenadeIcon, value, "GrenadeIcon"); if(null != _view && null != _view.GrenadeIcon && null == value) _view.GrenadeIcon.sprite = ViewModelUtil.EmptySprite; } }
        public IDragTarget NearByBgDragTarget { get { return _nearByBgDragTarget; } set {if(_nearByBgDragTarget != value) Set(ref _nearByBgDragTarget, value, "NearByBgDragTarget"); } }
        public int NearByBgDropSlot { get { return _nearByBgDropSlot; } set {if(_nearByBgDropSlot != value) Set(ref _nearByBgDropSlot, value, "NearByBgDropSlot"); } }
        public bool ShowNearbyItem1 { get { return _showNearbyItem1; } set {if(_showNearbyItem1 != value) Set(ref _showNearbyItem1, value, "ShowNearbyItem1"); } }
        public IDragSource NearByDragSource1 { get { return _nearByDragSource1; } set {if(_nearByDragSource1 != value) Set(ref _nearByDragSource1, value, "NearByDragSource1"); } }
        public int NearByDragSourceId1 { get { return _nearByDragSourceId1; } set {if(_nearByDragSourceId1 != value) Set(ref _nearByDragSourceId1, value, "NearByDragSourceId1"); } }
        public int NearByDragSourceSlot1 { get { return _nearByDragSourceSlot1; } set {if(_nearByDragSourceSlot1 != value) Set(ref _nearByDragSourceSlot1, value, "NearByDragSourceSlot1"); } }
        public Sprite NearbyItemIcon1 { get { return _nearbyItemIcon1; } set {if(_nearbyItemIcon1 != value) Set(ref _nearbyItemIcon1, value, "NearbyItemIcon1"); if(null != _view && null != _view.NearbyItemIcon1 && null == value) _view.NearbyItemIcon1.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName1 { get { return _nearbyItemName1; } set {if(_nearbyItemName1 != value) Set(ref _nearbyItemName1, value, "NearbyItemName1"); } }
        public string NearbyItemNum1 { get { return _nearbyItemNum1; } set {if(_nearbyItemNum1 != value) Set(ref _nearbyItemNum1, value, "NearbyItemNum1"); } }
        public bool ShowNearbyItem2 { get { return _showNearbyItem2; } set {if(_showNearbyItem2 != value) Set(ref _showNearbyItem2, value, "ShowNearbyItem2"); } }
        public IDragSource NearByDragSource2 { get { return _nearByDragSource2; } set {if(_nearByDragSource2 != value) Set(ref _nearByDragSource2, value, "NearByDragSource2"); } }
        public int NearByDragSourceId2 { get { return _nearByDragSourceId2; } set {if(_nearByDragSourceId2 != value) Set(ref _nearByDragSourceId2, value, "NearByDragSourceId2"); } }
        public int NearByDragSourceSlot2 { get { return _nearByDragSourceSlot2; } set {if(_nearByDragSourceSlot2 != value) Set(ref _nearByDragSourceSlot2, value, "NearByDragSourceSlot2"); } }
        public Sprite NearbyItemIcon2 { get { return _nearbyItemIcon2; } set {if(_nearbyItemIcon2 != value) Set(ref _nearbyItemIcon2, value, "NearbyItemIcon2"); if(null != _view && null != _view.NearbyItemIcon2 && null == value) _view.NearbyItemIcon2.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName2 { get { return _nearbyItemName2; } set {if(_nearbyItemName2 != value) Set(ref _nearbyItemName2, value, "NearbyItemName2"); } }
        public string NearbyItemNum2 { get { return _nearbyItemNum2; } set {if(_nearbyItemNum2 != value) Set(ref _nearbyItemNum2, value, "NearbyItemNum2"); } }
        public bool ShowNearbyItem3 { get { return _showNearbyItem3; } set {if(_showNearbyItem3 != value) Set(ref _showNearbyItem3, value, "ShowNearbyItem3"); } }
        public IDragSource NearByDragSource3 { get { return _nearByDragSource3; } set {if(_nearByDragSource3 != value) Set(ref _nearByDragSource3, value, "NearByDragSource3"); } }
        public int NearByDragSourceId3 { get { return _nearByDragSourceId3; } set {if(_nearByDragSourceId3 != value) Set(ref _nearByDragSourceId3, value, "NearByDragSourceId3"); } }
        public int NearByDragSourceSlot3 { get { return _nearByDragSourceSlot3; } set {if(_nearByDragSourceSlot3 != value) Set(ref _nearByDragSourceSlot3, value, "NearByDragSourceSlot3"); } }
        public Sprite NearbyItemIcon3 { get { return _nearbyItemIcon3; } set {if(_nearbyItemIcon3 != value) Set(ref _nearbyItemIcon3, value, "NearbyItemIcon3"); if(null != _view && null != _view.NearbyItemIcon3 && null == value) _view.NearbyItemIcon3.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName3 { get { return _nearbyItemName3; } set {if(_nearbyItemName3 != value) Set(ref _nearbyItemName3, value, "NearbyItemName3"); } }
        public string NearbyItemNum3 { get { return _nearbyItemNum3; } set {if(_nearbyItemNum3 != value) Set(ref _nearbyItemNum3, value, "NearbyItemNum3"); } }
        public bool ShowNearbyItem4 { get { return _showNearbyItem4; } set {if(_showNearbyItem4 != value) Set(ref _showNearbyItem4, value, "ShowNearbyItem4"); } }
        public IDragSource NearByDragSource4 { get { return _nearByDragSource4; } set {if(_nearByDragSource4 != value) Set(ref _nearByDragSource4, value, "NearByDragSource4"); } }
        public int NearByDragSourceId4 { get { return _nearByDragSourceId4; } set {if(_nearByDragSourceId4 != value) Set(ref _nearByDragSourceId4, value, "NearByDragSourceId4"); } }
        public int NearByDragSourceSlot4 { get { return _nearByDragSourceSlot4; } set {if(_nearByDragSourceSlot4 != value) Set(ref _nearByDragSourceSlot4, value, "NearByDragSourceSlot4"); } }
        public Sprite NearbyItemIcon4 { get { return _nearbyItemIcon4; } set {if(_nearbyItemIcon4 != value) Set(ref _nearbyItemIcon4, value, "NearbyItemIcon4"); if(null != _view && null != _view.NearbyItemIcon4 && null == value) _view.NearbyItemIcon4.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName4 { get { return _nearbyItemName4; } set {if(_nearbyItemName4 != value) Set(ref _nearbyItemName4, value, "NearbyItemName4"); } }
        public string NearbyItemNum4 { get { return _nearbyItemNum4; } set {if(_nearbyItemNum4 != value) Set(ref _nearbyItemNum4, value, "NearbyItemNum4"); } }
        public bool ShowNearbyItem5 { get { return _showNearbyItem5; } set {if(_showNearbyItem5 != value) Set(ref _showNearbyItem5, value, "ShowNearbyItem5"); } }
        public IDragSource NearByDragSource5 { get { return _nearByDragSource5; } set {if(_nearByDragSource5 != value) Set(ref _nearByDragSource5, value, "NearByDragSource5"); } }
        public int NearByDragSourceId5 { get { return _nearByDragSourceId5; } set {if(_nearByDragSourceId5 != value) Set(ref _nearByDragSourceId5, value, "NearByDragSourceId5"); } }
        public int NearByDragSourceSlot5 { get { return _nearByDragSourceSlot5; } set {if(_nearByDragSourceSlot5 != value) Set(ref _nearByDragSourceSlot5, value, "NearByDragSourceSlot5"); } }
        public Sprite NearbyItemIcon5 { get { return _nearbyItemIcon5; } set {if(_nearbyItemIcon5 != value) Set(ref _nearbyItemIcon5, value, "NearbyItemIcon5"); if(null != _view && null != _view.NearbyItemIcon5 && null == value) _view.NearbyItemIcon5.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName5 { get { return _nearbyItemName5; } set {if(_nearbyItemName5 != value) Set(ref _nearbyItemName5, value, "NearbyItemName5"); } }
        public string NearbyItemNum5 { get { return _nearbyItemNum5; } set {if(_nearbyItemNum5 != value) Set(ref _nearbyItemNum5, value, "NearbyItemNum5"); } }
        public bool ShowNearbyItem6 { get { return _showNearbyItem6; } set {if(_showNearbyItem6 != value) Set(ref _showNearbyItem6, value, "ShowNearbyItem6"); } }
        public IDragSource NearByDragSource6 { get { return _nearByDragSource6; } set {if(_nearByDragSource6 != value) Set(ref _nearByDragSource6, value, "NearByDragSource6"); } }
        public int NearByDragSourceId6 { get { return _nearByDragSourceId6; } set {if(_nearByDragSourceId6 != value) Set(ref _nearByDragSourceId6, value, "NearByDragSourceId6"); } }
        public int NearByDragSourceSlot6 { get { return _nearByDragSourceSlot6; } set {if(_nearByDragSourceSlot6 != value) Set(ref _nearByDragSourceSlot6, value, "NearByDragSourceSlot6"); } }
        public Sprite NearbyItemIcon6 { get { return _nearbyItemIcon6; } set {if(_nearbyItemIcon6 != value) Set(ref _nearbyItemIcon6, value, "NearbyItemIcon6"); if(null != _view && null != _view.NearbyItemIcon6 && null == value) _view.NearbyItemIcon6.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName6 { get { return _nearbyItemName6; } set {if(_nearbyItemName6 != value) Set(ref _nearbyItemName6, value, "NearbyItemName6"); } }
        public string NearbyItemNum6 { get { return _nearbyItemNum6; } set {if(_nearbyItemNum6 != value) Set(ref _nearbyItemNum6, value, "NearbyItemNum6"); } }
        public bool ShowNearbyItem7 { get { return _showNearbyItem7; } set {if(_showNearbyItem7 != value) Set(ref _showNearbyItem7, value, "ShowNearbyItem7"); } }
        public IDragSource NearByDragSource7 { get { return _nearByDragSource7; } set {if(_nearByDragSource7 != value) Set(ref _nearByDragSource7, value, "NearByDragSource7"); } }
        public int NearByDragSourceId7 { get { return _nearByDragSourceId7; } set {if(_nearByDragSourceId7 != value) Set(ref _nearByDragSourceId7, value, "NearByDragSourceId7"); } }
        public int NearByDragSourceSlot7 { get { return _nearByDragSourceSlot7; } set {if(_nearByDragSourceSlot7 != value) Set(ref _nearByDragSourceSlot7, value, "NearByDragSourceSlot7"); } }
        public Sprite NearbyItemIcon7 { get { return _nearbyItemIcon7; } set {if(_nearbyItemIcon7 != value) Set(ref _nearbyItemIcon7, value, "NearbyItemIcon7"); if(null != _view && null != _view.NearbyItemIcon7 && null == value) _view.NearbyItemIcon7.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName7 { get { return _nearbyItemName7; } set {if(_nearbyItemName7 != value) Set(ref _nearbyItemName7, value, "NearbyItemName7"); } }
        public string NearbyItemNum7 { get { return _nearbyItemNum7; } set {if(_nearbyItemNum7 != value) Set(ref _nearbyItemNum7, value, "NearbyItemNum7"); } }
        public bool ShowNearbyItem8 { get { return _showNearbyItem8; } set {if(_showNearbyItem8 != value) Set(ref _showNearbyItem8, value, "ShowNearbyItem8"); } }
        public IDragSource NearByDragSource8 { get { return _nearByDragSource8; } set {if(_nearByDragSource8 != value) Set(ref _nearByDragSource8, value, "NearByDragSource8"); } }
        public int NearByDragSourceId8 { get { return _nearByDragSourceId8; } set {if(_nearByDragSourceId8 != value) Set(ref _nearByDragSourceId8, value, "NearByDragSourceId8"); } }
        public int NearByDragSourceSlot8 { get { return _nearByDragSourceSlot8; } set {if(_nearByDragSourceSlot8 != value) Set(ref _nearByDragSourceSlot8, value, "NearByDragSourceSlot8"); } }
        public Sprite NearbyItemIcon8 { get { return _nearbyItemIcon8; } set {if(_nearbyItemIcon8 != value) Set(ref _nearbyItemIcon8, value, "NearbyItemIcon8"); if(null != _view && null != _view.NearbyItemIcon8 && null == value) _view.NearbyItemIcon8.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName8 { get { return _nearbyItemName8; } set {if(_nearbyItemName8 != value) Set(ref _nearbyItemName8, value, "NearbyItemName8"); } }
        public string NearbyItemNum8 { get { return _nearbyItemNum8; } set {if(_nearbyItemNum8 != value) Set(ref _nearbyItemNum8, value, "NearbyItemNum8"); } }
        public bool ShowNearbyItem9 { get { return _showNearbyItem9; } set {if(_showNearbyItem9 != value) Set(ref _showNearbyItem9, value, "ShowNearbyItem9"); } }
        public IDragSource NearByDragSource9 { get { return _nearByDragSource9; } set {if(_nearByDragSource9 != value) Set(ref _nearByDragSource9, value, "NearByDragSource9"); } }
        public int NearByDragSourceId9 { get { return _nearByDragSourceId9; } set {if(_nearByDragSourceId9 != value) Set(ref _nearByDragSourceId9, value, "NearByDragSourceId9"); } }
        public int NearByDragSourceSlot9 { get { return _nearByDragSourceSlot9; } set {if(_nearByDragSourceSlot9 != value) Set(ref _nearByDragSourceSlot9, value, "NearByDragSourceSlot9"); } }
        public Sprite NearbyItemIcon9 { get { return _nearbyItemIcon9; } set {if(_nearbyItemIcon9 != value) Set(ref _nearbyItemIcon9, value, "NearbyItemIcon9"); if(null != _view && null != _view.NearbyItemIcon9 && null == value) _view.NearbyItemIcon9.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName9 { get { return _nearbyItemName9; } set {if(_nearbyItemName9 != value) Set(ref _nearbyItemName9, value, "NearbyItemName9"); } }
        public string NearbyItemNum9 { get { return _nearbyItemNum9; } set {if(_nearbyItemNum9 != value) Set(ref _nearbyItemNum9, value, "NearbyItemNum9"); } }
        public bool ShowNearbyItem10 { get { return _showNearbyItem10; } set {if(_showNearbyItem10 != value) Set(ref _showNearbyItem10, value, "ShowNearbyItem10"); } }
        public IDragSource NearByDragSource10 { get { return _nearByDragSource10; } set {if(_nearByDragSource10 != value) Set(ref _nearByDragSource10, value, "NearByDragSource10"); } }
        public int NearByDragSourceId10 { get { return _nearByDragSourceId10; } set {if(_nearByDragSourceId10 != value) Set(ref _nearByDragSourceId10, value, "NearByDragSourceId10"); } }
        public int NearByDragSourceSlot10 { get { return _nearByDragSourceSlot10; } set {if(_nearByDragSourceSlot10 != value) Set(ref _nearByDragSourceSlot10, value, "NearByDragSourceSlot10"); } }
        public Sprite NearbyItemIcon10 { get { return _nearbyItemIcon10; } set {if(_nearbyItemIcon10 != value) Set(ref _nearbyItemIcon10, value, "NearbyItemIcon10"); if(null != _view && null != _view.NearbyItemIcon10 && null == value) _view.NearbyItemIcon10.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName10 { get { return _nearbyItemName10; } set {if(_nearbyItemName10 != value) Set(ref _nearbyItemName10, value, "NearbyItemName10"); } }
        public string NearbyItemNum10 { get { return _nearbyItemNum10; } set {if(_nearbyItemNum10 != value) Set(ref _nearbyItemNum10, value, "NearbyItemNum10"); } }
        public bool ShowNearbyItem11 { get { return _showNearbyItem11; } set {if(_showNearbyItem11 != value) Set(ref _showNearbyItem11, value, "ShowNearbyItem11"); } }
        public IDragSource NearByDragSource11 { get { return _nearByDragSource11; } set {if(_nearByDragSource11 != value) Set(ref _nearByDragSource11, value, "NearByDragSource11"); } }
        public int NearByDragSourceId11 { get { return _nearByDragSourceId11; } set {if(_nearByDragSourceId11 != value) Set(ref _nearByDragSourceId11, value, "NearByDragSourceId11"); } }
        public int NearByDragSourceSlot11 { get { return _nearByDragSourceSlot11; } set {if(_nearByDragSourceSlot11 != value) Set(ref _nearByDragSourceSlot11, value, "NearByDragSourceSlot11"); } }
        public Sprite NearbyItemIcon11 { get { return _nearbyItemIcon11; } set {if(_nearbyItemIcon11 != value) Set(ref _nearbyItemIcon11, value, "NearbyItemIcon11"); if(null != _view && null != _view.NearbyItemIcon11 && null == value) _view.NearbyItemIcon11.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName11 { get { return _nearbyItemName11; } set {if(_nearbyItemName11 != value) Set(ref _nearbyItemName11, value, "NearbyItemName11"); } }
        public string NearbyItemNum11 { get { return _nearbyItemNum11; } set {if(_nearbyItemNum11 != value) Set(ref _nearbyItemNum11, value, "NearbyItemNum11"); } }
        public bool ShowNearbyItem12 { get { return _showNearbyItem12; } set {if(_showNearbyItem12 != value) Set(ref _showNearbyItem12, value, "ShowNearbyItem12"); } }
        public IDragSource NearByDragSource12 { get { return _nearByDragSource12; } set {if(_nearByDragSource12 != value) Set(ref _nearByDragSource12, value, "NearByDragSource12"); } }
        public int NearByDragSourceId12 { get { return _nearByDragSourceId12; } set {if(_nearByDragSourceId12 != value) Set(ref _nearByDragSourceId12, value, "NearByDragSourceId12"); } }
        public int NearByDragSourceSlot12 { get { return _nearByDragSourceSlot12; } set {if(_nearByDragSourceSlot12 != value) Set(ref _nearByDragSourceSlot12, value, "NearByDragSourceSlot12"); } }
        public Sprite NearbyItemIcon12 { get { return _nearbyItemIcon12; } set {if(_nearbyItemIcon12 != value) Set(ref _nearbyItemIcon12, value, "NearbyItemIcon12"); if(null != _view && null != _view.NearbyItemIcon12 && null == value) _view.NearbyItemIcon12.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName12 { get { return _nearbyItemName12; } set {if(_nearbyItemName12 != value) Set(ref _nearbyItemName12, value, "NearbyItemName12"); } }
        public string NearbyItemNum12 { get { return _nearbyItemNum12; } set {if(_nearbyItemNum12 != value) Set(ref _nearbyItemNum12, value, "NearbyItemNum12"); } }
        public bool ShowNearbyItem13 { get { return _showNearbyItem13; } set {if(_showNearbyItem13 != value) Set(ref _showNearbyItem13, value, "ShowNearbyItem13"); } }
        public IDragSource NearByDragSource13 { get { return _nearByDragSource13; } set {if(_nearByDragSource13 != value) Set(ref _nearByDragSource13, value, "NearByDragSource13"); } }
        public int NearByDragSourceId13 { get { return _nearByDragSourceId13; } set {if(_nearByDragSourceId13 != value) Set(ref _nearByDragSourceId13, value, "NearByDragSourceId13"); } }
        public int NearByDragSourceSlot13 { get { return _nearByDragSourceSlot13; } set {if(_nearByDragSourceSlot13 != value) Set(ref _nearByDragSourceSlot13, value, "NearByDragSourceSlot13"); } }
        public Sprite NearbyItemIcon13 { get { return _nearbyItemIcon13; } set {if(_nearbyItemIcon13 != value) Set(ref _nearbyItemIcon13, value, "NearbyItemIcon13"); if(null != _view && null != _view.NearbyItemIcon13 && null == value) _view.NearbyItemIcon13.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName13 { get { return _nearbyItemName13; } set {if(_nearbyItemName13 != value) Set(ref _nearbyItemName13, value, "NearbyItemName13"); } }
        public string NearbyItemNum13 { get { return _nearbyItemNum13; } set {if(_nearbyItemNum13 != value) Set(ref _nearbyItemNum13, value, "NearbyItemNum13"); } }
        public bool ShowNearbyItem14 { get { return _showNearbyItem14; } set {if(_showNearbyItem14 != value) Set(ref _showNearbyItem14, value, "ShowNearbyItem14"); } }
        public IDragSource NearByDragSource14 { get { return _nearByDragSource14; } set {if(_nearByDragSource14 != value) Set(ref _nearByDragSource14, value, "NearByDragSource14"); } }
        public int NearByDragSourceId14 { get { return _nearByDragSourceId14; } set {if(_nearByDragSourceId14 != value) Set(ref _nearByDragSourceId14, value, "NearByDragSourceId14"); } }
        public int NearByDragSourceSlot14 { get { return _nearByDragSourceSlot14; } set {if(_nearByDragSourceSlot14 != value) Set(ref _nearByDragSourceSlot14, value, "NearByDragSourceSlot14"); } }
        public Sprite NearbyItemIcon14 { get { return _nearbyItemIcon14; } set {if(_nearbyItemIcon14 != value) Set(ref _nearbyItemIcon14, value, "NearbyItemIcon14"); if(null != _view && null != _view.NearbyItemIcon14 && null == value) _view.NearbyItemIcon14.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName14 { get { return _nearbyItemName14; } set {if(_nearbyItemName14 != value) Set(ref _nearbyItemName14, value, "NearbyItemName14"); } }
        public string NearbyItemNum14 { get { return _nearbyItemNum14; } set {if(_nearbyItemNum14 != value) Set(ref _nearbyItemNum14, value, "NearbyItemNum14"); } }
        public bool ShowNearbyItem15 { get { return _showNearbyItem15; } set {if(_showNearbyItem15 != value) Set(ref _showNearbyItem15, value, "ShowNearbyItem15"); } }
        public IDragSource NearByDragSource15 { get { return _nearByDragSource15; } set {if(_nearByDragSource15 != value) Set(ref _nearByDragSource15, value, "NearByDragSource15"); } }
        public int NearByDragSourceId15 { get { return _nearByDragSourceId15; } set {if(_nearByDragSourceId15 != value) Set(ref _nearByDragSourceId15, value, "NearByDragSourceId15"); } }
        public int NearByDragSourceSlot15 { get { return _nearByDragSourceSlot15; } set {if(_nearByDragSourceSlot15 != value) Set(ref _nearByDragSourceSlot15, value, "NearByDragSourceSlot15"); } }
        public Sprite NearbyItemIcon15 { get { return _nearbyItemIcon15; } set {if(_nearbyItemIcon15 != value) Set(ref _nearbyItemIcon15, value, "NearbyItemIcon15"); if(null != _view && null != _view.NearbyItemIcon15 && null == value) _view.NearbyItemIcon15.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName15 { get { return _nearbyItemName15; } set {if(_nearbyItemName15 != value) Set(ref _nearbyItemName15, value, "NearbyItemName15"); } }
        public string NearbyItemNum15 { get { return _nearbyItemNum15; } set {if(_nearbyItemNum15 != value) Set(ref _nearbyItemNum15, value, "NearbyItemNum15"); } }
        public bool ShowNearbyItem16 { get { return _showNearbyItem16; } set {if(_showNearbyItem16 != value) Set(ref _showNearbyItem16, value, "ShowNearbyItem16"); } }
        public IDragSource NearByDragSource16 { get { return _nearByDragSource16; } set {if(_nearByDragSource16 != value) Set(ref _nearByDragSource16, value, "NearByDragSource16"); } }
        public int NearByDragSourceId16 { get { return _nearByDragSourceId16; } set {if(_nearByDragSourceId16 != value) Set(ref _nearByDragSourceId16, value, "NearByDragSourceId16"); } }
        public int NearByDragSourceSlot16 { get { return _nearByDragSourceSlot16; } set {if(_nearByDragSourceSlot16 != value) Set(ref _nearByDragSourceSlot16, value, "NearByDragSourceSlot16"); } }
        public Sprite NearbyItemIcon16 { get { return _nearbyItemIcon16; } set {if(_nearbyItemIcon16 != value) Set(ref _nearbyItemIcon16, value, "NearbyItemIcon16"); if(null != _view && null != _view.NearbyItemIcon16 && null == value) _view.NearbyItemIcon16.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName16 { get { return _nearbyItemName16; } set {if(_nearbyItemName16 != value) Set(ref _nearbyItemName16, value, "NearbyItemName16"); } }
        public string NearbyItemNum16 { get { return _nearbyItemNum16; } set {if(_nearbyItemNum16 != value) Set(ref _nearbyItemNum16, value, "NearbyItemNum16"); } }
        public bool ShowNearbyItem17 { get { return _showNearbyItem17; } set {if(_showNearbyItem17 != value) Set(ref _showNearbyItem17, value, "ShowNearbyItem17"); } }
        public IDragSource NearByDragSource17 { get { return _nearByDragSource17; } set {if(_nearByDragSource17 != value) Set(ref _nearByDragSource17, value, "NearByDragSource17"); } }
        public int NearByDragSourceId17 { get { return _nearByDragSourceId17; } set {if(_nearByDragSourceId17 != value) Set(ref _nearByDragSourceId17, value, "NearByDragSourceId17"); } }
        public int NearByDragSourceSlot17 { get { return _nearByDragSourceSlot17; } set {if(_nearByDragSourceSlot17 != value) Set(ref _nearByDragSourceSlot17, value, "NearByDragSourceSlot17"); } }
        public Sprite NearbyItemIcon17 { get { return _nearbyItemIcon17; } set {if(_nearbyItemIcon17 != value) Set(ref _nearbyItemIcon17, value, "NearbyItemIcon17"); if(null != _view && null != _view.NearbyItemIcon17 && null == value) _view.NearbyItemIcon17.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName17 { get { return _nearbyItemName17; } set {if(_nearbyItemName17 != value) Set(ref _nearbyItemName17, value, "NearbyItemName17"); } }
        public string NearbyItemNum17 { get { return _nearbyItemNum17; } set {if(_nearbyItemNum17 != value) Set(ref _nearbyItemNum17, value, "NearbyItemNum17"); } }
        public bool ShowNearbyItem18 { get { return _showNearbyItem18; } set {if(_showNearbyItem18 != value) Set(ref _showNearbyItem18, value, "ShowNearbyItem18"); } }
        public IDragSource NearByDragSource18 { get { return _nearByDragSource18; } set {if(_nearByDragSource18 != value) Set(ref _nearByDragSource18, value, "NearByDragSource18"); } }
        public int NearByDragSourceId18 { get { return _nearByDragSourceId18; } set {if(_nearByDragSourceId18 != value) Set(ref _nearByDragSourceId18, value, "NearByDragSourceId18"); } }
        public int NearByDragSourceSlot18 { get { return _nearByDragSourceSlot18; } set {if(_nearByDragSourceSlot18 != value) Set(ref _nearByDragSourceSlot18, value, "NearByDragSourceSlot18"); } }
        public Sprite NearbyItemIcon18 { get { return _nearbyItemIcon18; } set {if(_nearbyItemIcon18 != value) Set(ref _nearbyItemIcon18, value, "NearbyItemIcon18"); if(null != _view && null != _view.NearbyItemIcon18 && null == value) _view.NearbyItemIcon18.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName18 { get { return _nearbyItemName18; } set {if(_nearbyItemName18 != value) Set(ref _nearbyItemName18, value, "NearbyItemName18"); } }
        public string NearbyItemNum18 { get { return _nearbyItemNum18; } set {if(_nearbyItemNum18 != value) Set(ref _nearbyItemNum18, value, "NearbyItemNum18"); } }
        public bool ShowNearbyItem19 { get { return _showNearbyItem19; } set {if(_showNearbyItem19 != value) Set(ref _showNearbyItem19, value, "ShowNearbyItem19"); } }
        public IDragSource NearByDragSource19 { get { return _nearByDragSource19; } set {if(_nearByDragSource19 != value) Set(ref _nearByDragSource19, value, "NearByDragSource19"); } }
        public int NearByDragSourceId19 { get { return _nearByDragSourceId19; } set {if(_nearByDragSourceId19 != value) Set(ref _nearByDragSourceId19, value, "NearByDragSourceId19"); } }
        public int NearByDragSourceSlot19 { get { return _nearByDragSourceSlot19; } set {if(_nearByDragSourceSlot19 != value) Set(ref _nearByDragSourceSlot19, value, "NearByDragSourceSlot19"); } }
        public Sprite NearbyItemIcon19 { get { return _nearbyItemIcon19; } set {if(_nearbyItemIcon19 != value) Set(ref _nearbyItemIcon19, value, "NearbyItemIcon19"); if(null != _view && null != _view.NearbyItemIcon19 && null == value) _view.NearbyItemIcon19.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName19 { get { return _nearbyItemName19; } set {if(_nearbyItemName19 != value) Set(ref _nearbyItemName19, value, "NearbyItemName19"); } }
        public string NearbyItemNum19 { get { return _nearbyItemNum19; } set {if(_nearbyItemNum19 != value) Set(ref _nearbyItemNum19, value, "NearbyItemNum19"); } }
        public bool ShowNearbyItem20 { get { return _showNearbyItem20; } set {if(_showNearbyItem20 != value) Set(ref _showNearbyItem20, value, "ShowNearbyItem20"); } }
        public IDragSource NearByDragSource20 { get { return _nearByDragSource20; } set {if(_nearByDragSource20 != value) Set(ref _nearByDragSource20, value, "NearByDragSource20"); } }
        public int NearByDragSourceId20 { get { return _nearByDragSourceId20; } set {if(_nearByDragSourceId20 != value) Set(ref _nearByDragSourceId20, value, "NearByDragSourceId20"); } }
        public int NearByDragSourceSlot20 { get { return _nearByDragSourceSlot20; } set {if(_nearByDragSourceSlot20 != value) Set(ref _nearByDragSourceSlot20, value, "NearByDragSourceSlot20"); } }
        public Sprite NearbyItemIcon20 { get { return _nearbyItemIcon20; } set {if(_nearbyItemIcon20 != value) Set(ref _nearbyItemIcon20, value, "NearbyItemIcon20"); if(null != _view && null != _view.NearbyItemIcon20 && null == value) _view.NearbyItemIcon20.sprite = ViewModelUtil.EmptySprite; } }
        public string NearbyItemName20 { get { return _nearbyItemName20; } set {if(_nearbyItemName20 != value) Set(ref _nearbyItemName20, value, "NearbyItemName20"); } }
        public string NearbyItemNum20 { get { return _nearbyItemNum20; } set {if(_nearbyItemNum20 != value) Set(ref _nearbyItemNum20, value, "NearbyItemNum20"); } }
        public bool ShowInventoryItem1 { get { return _showInventoryItem1; } set {if(_showInventoryItem1 != value) Set(ref _showInventoryItem1, value, "ShowInventoryItem1"); } }
        public IDragTarget InventoryDragTarget1 { get { return _inventoryDragTarget1; } set {if(_inventoryDragTarget1 != value) Set(ref _inventoryDragTarget1, value, "InventoryDragTarget1"); } }
        public int InventoryDropSlot1 { get { return _inventoryDropSlot1; } set {if(_inventoryDropSlot1 != value) Set(ref _inventoryDropSlot1, value, "InventoryDropSlot1"); } }
        public IDragSource InventoryDragSource1 { get { return _inventoryDragSource1; } set {if(_inventoryDragSource1 != value) Set(ref _inventoryDragSource1, value, "InventoryDragSource1"); } }
        public int InventoryDragSourceId1 { get { return _inventoryDragSourceId1; } set {if(_inventoryDragSourceId1 != value) Set(ref _inventoryDragSourceId1, value, "InventoryDragSourceId1"); } }
        public int InventoryDragSourceSlot1 { get { return _inventoryDragSourceSlot1; } set {if(_inventoryDragSourceSlot1 != value) Set(ref _inventoryDragSourceSlot1, value, "InventoryDragSourceSlot1"); } }
        public Sprite InventoryItemIcon1 { get { return _inventoryItemIcon1; } set {if(_inventoryItemIcon1 != value) Set(ref _inventoryItemIcon1, value, "InventoryItemIcon1"); if(null != _view && null != _view.InventoryItemIcon1 && null == value) _view.InventoryItemIcon1.sprite = ViewModelUtil.EmptySprite; } }
        public string InventoryItemName1 { get { return _inventoryItemName1; } set {if(_inventoryItemName1 != value) Set(ref _inventoryItemName1, value, "InventoryItemName1"); } }
        public string InventoryItemNum1 { get { return _inventoryItemNum1; } set {if(_inventoryItemNum1 != value) Set(ref _inventoryItemNum1, value, "InventoryItemNum1"); } }
        public bool ShowInventoryItem2 { get { return _showInventoryItem2; } set {if(_showInventoryItem2 != value) Set(ref _showInventoryItem2, value, "ShowInventoryItem2"); } }
        public IDragTarget InventoryDragTarget2 { get { return _inventoryDragTarget2; } set {if(_inventoryDragTarget2 != value) Set(ref _inventoryDragTarget2, value, "InventoryDragTarget2"); } }
        public int InventoryDropSlot2 { get { return _inventoryDropSlot2; } set {if(_inventoryDropSlot2 != value) Set(ref _inventoryDropSlot2, value, "InventoryDropSlot2"); } }
        public IDragSource InventoryDragSource2 { get { return _inventoryDragSource2; } set {if(_inventoryDragSource2 != value) Set(ref _inventoryDragSource2, value, "InventoryDragSource2"); } }
        public int InventoryDragSourceId2 { get { return _inventoryDragSourceId2; } set {if(_inventoryDragSourceId2 != value) Set(ref _inventoryDragSourceId2, value, "InventoryDragSourceId2"); } }
        public int InventoryDragSourceSlot2 { get { return _inventoryDragSourceSlot2; } set {if(_inventoryDragSourceSlot2 != value) Set(ref _inventoryDragSourceSlot2, value, "InventoryDragSourceSlot2"); } }
        public Sprite InventoryItemIcon2 { get { return _inventoryItemIcon2; } set {if(_inventoryItemIcon2 != value) Set(ref _inventoryItemIcon2, value, "InventoryItemIcon2"); if(null != _view && null != _view.InventoryItemIcon2 && null == value) _view.InventoryItemIcon2.sprite = ViewModelUtil.EmptySprite; } }
        public string InventoryItemName2 { get { return _inventoryItemName2; } set {if(_inventoryItemName2 != value) Set(ref _inventoryItemName2, value, "InventoryItemName2"); } }
        public string InventoryItemNum2 { get { return _inventoryItemNum2; } set {if(_inventoryItemNum2 != value) Set(ref _inventoryItemNum2, value, "InventoryItemNum2"); } }
        public bool ShowInventoryItem3 { get { return _showInventoryItem3; } set {if(_showInventoryItem3 != value) Set(ref _showInventoryItem3, value, "ShowInventoryItem3"); } }
        public IDragTarget InventoryDragTarget3 { get { return _inventoryDragTarget3; } set {if(_inventoryDragTarget3 != value) Set(ref _inventoryDragTarget3, value, "InventoryDragTarget3"); } }
        public int InventoryDropSlot3 { get { return _inventoryDropSlot3; } set {if(_inventoryDropSlot3 != value) Set(ref _inventoryDropSlot3, value, "InventoryDropSlot3"); } }
        public IDragSource InventoryDragSource3 { get { return _inventoryDragSource3; } set {if(_inventoryDragSource3 != value) Set(ref _inventoryDragSource3, value, "InventoryDragSource3"); } }
        public int InventoryDragSourceId3 { get { return _inventoryDragSourceId3; } set {if(_inventoryDragSourceId3 != value) Set(ref _inventoryDragSourceId3, value, "InventoryDragSourceId3"); } }
        public int InventoryDragSourceSlot3 { get { return _inventoryDragSourceSlot3; } set {if(_inventoryDragSourceSlot3 != value) Set(ref _inventoryDragSourceSlot3, value, "InventoryDragSourceSlot3"); } }
        public Sprite InventoryItemIcon3 { get { return _inventoryItemIcon3; } set {if(_inventoryItemIcon3 != value) Set(ref _inventoryItemIcon3, value, "InventoryItemIcon3"); if(null != _view && null != _view.InventoryItemIcon3 && null == value) _view.InventoryItemIcon3.sprite = ViewModelUtil.EmptySprite; } }
        public string InventoryItemName3 { get { return _inventoryItemName3; } set {if(_inventoryItemName3 != value) Set(ref _inventoryItemName3, value, "InventoryItemName3"); } }
        public string InventoryItemNum3 { get { return _inventoryItemNum3; } set {if(_inventoryItemNum3 != value) Set(ref _inventoryItemNum3, value, "InventoryItemNum3"); } }
        public bool ShowInventoryItem4 { get { return _showInventoryItem4; } set {if(_showInventoryItem4 != value) Set(ref _showInventoryItem4, value, "ShowInventoryItem4"); } }
        public IDragTarget InventoryDragTarget4 { get { return _inventoryDragTarget4; } set {if(_inventoryDragTarget4 != value) Set(ref _inventoryDragTarget4, value, "InventoryDragTarget4"); } }
        public int InventoryDropSlot4 { get { return _inventoryDropSlot4; } set {if(_inventoryDropSlot4 != value) Set(ref _inventoryDropSlot4, value, "InventoryDropSlot4"); } }
        public IDragSource InventoryDragSource4 { get { return _inventoryDragSource4; } set {if(_inventoryDragSource4 != value) Set(ref _inventoryDragSource4, value, "InventoryDragSource4"); } }
        public int InventoryDragSourceId4 { get { return _inventoryDragSourceId4; } set {if(_inventoryDragSourceId4 != value) Set(ref _inventoryDragSourceId4, value, "InventoryDragSourceId4"); } }
        public int InventoryDragSourceSlot4 { get { return _inventoryDragSourceSlot4; } set {if(_inventoryDragSourceSlot4 != value) Set(ref _inventoryDragSourceSlot4, value, "InventoryDragSourceSlot4"); } }
        public Sprite InventoryItemIcon4 { get { return _inventoryItemIcon4; } set {if(_inventoryItemIcon4 != value) Set(ref _inventoryItemIcon4, value, "InventoryItemIcon4"); if(null != _view && null != _view.InventoryItemIcon4 && null == value) _view.InventoryItemIcon4.sprite = ViewModelUtil.EmptySprite; } }
        public string InventoryItemName4 { get { return _inventoryItemName4; } set {if(_inventoryItemName4 != value) Set(ref _inventoryItemName4, value, "InventoryItemName4"); } }
        public string InventoryItemNum4 { get { return _inventoryItemNum4; } set {if(_inventoryItemNum4 != value) Set(ref _inventoryItemNum4, value, "InventoryItemNum4"); } }
        public bool ShowInventoryItem5 { get { return _showInventoryItem5; } set {if(_showInventoryItem5 != value) Set(ref _showInventoryItem5, value, "ShowInventoryItem5"); } }
        public IDragTarget InventoryDragTarget5 { get { return _inventoryDragTarget5; } set {if(_inventoryDragTarget5 != value) Set(ref _inventoryDragTarget5, value, "InventoryDragTarget5"); } }
        public int InventoryDropSlot5 { get { return _inventoryDropSlot5; } set {if(_inventoryDropSlot5 != value) Set(ref _inventoryDropSlot5, value, "InventoryDropSlot5"); } }
        public IDragSource InventoryDragSource5 { get { return _inventoryDragSource5; } set {if(_inventoryDragSource5 != value) Set(ref _inventoryDragSource5, value, "InventoryDragSource5"); } }
        public int InventoryDragSourceId5 { get { return _inventoryDragSourceId5; } set {if(_inventoryDragSourceId5 != value) Set(ref _inventoryDragSourceId5, value, "InventoryDragSourceId5"); } }
        public int InventoryDragSourceSlot5 { get { return _inventoryDragSourceSlot5; } set {if(_inventoryDragSourceSlot5 != value) Set(ref _inventoryDragSourceSlot5, value, "InventoryDragSourceSlot5"); } }
        public Sprite InventoryItemIcon5 { get { return _inventoryItemIcon5; } set {if(_inventoryItemIcon5 != value) Set(ref _inventoryItemIcon5, value, "InventoryItemIcon5"); if(null != _view && null != _view.InventoryItemIcon5 && null == value) _view.InventoryItemIcon5.sprite = ViewModelUtil.EmptySprite; } }
        public string InventoryItemName5 { get { return _inventoryItemName5; } set {if(_inventoryItemName5 != value) Set(ref _inventoryItemName5, value, "InventoryItemName5"); } }
        public string InventoryItemNum5 { get { return _inventoryItemNum5; } set {if(_inventoryItemNum5 != value) Set(ref _inventoryItemNum5, value, "InventoryItemNum5"); } }
        public bool ShowInventoryItem6 { get { return _showInventoryItem6; } set {if(_showInventoryItem6 != value) Set(ref _showInventoryItem6, value, "ShowInventoryItem6"); } }
        public IDragTarget InventoryDragTarget6 { get { return _inventoryDragTarget6; } set {if(_inventoryDragTarget6 != value) Set(ref _inventoryDragTarget6, value, "InventoryDragTarget6"); } }
        public int InventoryDropSlot6 { get { return _inventoryDropSlot6; } set {if(_inventoryDropSlot6 != value) Set(ref _inventoryDropSlot6, value, "InventoryDropSlot6"); } }
        public IDragSource InventoryDragSource6 { get { return _inventoryDragSource6; } set {if(_inventoryDragSource6 != value) Set(ref _inventoryDragSource6, value, "InventoryDragSource6"); } }
        public int InventoryDragSourceId6 { get { return _inventoryDragSourceId6; } set {if(_inventoryDragSourceId6 != value) Set(ref _inventoryDragSourceId6, value, "InventoryDragSourceId6"); } }
        public int InventoryDragSourceSlot6 { get { return _inventoryDragSourceSlot6; } set {if(_inventoryDragSourceSlot6 != value) Set(ref _inventoryDragSourceSlot6, value, "InventoryDragSourceSlot6"); } }
        public Sprite InventoryItemIcon6 { get { return _inventoryItemIcon6; } set {if(_inventoryItemIcon6 != value) Set(ref _inventoryItemIcon6, value, "InventoryItemIcon6"); if(null != _view && null != _view.InventoryItemIcon6 && null == value) _view.InventoryItemIcon6.sprite = ViewModelUtil.EmptySprite; } }
        public string InventoryItemName6 { get { return _inventoryItemName6; } set {if(_inventoryItemName6 != value) Set(ref _inventoryItemName6, value, "InventoryItemName6"); } }
        public string InventoryItemNum6 { get { return _inventoryItemNum6; } set {if(_inventoryItemNum6 != value) Set(ref _inventoryItemNum6, value, "InventoryItemNum6"); } }
        public bool ShowInventoryItem7 { get { return _showInventoryItem7; } set {if(_showInventoryItem7 != value) Set(ref _showInventoryItem7, value, "ShowInventoryItem7"); } }
        public IDragTarget InventoryDragTarget7 { get { return _inventoryDragTarget7; } set {if(_inventoryDragTarget7 != value) Set(ref _inventoryDragTarget7, value, "InventoryDragTarget7"); } }
        public int InventoryDropSlot7 { get { return _inventoryDropSlot7; } set {if(_inventoryDropSlot7 != value) Set(ref _inventoryDropSlot7, value, "InventoryDropSlot7"); } }
        public IDragSource InventoryDragSource7 { get { return _inventoryDragSource7; } set {if(_inventoryDragSource7 != value) Set(ref _inventoryDragSource7, value, "InventoryDragSource7"); } }
        public int InventoryDragSourceId7 { get { return _inventoryDragSourceId7; } set {if(_inventoryDragSourceId7 != value) Set(ref _inventoryDragSourceId7, value, "InventoryDragSourceId7"); } }
        public int InventoryDragSourceSlot7 { get { return _inventoryDragSourceSlot7; } set {if(_inventoryDragSourceSlot7 != value) Set(ref _inventoryDragSourceSlot7, value, "InventoryDragSourceSlot7"); } }
        public Sprite InventoryItemIcon7 { get { return _inventoryItemIcon7; } set {if(_inventoryItemIcon7 != value) Set(ref _inventoryItemIcon7, value, "InventoryItemIcon7"); if(null != _view && null != _view.InventoryItemIcon7 && null == value) _view.InventoryItemIcon7.sprite = ViewModelUtil.EmptySprite; } }
        public string InventoryItemName7 { get { return _inventoryItemName7; } set {if(_inventoryItemName7 != value) Set(ref _inventoryItemName7, value, "InventoryItemName7"); } }
        public string InventoryItemNum7 { get { return _inventoryItemNum7; } set {if(_inventoryItemNum7 != value) Set(ref _inventoryItemNum7, value, "InventoryItemNum7"); } }
        public bool ShowInventoryItem8 { get { return _showInventoryItem8; } set {if(_showInventoryItem8 != value) Set(ref _showInventoryItem8, value, "ShowInventoryItem8"); } }
        public IDragTarget InventoryDragTarget8 { get { return _inventoryDragTarget8; } set {if(_inventoryDragTarget8 != value) Set(ref _inventoryDragTarget8, value, "InventoryDragTarget8"); } }
        public int InventoryDropSlot8 { get { return _inventoryDropSlot8; } set {if(_inventoryDropSlot8 != value) Set(ref _inventoryDropSlot8, value, "InventoryDropSlot8"); } }
        public IDragSource InventoryDragSource8 { get { return _inventoryDragSource8; } set {if(_inventoryDragSource8 != value) Set(ref _inventoryDragSource8, value, "InventoryDragSource8"); } }
        public int InventoryDragSourceId8 { get { return _inventoryDragSourceId8; } set {if(_inventoryDragSourceId8 != value) Set(ref _inventoryDragSourceId8, value, "InventoryDragSourceId8"); } }
        public int InventoryDragSourceSlot8 { get { return _inventoryDragSourceSlot8; } set {if(_inventoryDragSourceSlot8 != value) Set(ref _inventoryDragSourceSlot8, value, "InventoryDragSourceSlot8"); } }
        public Sprite InventoryItemIcon8 { get { return _inventoryItemIcon8; } set {if(_inventoryItemIcon8 != value) Set(ref _inventoryItemIcon8, value, "InventoryItemIcon8"); if(null != _view && null != _view.InventoryItemIcon8 && null == value) _view.InventoryItemIcon8.sprite = ViewModelUtil.EmptySprite; } }
        public string InventoryItemName8 { get { return _inventoryItemName8; } set {if(_inventoryItemName8 != value) Set(ref _inventoryItemName8, value, "InventoryItemName8"); } }
        public string InventoryItemNum8 { get { return _inventoryItemNum8; } set {if(_inventoryItemNum8 != value) Set(ref _inventoryItemNum8, value, "InventoryItemNum8"); } }
        public bool ShowInventoryItem9 { get { return _showInventoryItem9; } set {if(_showInventoryItem9 != value) Set(ref _showInventoryItem9, value, "ShowInventoryItem9"); } }
        public IDragTarget InventoryDragTarget9 { get { return _inventoryDragTarget9; } set {if(_inventoryDragTarget9 != value) Set(ref _inventoryDragTarget9, value, "InventoryDragTarget9"); } }
        public int InventoryDropSlot9 { get { return _inventoryDropSlot9; } set {if(_inventoryDropSlot9 != value) Set(ref _inventoryDropSlot9, value, "InventoryDropSlot9"); } }
        public IDragSource InventoryDragSource9 { get { return _inventoryDragSource9; } set {if(_inventoryDragSource9 != value) Set(ref _inventoryDragSource9, value, "InventoryDragSource9"); } }
        public int InventoryDragSourceId9 { get { return _inventoryDragSourceId9; } set {if(_inventoryDragSourceId9 != value) Set(ref _inventoryDragSourceId9, value, "InventoryDragSourceId9"); } }
        public int InventoryDragSourceSlot9 { get { return _inventoryDragSourceSlot9; } set {if(_inventoryDragSourceSlot9 != value) Set(ref _inventoryDragSourceSlot9, value, "InventoryDragSourceSlot9"); } }
        public Sprite InventoryItemIcon9 { get { return _inventoryItemIcon9; } set {if(_inventoryItemIcon9 != value) Set(ref _inventoryItemIcon9, value, "InventoryItemIcon9"); if(null != _view && null != _view.InventoryItemIcon9 && null == value) _view.InventoryItemIcon9.sprite = ViewModelUtil.EmptySprite; } }
        public string InventoryItemName9 { get { return _inventoryItemName9; } set {if(_inventoryItemName9 != value) Set(ref _inventoryItemName9, value, "InventoryItemName9"); } }
        public string InventoryItemNum9 { get { return _inventoryItemNum9; } set {if(_inventoryItemNum9 != value) Set(ref _inventoryItemNum9, value, "InventoryItemNum9"); } }
        public bool ShowInventoryItem10 { get { return _showInventoryItem10; } set {if(_showInventoryItem10 != value) Set(ref _showInventoryItem10, value, "ShowInventoryItem10"); } }
        public IDragTarget InventoryDragTarget10 { get { return _inventoryDragTarget10; } set {if(_inventoryDragTarget10 != value) Set(ref _inventoryDragTarget10, value, "InventoryDragTarget10"); } }
        public int InventoryDropSlot10 { get { return _inventoryDropSlot10; } set {if(_inventoryDropSlot10 != value) Set(ref _inventoryDropSlot10, value, "InventoryDropSlot10"); } }
        public IDragSource InventoryDragSource10 { get { return _inventoryDragSource10; } set {if(_inventoryDragSource10 != value) Set(ref _inventoryDragSource10, value, "InventoryDragSource10"); } }
        public int InventoryDragSourceId10 { get { return _inventoryDragSourceId10; } set {if(_inventoryDragSourceId10 != value) Set(ref _inventoryDragSourceId10, value, "InventoryDragSourceId10"); } }
        public int InventoryDragSourceSlot10 { get { return _inventoryDragSourceSlot10; } set {if(_inventoryDragSourceSlot10 != value) Set(ref _inventoryDragSourceSlot10, value, "InventoryDragSourceSlot10"); } }
        public Sprite InventoryItemIcon10 { get { return _inventoryItemIcon10; } set {if(_inventoryItemIcon10 != value) Set(ref _inventoryItemIcon10, value, "InventoryItemIcon10"); if(null != _view && null != _view.InventoryItemIcon10 && null == value) _view.InventoryItemIcon10.sprite = ViewModelUtil.EmptySprite; } }
        public string InventoryItemName10 { get { return _inventoryItemName10; } set {if(_inventoryItemName10 != value) Set(ref _inventoryItemName10, value, "InventoryItemName10"); } }
        public string InventoryItemNum10 { get { return _inventoryItemNum10; } set {if(_inventoryItemNum10 != value) Set(ref _inventoryItemNum10, value, "InventoryItemNum10"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonBagView _view;
		
		public void Destory()
        {
            if (_viewGameObject != null)
            {
				UnityEngine.Object.Destroy(_viewGameObject);
            }
        }
		public void Visible(bool isViaible)
		{
		    if (_viewGameObject != null)
            {
				_viewGameObject.SetActive(isViaible);
            }
		
		}
		public void SetCanvasEnabled(bool value)
        {
            if (_viewCanvas != null)
            {
                _viewCanvas.enabled = value;
            }
        }
        public void CreateBinding(GameObject obj)
        {
			_viewGameObject = obj;
			_viewCanvas = _viewGameObject.GetComponent<Canvas>();

			var view = obj.GetComponent<CommonBagView>();
			if(view != null)
			{
				_view = view;
				Reset();        //回滚初始值
				view.BindingContext().DataContext = this; 
				return;
			}

            view = obj.AddComponent<CommonBagView>();
			_view = view;
            view.FillField();
            view.BindingContext().DataContext = this;

            BindingSet<CommonBagView, CommonBagViewModel> bindingSet =
                view.CreateBindingSet<CommonBagView, CommonBagViewModel>();

            view.oriShowGameObjectActiveSelf = _showGameObjectActiveSelf = view.ShowGameObjectActiveSelf.activeSelf;
            bindingSet.Bind(view.ShowGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.ShowGameObjectActiveSelf).OneWay();
            view.oriShowPlayer = _showPlayer = view.ShowPlayer.activeSelf;
            bindingSet.Bind(view.ShowPlayer).For(v => v.activeSelf).To(vm => vm.ShowPlayer).OneWay();
            view.oriPlayerTexture = _playerTexture = view.PlayerTexture.texture;
            bindingSet.Bind(view.PlayerTexture).For(v => v.texture).To(vm => vm.PlayerTexture).OneWay();
            view.oriInventoryBgDragTarget = _inventoryBgDragTarget = view.InventoryBgDragTarget.DragTarget;
            bindingSet.Bind(view.InventoryBgDragTarget).For(v => v.DragTarget).To(vm => vm.InventoryBgDragTarget).OneWay();
            view.oriInventoryBgDropSlot = _inventoryBgDropSlot = view.InventoryBgDropSlot.Slot;
            bindingSet.Bind(view.InventoryBgDropSlot).For(v => v.Slot).To(vm => vm.InventoryBgDropSlot).OneWay();
            view.oriShowPrimeWeapon = _showPrimeWeapon = view.ShowPrimeWeapon.activeSelf;
            bindingSet.Bind(view.ShowPrimeWeapon).For(v => v.activeSelf).To(vm => vm.ShowPrimeWeapon).OneWay();
            view.oriPrimeWeaponDragTarget = _primeWeaponDragTarget = view.PrimeWeaponDragTarget.DragTarget;
            bindingSet.Bind(view.PrimeWeaponDragTarget).For(v => v.DragTarget).To(vm => vm.PrimeWeaponDragTarget).OneWay();
            view.oriPrimeWeaponSlot = _primeWeaponSlot = view.PrimeWeaponSlot.Slot;
            bindingSet.Bind(view.PrimeWeaponSlot).For(v => v.Slot).To(vm => vm.PrimeWeaponSlot).OneWay();
            view.oriPrimeWeaponDragSource = _primeWeaponDragSource = view.PrimeWeaponDragSource.DragSource;
            bindingSet.Bind(view.PrimeWeaponDragSource).For(v => v.DragSource).To(vm => vm.PrimeWeaponDragSource).OneWay();
            view.oriPrimeWeaponDragSourceId = _primeWeaponDragSourceId = view.PrimeWeaponDragSourceId.Id;
            bindingSet.Bind(view.PrimeWeaponDragSourceId).For(v => v.Id).To(vm => vm.PrimeWeaponDragSourceId).OneWay();
            view.oriPrimeWeaponDragSourceSlot = _primeWeaponDragSourceSlot = view.PrimeWeaponDragSourceSlot.Slot;
            bindingSet.Bind(view.PrimeWeaponDragSourceSlot).For(v => v.Slot).To(vm => vm.PrimeWeaponDragSourceSlot).OneWay();
            view.oriPrimeWeaponName = _primeWeaponName = view.PrimeWeaponName.text;
            bindingSet.Bind(view.PrimeWeaponName).For(v => v.text).To(vm => vm.PrimeWeaponName).OneWay();
            bindingSet.Bind(view.PrimeWeaponIcon).For(v => v.sprite).To(vm => vm.PrimeWeaponIcon).OneWay();
            bindingSet.Bind(view.PrimeMuzzle).For(v => v.sprite).To(vm => vm.PrimeMuzzle).OneWay();
            bindingSet.Bind(view.PrimeUpperRail).For(v => v.sprite).To(vm => vm.PrimeUpperRail).OneWay();
            bindingSet.Bind(view.PrimeLowerRail).For(v => v.sprite).To(vm => vm.PrimeLowerRail).OneWay();
            bindingSet.Bind(view.PrimeMagzine).For(v => v.sprite).To(vm => vm.PrimeMagzine).OneWay();
            bindingSet.Bind(view.PrimeStock).For(v => v.sprite).To(vm => vm.PrimeStock).OneWay();
            view.oriShowSubWeapon = _showSubWeapon = view.ShowSubWeapon.activeSelf;
            bindingSet.Bind(view.ShowSubWeapon).For(v => v.activeSelf).To(vm => vm.ShowSubWeapon).OneWay();
            view.oriSubWeaponDragTarget = _subWeaponDragTarget = view.SubWeaponDragTarget.DragTarget;
            bindingSet.Bind(view.SubWeaponDragTarget).For(v => v.DragTarget).To(vm => vm.SubWeaponDragTarget).OneWay();
            view.oriSubWeaponSlot = _subWeaponSlot = view.SubWeaponSlot.Slot;
            bindingSet.Bind(view.SubWeaponSlot).For(v => v.Slot).To(vm => vm.SubWeaponSlot).OneWay();
            view.oriSubWeaponName = _subWeaponName = view.SubWeaponName.text;
            bindingSet.Bind(view.SubWeaponName).For(v => v.text).To(vm => vm.SubWeaponName).OneWay();
            bindingSet.Bind(view.SubWeaponIcon).For(v => v.sprite).To(vm => vm.SubWeaponIcon).OneWay();
            view.oriPistolDragTarget = _pistolDragTarget = view.PistolDragTarget.DragTarget;
            bindingSet.Bind(view.PistolDragTarget).For(v => v.DragTarget).To(vm => vm.PistolDragTarget).OneWay();
            view.oriPistolSlot = _pistolSlot = view.PistolSlot.Slot;
            bindingSet.Bind(view.PistolSlot).For(v => v.Slot).To(vm => vm.PistolSlot).OneWay();
            view.oriShowPistol = _showPistol = view.ShowPistol.activeSelf;
            bindingSet.Bind(view.ShowPistol).For(v => v.activeSelf).To(vm => vm.ShowPistol).OneWay();
            bindingSet.Bind(view.PistolIcon).For(v => v.sprite).To(vm => vm.PistolIcon).OneWay();
            view.oriPistolName = _pistolName = view.PistolName.text;
            bindingSet.Bind(view.PistolName).For(v => v.text).To(vm => vm.PistolName).OneWay();
            view.oriShowMelee = _showMelee = view.ShowMelee.activeSelf;
            bindingSet.Bind(view.ShowMelee).For(v => v.activeSelf).To(vm => vm.ShowMelee).OneWay();
            view.oriMeleeDragTarget = _meleeDragTarget = view.MeleeDragTarget.DragTarget;
            bindingSet.Bind(view.MeleeDragTarget).For(v => v.DragTarget).To(vm => vm.MeleeDragTarget).OneWay();
            view.oriMeleeSlot = _meleeSlot = view.MeleeSlot.Slot;
            bindingSet.Bind(view.MeleeSlot).For(v => v.Slot).To(vm => vm.MeleeSlot).OneWay();
            view.oriMeleeName = _meleeName = view.MeleeName.text;
            bindingSet.Bind(view.MeleeName).For(v => v.text).To(vm => vm.MeleeName).OneWay();
            bindingSet.Bind(view.MeleeIcon).For(v => v.sprite).To(vm => vm.MeleeIcon).OneWay();
            view.oriShowGrenade = _showGrenade = view.ShowGrenade.activeSelf;
            bindingSet.Bind(view.ShowGrenade).For(v => v.activeSelf).To(vm => vm.ShowGrenade).OneWay();
            view.oriGrenadeDragTarget = _grenadeDragTarget = view.GrenadeDragTarget.DragTarget;
            bindingSet.Bind(view.GrenadeDragTarget).For(v => v.DragTarget).To(vm => vm.GrenadeDragTarget).OneWay();
            view.oriGrenadeSlot = _grenadeSlot = view.GrenadeSlot.Slot;
            bindingSet.Bind(view.GrenadeSlot).For(v => v.Slot).To(vm => vm.GrenadeSlot).OneWay();
            view.oriGrenadeName = _grenadeName = view.GrenadeName.text;
            bindingSet.Bind(view.GrenadeName).For(v => v.text).To(vm => vm.GrenadeName).OneWay();
            bindingSet.Bind(view.GrenadeIcon).For(v => v.sprite).To(vm => vm.GrenadeIcon).OneWay();
            view.oriNearByBgDragTarget = _nearByBgDragTarget = view.NearByBgDragTarget.DragTarget;
            bindingSet.Bind(view.NearByBgDragTarget).For(v => v.DragTarget).To(vm => vm.NearByBgDragTarget).OneWay();
            view.oriNearByBgDropSlot = _nearByBgDropSlot = view.NearByBgDropSlot.Slot;
            bindingSet.Bind(view.NearByBgDropSlot).For(v => v.Slot).To(vm => vm.NearByBgDropSlot).OneWay();
            bindingSet.Bind(view.ShowNearbyItem1).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem1).OneWay();
            bindingSet.Bind(view.NearByDragSource1).For(v => v.DragSource).To(vm => vm.NearByDragSource1).OneWay();
            bindingSet.Bind(view.NearByDragSourceId1).For(v => v.Id).To(vm => vm.NearByDragSourceId1).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot1).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot1).OneWay();
            bindingSet.Bind(view.NearbyItemIcon1).For(v => v.sprite).To(vm => vm.NearbyItemIcon1).OneWay();
            bindingSet.Bind(view.NearbyItemName1).For(v => v.text).To(vm => vm.NearbyItemName1).OneWay();
            bindingSet.Bind(view.NearbyItemNum1).For(v => v.text).To(vm => vm.NearbyItemNum1).OneWay();
            bindingSet.Bind(view.ShowNearbyItem2).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem2).OneWay();
            bindingSet.Bind(view.NearByDragSource2).For(v => v.DragSource).To(vm => vm.NearByDragSource2).OneWay();
            bindingSet.Bind(view.NearByDragSourceId2).For(v => v.Id).To(vm => vm.NearByDragSourceId2).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot2).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot2).OneWay();
            bindingSet.Bind(view.NearbyItemIcon2).For(v => v.sprite).To(vm => vm.NearbyItemIcon2).OneWay();
            bindingSet.Bind(view.NearbyItemName2).For(v => v.text).To(vm => vm.NearbyItemName2).OneWay();
            bindingSet.Bind(view.NearbyItemNum2).For(v => v.text).To(vm => vm.NearbyItemNum2).OneWay();
            bindingSet.Bind(view.ShowNearbyItem3).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem3).OneWay();
            bindingSet.Bind(view.NearByDragSource3).For(v => v.DragSource).To(vm => vm.NearByDragSource3).OneWay();
            bindingSet.Bind(view.NearByDragSourceId3).For(v => v.Id).To(vm => vm.NearByDragSourceId3).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot3).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot3).OneWay();
            bindingSet.Bind(view.NearbyItemIcon3).For(v => v.sprite).To(vm => vm.NearbyItemIcon3).OneWay();
            bindingSet.Bind(view.NearbyItemName3).For(v => v.text).To(vm => vm.NearbyItemName3).OneWay();
            bindingSet.Bind(view.NearbyItemNum3).For(v => v.text).To(vm => vm.NearbyItemNum3).OneWay();
            bindingSet.Bind(view.ShowNearbyItem4).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem4).OneWay();
            bindingSet.Bind(view.NearByDragSource4).For(v => v.DragSource).To(vm => vm.NearByDragSource4).OneWay();
            bindingSet.Bind(view.NearByDragSourceId4).For(v => v.Id).To(vm => vm.NearByDragSourceId4).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot4).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot4).OneWay();
            bindingSet.Bind(view.NearbyItemIcon4).For(v => v.sprite).To(vm => vm.NearbyItemIcon4).OneWay();
            bindingSet.Bind(view.NearbyItemName4).For(v => v.text).To(vm => vm.NearbyItemName4).OneWay();
            bindingSet.Bind(view.NearbyItemNum4).For(v => v.text).To(vm => vm.NearbyItemNum4).OneWay();
            bindingSet.Bind(view.ShowNearbyItem5).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem5).OneWay();
            bindingSet.Bind(view.NearByDragSource5).For(v => v.DragSource).To(vm => vm.NearByDragSource5).OneWay();
            bindingSet.Bind(view.NearByDragSourceId5).For(v => v.Id).To(vm => vm.NearByDragSourceId5).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot5).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot5).OneWay();
            bindingSet.Bind(view.NearbyItemIcon5).For(v => v.sprite).To(vm => vm.NearbyItemIcon5).OneWay();
            bindingSet.Bind(view.NearbyItemName5).For(v => v.text).To(vm => vm.NearbyItemName5).OneWay();
            bindingSet.Bind(view.NearbyItemNum5).For(v => v.text).To(vm => vm.NearbyItemNum5).OneWay();
            bindingSet.Bind(view.ShowNearbyItem6).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem6).OneWay();
            bindingSet.Bind(view.NearByDragSource6).For(v => v.DragSource).To(vm => vm.NearByDragSource6).OneWay();
            bindingSet.Bind(view.NearByDragSourceId6).For(v => v.Id).To(vm => vm.NearByDragSourceId6).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot6).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot6).OneWay();
            bindingSet.Bind(view.NearbyItemIcon6).For(v => v.sprite).To(vm => vm.NearbyItemIcon6).OneWay();
            bindingSet.Bind(view.NearbyItemName6).For(v => v.text).To(vm => vm.NearbyItemName6).OneWay();
            bindingSet.Bind(view.NearbyItemNum6).For(v => v.text).To(vm => vm.NearbyItemNum6).OneWay();
            bindingSet.Bind(view.ShowNearbyItem7).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem7).OneWay();
            bindingSet.Bind(view.NearByDragSource7).For(v => v.DragSource).To(vm => vm.NearByDragSource7).OneWay();
            bindingSet.Bind(view.NearByDragSourceId7).For(v => v.Id).To(vm => vm.NearByDragSourceId7).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot7).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot7).OneWay();
            bindingSet.Bind(view.NearbyItemIcon7).For(v => v.sprite).To(vm => vm.NearbyItemIcon7).OneWay();
            bindingSet.Bind(view.NearbyItemName7).For(v => v.text).To(vm => vm.NearbyItemName7).OneWay();
            bindingSet.Bind(view.NearbyItemNum7).For(v => v.text).To(vm => vm.NearbyItemNum7).OneWay();
            bindingSet.Bind(view.ShowNearbyItem8).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem8).OneWay();
            bindingSet.Bind(view.NearByDragSource8).For(v => v.DragSource).To(vm => vm.NearByDragSource8).OneWay();
            bindingSet.Bind(view.NearByDragSourceId8).For(v => v.Id).To(vm => vm.NearByDragSourceId8).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot8).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot8).OneWay();
            bindingSet.Bind(view.NearbyItemIcon8).For(v => v.sprite).To(vm => vm.NearbyItemIcon8).OneWay();
            bindingSet.Bind(view.NearbyItemName8).For(v => v.text).To(vm => vm.NearbyItemName8).OneWay();
            bindingSet.Bind(view.NearbyItemNum8).For(v => v.text).To(vm => vm.NearbyItemNum8).OneWay();
            bindingSet.Bind(view.ShowNearbyItem9).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem9).OneWay();
            bindingSet.Bind(view.NearByDragSource9).For(v => v.DragSource).To(vm => vm.NearByDragSource9).OneWay();
            bindingSet.Bind(view.NearByDragSourceId9).For(v => v.Id).To(vm => vm.NearByDragSourceId9).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot9).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot9).OneWay();
            bindingSet.Bind(view.NearbyItemIcon9).For(v => v.sprite).To(vm => vm.NearbyItemIcon9).OneWay();
            bindingSet.Bind(view.NearbyItemName9).For(v => v.text).To(vm => vm.NearbyItemName9).OneWay();
            bindingSet.Bind(view.NearbyItemNum9).For(v => v.text).To(vm => vm.NearbyItemNum9).OneWay();
            bindingSet.Bind(view.ShowNearbyItem10).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem10).OneWay();
            bindingSet.Bind(view.NearByDragSource10).For(v => v.DragSource).To(vm => vm.NearByDragSource10).OneWay();
            bindingSet.Bind(view.NearByDragSourceId10).For(v => v.Id).To(vm => vm.NearByDragSourceId10).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot10).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot10).OneWay();
            bindingSet.Bind(view.NearbyItemIcon10).For(v => v.sprite).To(vm => vm.NearbyItemIcon10).OneWay();
            bindingSet.Bind(view.NearbyItemName10).For(v => v.text).To(vm => vm.NearbyItemName10).OneWay();
            bindingSet.Bind(view.NearbyItemNum10).For(v => v.text).To(vm => vm.NearbyItemNum10).OneWay();
            bindingSet.Bind(view.ShowNearbyItem11).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem11).OneWay();
            bindingSet.Bind(view.NearByDragSource11).For(v => v.DragSource).To(vm => vm.NearByDragSource11).OneWay();
            bindingSet.Bind(view.NearByDragSourceId11).For(v => v.Id).To(vm => vm.NearByDragSourceId11).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot11).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot11).OneWay();
            bindingSet.Bind(view.NearbyItemIcon11).For(v => v.sprite).To(vm => vm.NearbyItemIcon11).OneWay();
            bindingSet.Bind(view.NearbyItemName11).For(v => v.text).To(vm => vm.NearbyItemName11).OneWay();
            bindingSet.Bind(view.NearbyItemNum11).For(v => v.text).To(vm => vm.NearbyItemNum11).OneWay();
            bindingSet.Bind(view.ShowNearbyItem12).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem12).OneWay();
            bindingSet.Bind(view.NearByDragSource12).For(v => v.DragSource).To(vm => vm.NearByDragSource12).OneWay();
            bindingSet.Bind(view.NearByDragSourceId12).For(v => v.Id).To(vm => vm.NearByDragSourceId12).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot12).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot12).OneWay();
            bindingSet.Bind(view.NearbyItemIcon12).For(v => v.sprite).To(vm => vm.NearbyItemIcon12).OneWay();
            bindingSet.Bind(view.NearbyItemName12).For(v => v.text).To(vm => vm.NearbyItemName12).OneWay();
            bindingSet.Bind(view.NearbyItemNum12).For(v => v.text).To(vm => vm.NearbyItemNum12).OneWay();
            bindingSet.Bind(view.ShowNearbyItem13).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem13).OneWay();
            bindingSet.Bind(view.NearByDragSource13).For(v => v.DragSource).To(vm => vm.NearByDragSource13).OneWay();
            bindingSet.Bind(view.NearByDragSourceId13).For(v => v.Id).To(vm => vm.NearByDragSourceId13).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot13).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot13).OneWay();
            bindingSet.Bind(view.NearbyItemIcon13).For(v => v.sprite).To(vm => vm.NearbyItemIcon13).OneWay();
            bindingSet.Bind(view.NearbyItemName13).For(v => v.text).To(vm => vm.NearbyItemName13).OneWay();
            bindingSet.Bind(view.NearbyItemNum13).For(v => v.text).To(vm => vm.NearbyItemNum13).OneWay();
            bindingSet.Bind(view.ShowNearbyItem14).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem14).OneWay();
            bindingSet.Bind(view.NearByDragSource14).For(v => v.DragSource).To(vm => vm.NearByDragSource14).OneWay();
            bindingSet.Bind(view.NearByDragSourceId14).For(v => v.Id).To(vm => vm.NearByDragSourceId14).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot14).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot14).OneWay();
            bindingSet.Bind(view.NearbyItemIcon14).For(v => v.sprite).To(vm => vm.NearbyItemIcon14).OneWay();
            bindingSet.Bind(view.NearbyItemName14).For(v => v.text).To(vm => vm.NearbyItemName14).OneWay();
            bindingSet.Bind(view.NearbyItemNum14).For(v => v.text).To(vm => vm.NearbyItemNum14).OneWay();
            bindingSet.Bind(view.ShowNearbyItem15).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem15).OneWay();
            bindingSet.Bind(view.NearByDragSource15).For(v => v.DragSource).To(vm => vm.NearByDragSource15).OneWay();
            bindingSet.Bind(view.NearByDragSourceId15).For(v => v.Id).To(vm => vm.NearByDragSourceId15).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot15).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot15).OneWay();
            bindingSet.Bind(view.NearbyItemIcon15).For(v => v.sprite).To(vm => vm.NearbyItemIcon15).OneWay();
            bindingSet.Bind(view.NearbyItemName15).For(v => v.text).To(vm => vm.NearbyItemName15).OneWay();
            bindingSet.Bind(view.NearbyItemNum15).For(v => v.text).To(vm => vm.NearbyItemNum15).OneWay();
            bindingSet.Bind(view.ShowNearbyItem16).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem16).OneWay();
            bindingSet.Bind(view.NearByDragSource16).For(v => v.DragSource).To(vm => vm.NearByDragSource16).OneWay();
            bindingSet.Bind(view.NearByDragSourceId16).For(v => v.Id).To(vm => vm.NearByDragSourceId16).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot16).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot16).OneWay();
            bindingSet.Bind(view.NearbyItemIcon16).For(v => v.sprite).To(vm => vm.NearbyItemIcon16).OneWay();
            bindingSet.Bind(view.NearbyItemName16).For(v => v.text).To(vm => vm.NearbyItemName16).OneWay();
            bindingSet.Bind(view.NearbyItemNum16).For(v => v.text).To(vm => vm.NearbyItemNum16).OneWay();
            bindingSet.Bind(view.ShowNearbyItem17).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem17).OneWay();
            bindingSet.Bind(view.NearByDragSource17).For(v => v.DragSource).To(vm => vm.NearByDragSource17).OneWay();
            bindingSet.Bind(view.NearByDragSourceId17).For(v => v.Id).To(vm => vm.NearByDragSourceId17).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot17).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot17).OneWay();
            bindingSet.Bind(view.NearbyItemIcon17).For(v => v.sprite).To(vm => vm.NearbyItemIcon17).OneWay();
            bindingSet.Bind(view.NearbyItemName17).For(v => v.text).To(vm => vm.NearbyItemName17).OneWay();
            bindingSet.Bind(view.NearbyItemNum17).For(v => v.text).To(vm => vm.NearbyItemNum17).OneWay();
            bindingSet.Bind(view.ShowNearbyItem18).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem18).OneWay();
            bindingSet.Bind(view.NearByDragSource18).For(v => v.DragSource).To(vm => vm.NearByDragSource18).OneWay();
            bindingSet.Bind(view.NearByDragSourceId18).For(v => v.Id).To(vm => vm.NearByDragSourceId18).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot18).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot18).OneWay();
            bindingSet.Bind(view.NearbyItemIcon18).For(v => v.sprite).To(vm => vm.NearbyItemIcon18).OneWay();
            bindingSet.Bind(view.NearbyItemName18).For(v => v.text).To(vm => vm.NearbyItemName18).OneWay();
            bindingSet.Bind(view.NearbyItemNum18).For(v => v.text).To(vm => vm.NearbyItemNum18).OneWay();
            bindingSet.Bind(view.ShowNearbyItem19).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem19).OneWay();
            bindingSet.Bind(view.NearByDragSource19).For(v => v.DragSource).To(vm => vm.NearByDragSource19).OneWay();
            bindingSet.Bind(view.NearByDragSourceId19).For(v => v.Id).To(vm => vm.NearByDragSourceId19).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot19).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot19).OneWay();
            bindingSet.Bind(view.NearbyItemIcon19).For(v => v.sprite).To(vm => vm.NearbyItemIcon19).OneWay();
            bindingSet.Bind(view.NearbyItemName19).For(v => v.text).To(vm => vm.NearbyItemName19).OneWay();
            bindingSet.Bind(view.NearbyItemNum19).For(v => v.text).To(vm => vm.NearbyItemNum19).OneWay();
            bindingSet.Bind(view.ShowNearbyItem20).For(v => v.activeSelf).To(vm => vm.ShowNearbyItem20).OneWay();
            bindingSet.Bind(view.NearByDragSource20).For(v => v.DragSource).To(vm => vm.NearByDragSource20).OneWay();
            bindingSet.Bind(view.NearByDragSourceId20).For(v => v.Id).To(vm => vm.NearByDragSourceId20).OneWay();
            bindingSet.Bind(view.NearByDragSourceSlot20).For(v => v.Slot).To(vm => vm.NearByDragSourceSlot20).OneWay();
            bindingSet.Bind(view.NearbyItemIcon20).For(v => v.sprite).To(vm => vm.NearbyItemIcon20).OneWay();
            bindingSet.Bind(view.NearbyItemName20).For(v => v.text).To(vm => vm.NearbyItemName20).OneWay();
            bindingSet.Bind(view.NearbyItemNum20).For(v => v.text).To(vm => vm.NearbyItemNum20).OneWay();
            bindingSet.Bind(view.ShowInventoryItem1).For(v => v.activeSelf).To(vm => vm.ShowInventoryItem1).OneWay();
            bindingSet.Bind(view.InventoryDragTarget1).For(v => v.DragTarget).To(vm => vm.InventoryDragTarget1).OneWay();
            bindingSet.Bind(view.InventoryDropSlot1).For(v => v.Slot).To(vm => vm.InventoryDropSlot1).OneWay();
            bindingSet.Bind(view.InventoryDragSource1).For(v => v.DragSource).To(vm => vm.InventoryDragSource1).OneWay();
            bindingSet.Bind(view.InventoryDragSourceId1).For(v => v.Id).To(vm => vm.InventoryDragSourceId1).OneWay();
            bindingSet.Bind(view.InventoryDragSourceSlot1).For(v => v.Slot).To(vm => vm.InventoryDragSourceSlot1).OneWay();
            bindingSet.Bind(view.InventoryItemIcon1).For(v => v.sprite).To(vm => vm.InventoryItemIcon1).OneWay();
            bindingSet.Bind(view.InventoryItemName1).For(v => v.text).To(vm => vm.InventoryItemName1).OneWay();
            bindingSet.Bind(view.InventoryItemNum1).For(v => v.text).To(vm => vm.InventoryItemNum1).OneWay();
            bindingSet.Bind(view.ShowInventoryItem2).For(v => v.activeSelf).To(vm => vm.ShowInventoryItem2).OneWay();
            bindingSet.Bind(view.InventoryDragTarget2).For(v => v.DragTarget).To(vm => vm.InventoryDragTarget2).OneWay();
            bindingSet.Bind(view.InventoryDropSlot2).For(v => v.Slot).To(vm => vm.InventoryDropSlot2).OneWay();
            bindingSet.Bind(view.InventoryDragSource2).For(v => v.DragSource).To(vm => vm.InventoryDragSource2).OneWay();
            bindingSet.Bind(view.InventoryDragSourceId2).For(v => v.Id).To(vm => vm.InventoryDragSourceId2).OneWay();
            bindingSet.Bind(view.InventoryDragSourceSlot2).For(v => v.Slot).To(vm => vm.InventoryDragSourceSlot2).OneWay();
            bindingSet.Bind(view.InventoryItemIcon2).For(v => v.sprite).To(vm => vm.InventoryItemIcon2).OneWay();
            bindingSet.Bind(view.InventoryItemName2).For(v => v.text).To(vm => vm.InventoryItemName2).OneWay();
            bindingSet.Bind(view.InventoryItemNum2).For(v => v.text).To(vm => vm.InventoryItemNum2).OneWay();
            bindingSet.Bind(view.ShowInventoryItem3).For(v => v.activeSelf).To(vm => vm.ShowInventoryItem3).OneWay();
            bindingSet.Bind(view.InventoryDragTarget3).For(v => v.DragTarget).To(vm => vm.InventoryDragTarget3).OneWay();
            bindingSet.Bind(view.InventoryDropSlot3).For(v => v.Slot).To(vm => vm.InventoryDropSlot3).OneWay();
            bindingSet.Bind(view.InventoryDragSource3).For(v => v.DragSource).To(vm => vm.InventoryDragSource3).OneWay();
            bindingSet.Bind(view.InventoryDragSourceId3).For(v => v.Id).To(vm => vm.InventoryDragSourceId3).OneWay();
            bindingSet.Bind(view.InventoryDragSourceSlot3).For(v => v.Slot).To(vm => vm.InventoryDragSourceSlot3).OneWay();
            bindingSet.Bind(view.InventoryItemIcon3).For(v => v.sprite).To(vm => vm.InventoryItemIcon3).OneWay();
            bindingSet.Bind(view.InventoryItemName3).For(v => v.text).To(vm => vm.InventoryItemName3).OneWay();
            bindingSet.Bind(view.InventoryItemNum3).For(v => v.text).To(vm => vm.InventoryItemNum3).OneWay();
            bindingSet.Bind(view.ShowInventoryItem4).For(v => v.activeSelf).To(vm => vm.ShowInventoryItem4).OneWay();
            bindingSet.Bind(view.InventoryDragTarget4).For(v => v.DragTarget).To(vm => vm.InventoryDragTarget4).OneWay();
            bindingSet.Bind(view.InventoryDropSlot4).For(v => v.Slot).To(vm => vm.InventoryDropSlot4).OneWay();
            bindingSet.Bind(view.InventoryDragSource4).For(v => v.DragSource).To(vm => vm.InventoryDragSource4).OneWay();
            bindingSet.Bind(view.InventoryDragSourceId4).For(v => v.Id).To(vm => vm.InventoryDragSourceId4).OneWay();
            bindingSet.Bind(view.InventoryDragSourceSlot4).For(v => v.Slot).To(vm => vm.InventoryDragSourceSlot4).OneWay();
            bindingSet.Bind(view.InventoryItemIcon4).For(v => v.sprite).To(vm => vm.InventoryItemIcon4).OneWay();
            bindingSet.Bind(view.InventoryItemName4).For(v => v.text).To(vm => vm.InventoryItemName4).OneWay();
            bindingSet.Bind(view.InventoryItemNum4).For(v => v.text).To(vm => vm.InventoryItemNum4).OneWay();
            bindingSet.Bind(view.ShowInventoryItem5).For(v => v.activeSelf).To(vm => vm.ShowInventoryItem5).OneWay();
            bindingSet.Bind(view.InventoryDragTarget5).For(v => v.DragTarget).To(vm => vm.InventoryDragTarget5).OneWay();
            bindingSet.Bind(view.InventoryDropSlot5).For(v => v.Slot).To(vm => vm.InventoryDropSlot5).OneWay();
            bindingSet.Bind(view.InventoryDragSource5).For(v => v.DragSource).To(vm => vm.InventoryDragSource5).OneWay();
            bindingSet.Bind(view.InventoryDragSourceId5).For(v => v.Id).To(vm => vm.InventoryDragSourceId5).OneWay();
            bindingSet.Bind(view.InventoryDragSourceSlot5).For(v => v.Slot).To(vm => vm.InventoryDragSourceSlot5).OneWay();
            bindingSet.Bind(view.InventoryItemIcon5).For(v => v.sprite).To(vm => vm.InventoryItemIcon5).OneWay();
            bindingSet.Bind(view.InventoryItemName5).For(v => v.text).To(vm => vm.InventoryItemName5).OneWay();
            bindingSet.Bind(view.InventoryItemNum5).For(v => v.text).To(vm => vm.InventoryItemNum5).OneWay();
            bindingSet.Bind(view.ShowInventoryItem6).For(v => v.activeSelf).To(vm => vm.ShowInventoryItem6).OneWay();
            bindingSet.Bind(view.InventoryDragTarget6).For(v => v.DragTarget).To(vm => vm.InventoryDragTarget6).OneWay();
            bindingSet.Bind(view.InventoryDropSlot6).For(v => v.Slot).To(vm => vm.InventoryDropSlot6).OneWay();
            bindingSet.Bind(view.InventoryDragSource6).For(v => v.DragSource).To(vm => vm.InventoryDragSource6).OneWay();
            bindingSet.Bind(view.InventoryDragSourceId6).For(v => v.Id).To(vm => vm.InventoryDragSourceId6).OneWay();
            bindingSet.Bind(view.InventoryDragSourceSlot6).For(v => v.Slot).To(vm => vm.InventoryDragSourceSlot6).OneWay();
            bindingSet.Bind(view.InventoryItemIcon6).For(v => v.sprite).To(vm => vm.InventoryItemIcon6).OneWay();
            bindingSet.Bind(view.InventoryItemName6).For(v => v.text).To(vm => vm.InventoryItemName6).OneWay();
            bindingSet.Bind(view.InventoryItemNum6).For(v => v.text).To(vm => vm.InventoryItemNum6).OneWay();
            bindingSet.Bind(view.ShowInventoryItem7).For(v => v.activeSelf).To(vm => vm.ShowInventoryItem7).OneWay();
            bindingSet.Bind(view.InventoryDragTarget7).For(v => v.DragTarget).To(vm => vm.InventoryDragTarget7).OneWay();
            bindingSet.Bind(view.InventoryDropSlot7).For(v => v.Slot).To(vm => vm.InventoryDropSlot7).OneWay();
            bindingSet.Bind(view.InventoryDragSource7).For(v => v.DragSource).To(vm => vm.InventoryDragSource7).OneWay();
            bindingSet.Bind(view.InventoryDragSourceId7).For(v => v.Id).To(vm => vm.InventoryDragSourceId7).OneWay();
            bindingSet.Bind(view.InventoryDragSourceSlot7).For(v => v.Slot).To(vm => vm.InventoryDragSourceSlot7).OneWay();
            bindingSet.Bind(view.InventoryItemIcon7).For(v => v.sprite).To(vm => vm.InventoryItemIcon7).OneWay();
            bindingSet.Bind(view.InventoryItemName7).For(v => v.text).To(vm => vm.InventoryItemName7).OneWay();
            bindingSet.Bind(view.InventoryItemNum7).For(v => v.text).To(vm => vm.InventoryItemNum7).OneWay();
            bindingSet.Bind(view.ShowInventoryItem8).For(v => v.activeSelf).To(vm => vm.ShowInventoryItem8).OneWay();
            bindingSet.Bind(view.InventoryDragTarget8).For(v => v.DragTarget).To(vm => vm.InventoryDragTarget8).OneWay();
            bindingSet.Bind(view.InventoryDropSlot8).For(v => v.Slot).To(vm => vm.InventoryDropSlot8).OneWay();
            bindingSet.Bind(view.InventoryDragSource8).For(v => v.DragSource).To(vm => vm.InventoryDragSource8).OneWay();
            bindingSet.Bind(view.InventoryDragSourceId8).For(v => v.Id).To(vm => vm.InventoryDragSourceId8).OneWay();
            bindingSet.Bind(view.InventoryDragSourceSlot8).For(v => v.Slot).To(vm => vm.InventoryDragSourceSlot8).OneWay();
            bindingSet.Bind(view.InventoryItemIcon8).For(v => v.sprite).To(vm => vm.InventoryItemIcon8).OneWay();
            bindingSet.Bind(view.InventoryItemName8).For(v => v.text).To(vm => vm.InventoryItemName8).OneWay();
            bindingSet.Bind(view.InventoryItemNum8).For(v => v.text).To(vm => vm.InventoryItemNum8).OneWay();
            bindingSet.Bind(view.ShowInventoryItem9).For(v => v.activeSelf).To(vm => vm.ShowInventoryItem9).OneWay();
            bindingSet.Bind(view.InventoryDragTarget9).For(v => v.DragTarget).To(vm => vm.InventoryDragTarget9).OneWay();
            bindingSet.Bind(view.InventoryDropSlot9).For(v => v.Slot).To(vm => vm.InventoryDropSlot9).OneWay();
            bindingSet.Bind(view.InventoryDragSource9).For(v => v.DragSource).To(vm => vm.InventoryDragSource9).OneWay();
            bindingSet.Bind(view.InventoryDragSourceId9).For(v => v.Id).To(vm => vm.InventoryDragSourceId9).OneWay();
            bindingSet.Bind(view.InventoryDragSourceSlot9).For(v => v.Slot).To(vm => vm.InventoryDragSourceSlot9).OneWay();
            bindingSet.Bind(view.InventoryItemIcon9).For(v => v.sprite).To(vm => vm.InventoryItemIcon9).OneWay();
            bindingSet.Bind(view.InventoryItemName9).For(v => v.text).To(vm => vm.InventoryItemName9).OneWay();
            bindingSet.Bind(view.InventoryItemNum9).For(v => v.text).To(vm => vm.InventoryItemNum9).OneWay();
            bindingSet.Bind(view.ShowInventoryItem10).For(v => v.activeSelf).To(vm => vm.ShowInventoryItem10).OneWay();
            bindingSet.Bind(view.InventoryDragTarget10).For(v => v.DragTarget).To(vm => vm.InventoryDragTarget10).OneWay();
            bindingSet.Bind(view.InventoryDropSlot10).For(v => v.Slot).To(vm => vm.InventoryDropSlot10).OneWay();
            bindingSet.Bind(view.InventoryDragSource10).For(v => v.DragSource).To(vm => vm.InventoryDragSource10).OneWay();
            bindingSet.Bind(view.InventoryDragSourceId10).For(v => v.Id).To(vm => vm.InventoryDragSourceId10).OneWay();
            bindingSet.Bind(view.InventoryDragSourceSlot10).For(v => v.Slot).To(vm => vm.InventoryDragSourceSlot10).OneWay();
            bindingSet.Bind(view.InventoryItemIcon10).For(v => v.sprite).To(vm => vm.InventoryItemIcon10).OneWay();
            bindingSet.Bind(view.InventoryItemName10).For(v => v.text).To(vm => vm.InventoryItemName10).OneWay();
            bindingSet.Bind(view.InventoryItemNum10).For(v => v.text).To(vm => vm.InventoryItemNum10).OneWay();
            bindingSet.Build();

			SpriteReset();
        }
		private void EventTriggerBind(CommonBagView view)
		{
		}


        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonBagViewModel()
        {
            Type type = typeof(CommonBagViewModel);
            foreach (var property in type.GetProperties())
            {
                if (property.CanWrite)
                {
                    PropertySetter.Add(property.Name, property);
                }
            }
			foreach (var methodInfo in type.GetMethods())
            {
                if (methodInfo.IsPublic)
                {
                    MethodSetter.Add(methodInfo.Name, methodInfo);
                }
            }
        }

		private void SpriteReset()
		{
			PrimeWeaponIcon = ViewModelUtil.EmptySprite;
			PrimeMuzzle = ViewModelUtil.EmptySprite;
			PrimeUpperRail = ViewModelUtil.EmptySprite;
			PrimeLowerRail = ViewModelUtil.EmptySprite;
			PrimeMagzine = ViewModelUtil.EmptySprite;
			PrimeStock = ViewModelUtil.EmptySprite;
			SubWeaponIcon = ViewModelUtil.EmptySprite;
			PistolIcon = ViewModelUtil.EmptySprite;
			MeleeIcon = ViewModelUtil.EmptySprite;
			GrenadeIcon = ViewModelUtil.EmptySprite;
			NearbyItemIcon1 = ViewModelUtil.EmptySprite;
			NearbyItemIcon2 = ViewModelUtil.EmptySprite;
			NearbyItemIcon3 = ViewModelUtil.EmptySprite;
			NearbyItemIcon4 = ViewModelUtil.EmptySprite;
			NearbyItemIcon5 = ViewModelUtil.EmptySprite;
			NearbyItemIcon6 = ViewModelUtil.EmptySprite;
			NearbyItemIcon7 = ViewModelUtil.EmptySprite;
			NearbyItemIcon8 = ViewModelUtil.EmptySprite;
			NearbyItemIcon9 = ViewModelUtil.EmptySprite;
			NearbyItemIcon10 = ViewModelUtil.EmptySprite;
			NearbyItemIcon11 = ViewModelUtil.EmptySprite;
			NearbyItemIcon12 = ViewModelUtil.EmptySprite;
			NearbyItemIcon13 = ViewModelUtil.EmptySprite;
			NearbyItemIcon14 = ViewModelUtil.EmptySprite;
			NearbyItemIcon15 = ViewModelUtil.EmptySprite;
			NearbyItemIcon16 = ViewModelUtil.EmptySprite;
			NearbyItemIcon17 = ViewModelUtil.EmptySprite;
			NearbyItemIcon18 = ViewModelUtil.EmptySprite;
			NearbyItemIcon19 = ViewModelUtil.EmptySprite;
			NearbyItemIcon20 = ViewModelUtil.EmptySprite;
			InventoryItemIcon1 = ViewModelUtil.EmptySprite;
			InventoryItemIcon2 = ViewModelUtil.EmptySprite;
			InventoryItemIcon3 = ViewModelUtil.EmptySprite;
			InventoryItemIcon4 = ViewModelUtil.EmptySprite;
			InventoryItemIcon5 = ViewModelUtil.EmptySprite;
			InventoryItemIcon6 = ViewModelUtil.EmptySprite;
			InventoryItemIcon7 = ViewModelUtil.EmptySprite;
			InventoryItemIcon8 = ViewModelUtil.EmptySprite;
			InventoryItemIcon9 = ViewModelUtil.EmptySprite;
			InventoryItemIcon10 = ViewModelUtil.EmptySprite;
		}

		public void Reset()
		{
			ShowGameObjectActiveSelf = _view.oriShowGameObjectActiveSelf;
			ShowPlayer = _view.oriShowPlayer;
			PlayerTexture = _view.oriPlayerTexture;
			InventoryBgDragTarget = _view.oriInventoryBgDragTarget;
			InventoryBgDropSlot = _view.oriInventoryBgDropSlot;
			ShowPrimeWeapon = _view.oriShowPrimeWeapon;
			PrimeWeaponDragTarget = _view.oriPrimeWeaponDragTarget;
			PrimeWeaponSlot = _view.oriPrimeWeaponSlot;
			PrimeWeaponDragSource = _view.oriPrimeWeaponDragSource;
			PrimeWeaponDragSourceId = _view.oriPrimeWeaponDragSourceId;
			PrimeWeaponDragSourceSlot = _view.oriPrimeWeaponDragSourceSlot;
			PrimeWeaponName = _view.oriPrimeWeaponName;
			ShowSubWeapon = _view.oriShowSubWeapon;
			SubWeaponDragTarget = _view.oriSubWeaponDragTarget;
			SubWeaponSlot = _view.oriSubWeaponSlot;
			SubWeaponName = _view.oriSubWeaponName;
			PistolDragTarget = _view.oriPistolDragTarget;
			PistolSlot = _view.oriPistolSlot;
			ShowPistol = _view.oriShowPistol;
			PistolName = _view.oriPistolName;
			ShowMelee = _view.oriShowMelee;
			MeleeDragTarget = _view.oriMeleeDragTarget;
			MeleeSlot = _view.oriMeleeSlot;
			MeleeName = _view.oriMeleeName;
			ShowGrenade = _view.oriShowGrenade;
			GrenadeDragTarget = _view.oriGrenadeDragTarget;
			GrenadeSlot = _view.oriGrenadeSlot;
			GrenadeName = _view.oriGrenadeName;
			NearByBgDragTarget = _view.oriNearByBgDragTarget;
			NearByBgDropSlot = _view.oriNearByBgDropSlot;
			SpriteReset();
		}

		public void CallFunction(string functionName)
        {
            if (MethodSetter.ContainsKey(functionName))
            {
                MethodSetter[functionName].Invoke(this, null);
            }
        }

		public bool IsPropertyExist(string propertyId)
        {
            return PropertySetter.ContainsKey(propertyId);
        }

		public Transform GetParentLinkNode()
		{
			return null;
		}

		public Transform GetChildLinkNode()
		{
			return null;
		}

        public const string ShowNearbyItem = "ShowNearbyItem";
        public const int ShowNearbyItemCount = 20;
        public const string NearByDragSource = "NearByDragSource";
        public const int NearByDragSourceCount = 20;
        public const string NearByDragSourceId = "NearByDragSourceId";
        public const int NearByDragSourceIdCount = 20;
        public const string NearByDragSourceSlot = "NearByDragSourceSlot";
        public const int NearByDragSourceSlotCount = 20;
        public const string NearbyItemIcon = "NearbyItemIcon";
        public const int NearbyItemIconCount = 20;
        public const string NearbyItemName = "NearbyItemName";
        public const int NearbyItemNameCount = 20;
        public const string NearbyItemNum = "NearbyItemNum";
        public const int NearbyItemNumCount = 20;
        public const string ShowInventoryItem = "ShowInventoryItem";
        public const int ShowInventoryItemCount = 10;
        public const string InventoryDragTarget = "InventoryDragTarget";
        public const int InventoryDragTargetCount = 10;
        public const string InventoryDropSlot = "InventoryDropSlot";
        public const int InventoryDropSlotCount = 10;
        public const string InventoryDragSource = "InventoryDragSource";
        public const int InventoryDragSourceCount = 10;
        public const string InventoryDragSourceId = "InventoryDragSourceId";
        public const int InventoryDragSourceIdCount = 10;
        public const string InventoryDragSourceSlot = "InventoryDragSourceSlot";
        public const int InventoryDragSourceSlotCount = 10;
        public const string InventoryItemIcon = "InventoryItemIcon";
        public const int InventoryItemIconCount = 10;
        public const string InventoryItemName = "InventoryItemName";
        public const int InventoryItemNameCount = 10;
        public const string InventoryItemNum = "InventoryItemNum";
        public const int InventoryItemNumCount = 10;
        public bool SetShowNearbyItem (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		ShowNearbyItem1 = val;
        		break;
        	case 2:
        		ShowNearbyItem2 = val;
        		break;
        	case 3:
        		ShowNearbyItem3 = val;
        		break;
        	case 4:
        		ShowNearbyItem4 = val;
        		break;
        	case 5:
        		ShowNearbyItem5 = val;
        		break;
        	case 6:
        		ShowNearbyItem6 = val;
        		break;
        	case 7:
        		ShowNearbyItem7 = val;
        		break;
        	case 8:
        		ShowNearbyItem8 = val;
        		break;
        	case 9:
        		ShowNearbyItem9 = val;
        		break;
        	case 10:
        		ShowNearbyItem10 = val;
        		break;
        	case 11:
        		ShowNearbyItem11 = val;
        		break;
        	case 12:
        		ShowNearbyItem12 = val;
        		break;
        	case 13:
        		ShowNearbyItem13 = val;
        		break;
        	case 14:
        		ShowNearbyItem14 = val;
        		break;
        	case 15:
        		ShowNearbyItem15 = val;
        		break;
        	case 16:
        		ShowNearbyItem16 = val;
        		break;
        	case 17:
        		ShowNearbyItem17 = val;
        		break;
        	case 18:
        		ShowNearbyItem18 = val;
        		break;
        	case 19:
        		ShowNearbyItem19 = val;
        		break;
        	case 20:
        		ShowNearbyItem20 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetShowNearbyItem (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return ShowNearbyItem1;
        	case 2:
        		return ShowNearbyItem2;
        	case 3:
        		return ShowNearbyItem3;
        	case 4:
        		return ShowNearbyItem4;
        	case 5:
        		return ShowNearbyItem5;
        	case 6:
        		return ShowNearbyItem6;
        	case 7:
        		return ShowNearbyItem7;
        	case 8:
        		return ShowNearbyItem8;
        	case 9:
        		return ShowNearbyItem9;
        	case 10:
        		return ShowNearbyItem10;
        	case 11:
        		return ShowNearbyItem11;
        	case 12:
        		return ShowNearbyItem12;
        	case 13:
        		return ShowNearbyItem13;
        	case 14:
        		return ShowNearbyItem14;
        	case 15:
        		return ShowNearbyItem15;
        	case 16:
        		return ShowNearbyItem16;
        	case 17:
        		return ShowNearbyItem17;
        	case 18:
        		return ShowNearbyItem18;
        	case 19:
        		return ShowNearbyItem19;
        	case 20:
        		return ShowNearbyItem20;
        	default:
        		return default(bool);
        	}
        }
        public bool SetNearByDragSource (int index, IDragSource val)
        {
        	switch(index)
        	{
        	case 1:
        		NearByDragSource1 = val;
        		break;
        	case 2:
        		NearByDragSource2 = val;
        		break;
        	case 3:
        		NearByDragSource3 = val;
        		break;
        	case 4:
        		NearByDragSource4 = val;
        		break;
        	case 5:
        		NearByDragSource5 = val;
        		break;
        	case 6:
        		NearByDragSource6 = val;
        		break;
        	case 7:
        		NearByDragSource7 = val;
        		break;
        	case 8:
        		NearByDragSource8 = val;
        		break;
        	case 9:
        		NearByDragSource9 = val;
        		break;
        	case 10:
        		NearByDragSource10 = val;
        		break;
        	case 11:
        		NearByDragSource11 = val;
        		break;
        	case 12:
        		NearByDragSource12 = val;
        		break;
        	case 13:
        		NearByDragSource13 = val;
        		break;
        	case 14:
        		NearByDragSource14 = val;
        		break;
        	case 15:
        		NearByDragSource15 = val;
        		break;
        	case 16:
        		NearByDragSource16 = val;
        		break;
        	case 17:
        		NearByDragSource17 = val;
        		break;
        	case 18:
        		NearByDragSource18 = val;
        		break;
        	case 19:
        		NearByDragSource19 = val;
        		break;
        	case 20:
        		NearByDragSource20 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public IDragSource GetNearByDragSource (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return NearByDragSource1;
        	case 2:
        		return NearByDragSource2;
        	case 3:
        		return NearByDragSource3;
        	case 4:
        		return NearByDragSource4;
        	case 5:
        		return NearByDragSource5;
        	case 6:
        		return NearByDragSource6;
        	case 7:
        		return NearByDragSource7;
        	case 8:
        		return NearByDragSource8;
        	case 9:
        		return NearByDragSource9;
        	case 10:
        		return NearByDragSource10;
        	case 11:
        		return NearByDragSource11;
        	case 12:
        		return NearByDragSource12;
        	case 13:
        		return NearByDragSource13;
        	case 14:
        		return NearByDragSource14;
        	case 15:
        		return NearByDragSource15;
        	case 16:
        		return NearByDragSource16;
        	case 17:
        		return NearByDragSource17;
        	case 18:
        		return NearByDragSource18;
        	case 19:
        		return NearByDragSource19;
        	case 20:
        		return NearByDragSource20;
        	default:
        		return default(IDragSource);
        	}
        }
        public bool SetNearByDragSourceId (int index, int val)
        {
        	switch(index)
        	{
        	case 1:
        		NearByDragSourceId1 = val;
        		break;
        	case 2:
        		NearByDragSourceId2 = val;
        		break;
        	case 3:
        		NearByDragSourceId3 = val;
        		break;
        	case 4:
        		NearByDragSourceId4 = val;
        		break;
        	case 5:
        		NearByDragSourceId5 = val;
        		break;
        	case 6:
        		NearByDragSourceId6 = val;
        		break;
        	case 7:
        		NearByDragSourceId7 = val;
        		break;
        	case 8:
        		NearByDragSourceId8 = val;
        		break;
        	case 9:
        		NearByDragSourceId9 = val;
        		break;
        	case 10:
        		NearByDragSourceId10 = val;
        		break;
        	case 11:
        		NearByDragSourceId11 = val;
        		break;
        	case 12:
        		NearByDragSourceId12 = val;
        		break;
        	case 13:
        		NearByDragSourceId13 = val;
        		break;
        	case 14:
        		NearByDragSourceId14 = val;
        		break;
        	case 15:
        		NearByDragSourceId15 = val;
        		break;
        	case 16:
        		NearByDragSourceId16 = val;
        		break;
        	case 17:
        		NearByDragSourceId17 = val;
        		break;
        	case 18:
        		NearByDragSourceId18 = val;
        		break;
        	case 19:
        		NearByDragSourceId19 = val;
        		break;
        	case 20:
        		NearByDragSourceId20 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public int GetNearByDragSourceId (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return NearByDragSourceId1;
        	case 2:
        		return NearByDragSourceId2;
        	case 3:
        		return NearByDragSourceId3;
        	case 4:
        		return NearByDragSourceId4;
        	case 5:
        		return NearByDragSourceId5;
        	case 6:
        		return NearByDragSourceId6;
        	case 7:
        		return NearByDragSourceId7;
        	case 8:
        		return NearByDragSourceId8;
        	case 9:
        		return NearByDragSourceId9;
        	case 10:
        		return NearByDragSourceId10;
        	case 11:
        		return NearByDragSourceId11;
        	case 12:
        		return NearByDragSourceId12;
        	case 13:
        		return NearByDragSourceId13;
        	case 14:
        		return NearByDragSourceId14;
        	case 15:
        		return NearByDragSourceId15;
        	case 16:
        		return NearByDragSourceId16;
        	case 17:
        		return NearByDragSourceId17;
        	case 18:
        		return NearByDragSourceId18;
        	case 19:
        		return NearByDragSourceId19;
        	case 20:
        		return NearByDragSourceId20;
        	default:
        		return default(int);
        	}
        }
        public bool SetNearByDragSourceSlot (int index, int val)
        {
        	switch(index)
        	{
        	case 1:
        		NearByDragSourceSlot1 = val;
        		break;
        	case 2:
        		NearByDragSourceSlot2 = val;
        		break;
        	case 3:
        		NearByDragSourceSlot3 = val;
        		break;
        	case 4:
        		NearByDragSourceSlot4 = val;
        		break;
        	case 5:
        		NearByDragSourceSlot5 = val;
        		break;
        	case 6:
        		NearByDragSourceSlot6 = val;
        		break;
        	case 7:
        		NearByDragSourceSlot7 = val;
        		break;
        	case 8:
        		NearByDragSourceSlot8 = val;
        		break;
        	case 9:
        		NearByDragSourceSlot9 = val;
        		break;
        	case 10:
        		NearByDragSourceSlot10 = val;
        		break;
        	case 11:
        		NearByDragSourceSlot11 = val;
        		break;
        	case 12:
        		NearByDragSourceSlot12 = val;
        		break;
        	case 13:
        		NearByDragSourceSlot13 = val;
        		break;
        	case 14:
        		NearByDragSourceSlot14 = val;
        		break;
        	case 15:
        		NearByDragSourceSlot15 = val;
        		break;
        	case 16:
        		NearByDragSourceSlot16 = val;
        		break;
        	case 17:
        		NearByDragSourceSlot17 = val;
        		break;
        	case 18:
        		NearByDragSourceSlot18 = val;
        		break;
        	case 19:
        		NearByDragSourceSlot19 = val;
        		break;
        	case 20:
        		NearByDragSourceSlot20 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public int GetNearByDragSourceSlot (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return NearByDragSourceSlot1;
        	case 2:
        		return NearByDragSourceSlot2;
        	case 3:
        		return NearByDragSourceSlot3;
        	case 4:
        		return NearByDragSourceSlot4;
        	case 5:
        		return NearByDragSourceSlot5;
        	case 6:
        		return NearByDragSourceSlot6;
        	case 7:
        		return NearByDragSourceSlot7;
        	case 8:
        		return NearByDragSourceSlot8;
        	case 9:
        		return NearByDragSourceSlot9;
        	case 10:
        		return NearByDragSourceSlot10;
        	case 11:
        		return NearByDragSourceSlot11;
        	case 12:
        		return NearByDragSourceSlot12;
        	case 13:
        		return NearByDragSourceSlot13;
        	case 14:
        		return NearByDragSourceSlot14;
        	case 15:
        		return NearByDragSourceSlot15;
        	case 16:
        		return NearByDragSourceSlot16;
        	case 17:
        		return NearByDragSourceSlot17;
        	case 18:
        		return NearByDragSourceSlot18;
        	case 19:
        		return NearByDragSourceSlot19;
        	case 20:
        		return NearByDragSourceSlot20;
        	default:
        		return default(int);
        	}
        }
        public bool SetNearbyItemIcon (int index, Sprite val)
        {
        	switch(index)
        	{
        	case 1:
        		NearbyItemIcon1 = val;
        		break;
        	case 2:
        		NearbyItemIcon2 = val;
        		break;
        	case 3:
        		NearbyItemIcon3 = val;
        		break;
        	case 4:
        		NearbyItemIcon4 = val;
        		break;
        	case 5:
        		NearbyItemIcon5 = val;
        		break;
        	case 6:
        		NearbyItemIcon6 = val;
        		break;
        	case 7:
        		NearbyItemIcon7 = val;
        		break;
        	case 8:
        		NearbyItemIcon8 = val;
        		break;
        	case 9:
        		NearbyItemIcon9 = val;
        		break;
        	case 10:
        		NearbyItemIcon10 = val;
        		break;
        	case 11:
        		NearbyItemIcon11 = val;
        		break;
        	case 12:
        		NearbyItemIcon12 = val;
        		break;
        	case 13:
        		NearbyItemIcon13 = val;
        		break;
        	case 14:
        		NearbyItemIcon14 = val;
        		break;
        	case 15:
        		NearbyItemIcon15 = val;
        		break;
        	case 16:
        		NearbyItemIcon16 = val;
        		break;
        	case 17:
        		NearbyItemIcon17 = val;
        		break;
        	case 18:
        		NearbyItemIcon18 = val;
        		break;
        	case 19:
        		NearbyItemIcon19 = val;
        		break;
        	case 20:
        		NearbyItemIcon20 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Sprite GetNearbyItemIcon (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return NearbyItemIcon1;
        	case 2:
        		return NearbyItemIcon2;
        	case 3:
        		return NearbyItemIcon3;
        	case 4:
        		return NearbyItemIcon4;
        	case 5:
        		return NearbyItemIcon5;
        	case 6:
        		return NearbyItemIcon6;
        	case 7:
        		return NearbyItemIcon7;
        	case 8:
        		return NearbyItemIcon8;
        	case 9:
        		return NearbyItemIcon9;
        	case 10:
        		return NearbyItemIcon10;
        	case 11:
        		return NearbyItemIcon11;
        	case 12:
        		return NearbyItemIcon12;
        	case 13:
        		return NearbyItemIcon13;
        	case 14:
        		return NearbyItemIcon14;
        	case 15:
        		return NearbyItemIcon15;
        	case 16:
        		return NearbyItemIcon16;
        	case 17:
        		return NearbyItemIcon17;
        	case 18:
        		return NearbyItemIcon18;
        	case 19:
        		return NearbyItemIcon19;
        	case 20:
        		return NearbyItemIcon20;
        	default:
        		return default(Sprite);
        	}
        }
        public bool SetNearbyItemName (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		NearbyItemName1 = val;
        		break;
        	case 2:
        		NearbyItemName2 = val;
        		break;
        	case 3:
        		NearbyItemName3 = val;
        		break;
        	case 4:
        		NearbyItemName4 = val;
        		break;
        	case 5:
        		NearbyItemName5 = val;
        		break;
        	case 6:
        		NearbyItemName6 = val;
        		break;
        	case 7:
        		NearbyItemName7 = val;
        		break;
        	case 8:
        		NearbyItemName8 = val;
        		break;
        	case 9:
        		NearbyItemName9 = val;
        		break;
        	case 10:
        		NearbyItemName10 = val;
        		break;
        	case 11:
        		NearbyItemName11 = val;
        		break;
        	case 12:
        		NearbyItemName12 = val;
        		break;
        	case 13:
        		NearbyItemName13 = val;
        		break;
        	case 14:
        		NearbyItemName14 = val;
        		break;
        	case 15:
        		NearbyItemName15 = val;
        		break;
        	case 16:
        		NearbyItemName16 = val;
        		break;
        	case 17:
        		NearbyItemName17 = val;
        		break;
        	case 18:
        		NearbyItemName18 = val;
        		break;
        	case 19:
        		NearbyItemName19 = val;
        		break;
        	case 20:
        		NearbyItemName20 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetNearbyItemName (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return NearbyItemName1;
        	case 2:
        		return NearbyItemName2;
        	case 3:
        		return NearbyItemName3;
        	case 4:
        		return NearbyItemName4;
        	case 5:
        		return NearbyItemName5;
        	case 6:
        		return NearbyItemName6;
        	case 7:
        		return NearbyItemName7;
        	case 8:
        		return NearbyItemName8;
        	case 9:
        		return NearbyItemName9;
        	case 10:
        		return NearbyItemName10;
        	case 11:
        		return NearbyItemName11;
        	case 12:
        		return NearbyItemName12;
        	case 13:
        		return NearbyItemName13;
        	case 14:
        		return NearbyItemName14;
        	case 15:
        		return NearbyItemName15;
        	case 16:
        		return NearbyItemName16;
        	case 17:
        		return NearbyItemName17;
        	case 18:
        		return NearbyItemName18;
        	case 19:
        		return NearbyItemName19;
        	case 20:
        		return NearbyItemName20;
        	default:
        		return default(string);
        	}
        }
        public bool SetNearbyItemNum (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		NearbyItemNum1 = val;
        		break;
        	case 2:
        		NearbyItemNum2 = val;
        		break;
        	case 3:
        		NearbyItemNum3 = val;
        		break;
        	case 4:
        		NearbyItemNum4 = val;
        		break;
        	case 5:
        		NearbyItemNum5 = val;
        		break;
        	case 6:
        		NearbyItemNum6 = val;
        		break;
        	case 7:
        		NearbyItemNum7 = val;
        		break;
        	case 8:
        		NearbyItemNum8 = val;
        		break;
        	case 9:
        		NearbyItemNum9 = val;
        		break;
        	case 10:
        		NearbyItemNum10 = val;
        		break;
        	case 11:
        		NearbyItemNum11 = val;
        		break;
        	case 12:
        		NearbyItemNum12 = val;
        		break;
        	case 13:
        		NearbyItemNum13 = val;
        		break;
        	case 14:
        		NearbyItemNum14 = val;
        		break;
        	case 15:
        		NearbyItemNum15 = val;
        		break;
        	case 16:
        		NearbyItemNum16 = val;
        		break;
        	case 17:
        		NearbyItemNum17 = val;
        		break;
        	case 18:
        		NearbyItemNum18 = val;
        		break;
        	case 19:
        		NearbyItemNum19 = val;
        		break;
        	case 20:
        		NearbyItemNum20 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetNearbyItemNum (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return NearbyItemNum1;
        	case 2:
        		return NearbyItemNum2;
        	case 3:
        		return NearbyItemNum3;
        	case 4:
        		return NearbyItemNum4;
        	case 5:
        		return NearbyItemNum5;
        	case 6:
        		return NearbyItemNum6;
        	case 7:
        		return NearbyItemNum7;
        	case 8:
        		return NearbyItemNum8;
        	case 9:
        		return NearbyItemNum9;
        	case 10:
        		return NearbyItemNum10;
        	case 11:
        		return NearbyItemNum11;
        	case 12:
        		return NearbyItemNum12;
        	case 13:
        		return NearbyItemNum13;
        	case 14:
        		return NearbyItemNum14;
        	case 15:
        		return NearbyItemNum15;
        	case 16:
        		return NearbyItemNum16;
        	case 17:
        		return NearbyItemNum17;
        	case 18:
        		return NearbyItemNum18;
        	case 19:
        		return NearbyItemNum19;
        	case 20:
        		return NearbyItemNum20;
        	default:
        		return default(string);
        	}
        }
        public bool SetShowInventoryItem (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		ShowInventoryItem1 = val;
        		break;
        	case 2:
        		ShowInventoryItem2 = val;
        		break;
        	case 3:
        		ShowInventoryItem3 = val;
        		break;
        	case 4:
        		ShowInventoryItem4 = val;
        		break;
        	case 5:
        		ShowInventoryItem5 = val;
        		break;
        	case 6:
        		ShowInventoryItem6 = val;
        		break;
        	case 7:
        		ShowInventoryItem7 = val;
        		break;
        	case 8:
        		ShowInventoryItem8 = val;
        		break;
        	case 9:
        		ShowInventoryItem9 = val;
        		break;
        	case 10:
        		ShowInventoryItem10 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetShowInventoryItem (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return ShowInventoryItem1;
        	case 2:
        		return ShowInventoryItem2;
        	case 3:
        		return ShowInventoryItem3;
        	case 4:
        		return ShowInventoryItem4;
        	case 5:
        		return ShowInventoryItem5;
        	case 6:
        		return ShowInventoryItem6;
        	case 7:
        		return ShowInventoryItem7;
        	case 8:
        		return ShowInventoryItem8;
        	case 9:
        		return ShowInventoryItem9;
        	case 10:
        		return ShowInventoryItem10;
        	default:
        		return default(bool);
        	}
        }
        public bool SetInventoryDragTarget (int index, IDragTarget val)
        {
        	switch(index)
        	{
        	case 1:
        		InventoryDragTarget1 = val;
        		break;
        	case 2:
        		InventoryDragTarget2 = val;
        		break;
        	case 3:
        		InventoryDragTarget3 = val;
        		break;
        	case 4:
        		InventoryDragTarget4 = val;
        		break;
        	case 5:
        		InventoryDragTarget5 = val;
        		break;
        	case 6:
        		InventoryDragTarget6 = val;
        		break;
        	case 7:
        		InventoryDragTarget7 = val;
        		break;
        	case 8:
        		InventoryDragTarget8 = val;
        		break;
        	case 9:
        		InventoryDragTarget9 = val;
        		break;
        	case 10:
        		InventoryDragTarget10 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public IDragTarget GetInventoryDragTarget (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return InventoryDragTarget1;
        	case 2:
        		return InventoryDragTarget2;
        	case 3:
        		return InventoryDragTarget3;
        	case 4:
        		return InventoryDragTarget4;
        	case 5:
        		return InventoryDragTarget5;
        	case 6:
        		return InventoryDragTarget6;
        	case 7:
        		return InventoryDragTarget7;
        	case 8:
        		return InventoryDragTarget8;
        	case 9:
        		return InventoryDragTarget9;
        	case 10:
        		return InventoryDragTarget10;
        	default:
        		return default(IDragTarget);
        	}
        }
        public bool SetInventoryDropSlot (int index, int val)
        {
        	switch(index)
        	{
        	case 1:
        		InventoryDropSlot1 = val;
        		break;
        	case 2:
        		InventoryDropSlot2 = val;
        		break;
        	case 3:
        		InventoryDropSlot3 = val;
        		break;
        	case 4:
        		InventoryDropSlot4 = val;
        		break;
        	case 5:
        		InventoryDropSlot5 = val;
        		break;
        	case 6:
        		InventoryDropSlot6 = val;
        		break;
        	case 7:
        		InventoryDropSlot7 = val;
        		break;
        	case 8:
        		InventoryDropSlot8 = val;
        		break;
        	case 9:
        		InventoryDropSlot9 = val;
        		break;
        	case 10:
        		InventoryDropSlot10 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public int GetInventoryDropSlot (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return InventoryDropSlot1;
        	case 2:
        		return InventoryDropSlot2;
        	case 3:
        		return InventoryDropSlot3;
        	case 4:
        		return InventoryDropSlot4;
        	case 5:
        		return InventoryDropSlot5;
        	case 6:
        		return InventoryDropSlot6;
        	case 7:
        		return InventoryDropSlot7;
        	case 8:
        		return InventoryDropSlot8;
        	case 9:
        		return InventoryDropSlot9;
        	case 10:
        		return InventoryDropSlot10;
        	default:
        		return default(int);
        	}
        }
        public bool SetInventoryDragSource (int index, IDragSource val)
        {
        	switch(index)
        	{
        	case 1:
        		InventoryDragSource1 = val;
        		break;
        	case 2:
        		InventoryDragSource2 = val;
        		break;
        	case 3:
        		InventoryDragSource3 = val;
        		break;
        	case 4:
        		InventoryDragSource4 = val;
        		break;
        	case 5:
        		InventoryDragSource5 = val;
        		break;
        	case 6:
        		InventoryDragSource6 = val;
        		break;
        	case 7:
        		InventoryDragSource7 = val;
        		break;
        	case 8:
        		InventoryDragSource8 = val;
        		break;
        	case 9:
        		InventoryDragSource9 = val;
        		break;
        	case 10:
        		InventoryDragSource10 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public IDragSource GetInventoryDragSource (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return InventoryDragSource1;
        	case 2:
        		return InventoryDragSource2;
        	case 3:
        		return InventoryDragSource3;
        	case 4:
        		return InventoryDragSource4;
        	case 5:
        		return InventoryDragSource5;
        	case 6:
        		return InventoryDragSource6;
        	case 7:
        		return InventoryDragSource7;
        	case 8:
        		return InventoryDragSource8;
        	case 9:
        		return InventoryDragSource9;
        	case 10:
        		return InventoryDragSource10;
        	default:
        		return default(IDragSource);
        	}
        }
        public bool SetInventoryDragSourceId (int index, int val)
        {
        	switch(index)
        	{
        	case 1:
        		InventoryDragSourceId1 = val;
        		break;
        	case 2:
        		InventoryDragSourceId2 = val;
        		break;
        	case 3:
        		InventoryDragSourceId3 = val;
        		break;
        	case 4:
        		InventoryDragSourceId4 = val;
        		break;
        	case 5:
        		InventoryDragSourceId5 = val;
        		break;
        	case 6:
        		InventoryDragSourceId6 = val;
        		break;
        	case 7:
        		InventoryDragSourceId7 = val;
        		break;
        	case 8:
        		InventoryDragSourceId8 = val;
        		break;
        	case 9:
        		InventoryDragSourceId9 = val;
        		break;
        	case 10:
        		InventoryDragSourceId10 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public int GetInventoryDragSourceId (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return InventoryDragSourceId1;
        	case 2:
        		return InventoryDragSourceId2;
        	case 3:
        		return InventoryDragSourceId3;
        	case 4:
        		return InventoryDragSourceId4;
        	case 5:
        		return InventoryDragSourceId5;
        	case 6:
        		return InventoryDragSourceId6;
        	case 7:
        		return InventoryDragSourceId7;
        	case 8:
        		return InventoryDragSourceId8;
        	case 9:
        		return InventoryDragSourceId9;
        	case 10:
        		return InventoryDragSourceId10;
        	default:
        		return default(int);
        	}
        }
        public bool SetInventoryDragSourceSlot (int index, int val)
        {
        	switch(index)
        	{
        	case 1:
        		InventoryDragSourceSlot1 = val;
        		break;
        	case 2:
        		InventoryDragSourceSlot2 = val;
        		break;
        	case 3:
        		InventoryDragSourceSlot3 = val;
        		break;
        	case 4:
        		InventoryDragSourceSlot4 = val;
        		break;
        	case 5:
        		InventoryDragSourceSlot5 = val;
        		break;
        	case 6:
        		InventoryDragSourceSlot6 = val;
        		break;
        	case 7:
        		InventoryDragSourceSlot7 = val;
        		break;
        	case 8:
        		InventoryDragSourceSlot8 = val;
        		break;
        	case 9:
        		InventoryDragSourceSlot9 = val;
        		break;
        	case 10:
        		InventoryDragSourceSlot10 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public int GetInventoryDragSourceSlot (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return InventoryDragSourceSlot1;
        	case 2:
        		return InventoryDragSourceSlot2;
        	case 3:
        		return InventoryDragSourceSlot3;
        	case 4:
        		return InventoryDragSourceSlot4;
        	case 5:
        		return InventoryDragSourceSlot5;
        	case 6:
        		return InventoryDragSourceSlot6;
        	case 7:
        		return InventoryDragSourceSlot7;
        	case 8:
        		return InventoryDragSourceSlot8;
        	case 9:
        		return InventoryDragSourceSlot9;
        	case 10:
        		return InventoryDragSourceSlot10;
        	default:
        		return default(int);
        	}
        }
        public bool SetInventoryItemIcon (int index, Sprite val)
        {
        	switch(index)
        	{
        	case 1:
        		InventoryItemIcon1 = val;
        		break;
        	case 2:
        		InventoryItemIcon2 = val;
        		break;
        	case 3:
        		InventoryItemIcon3 = val;
        		break;
        	case 4:
        		InventoryItemIcon4 = val;
        		break;
        	case 5:
        		InventoryItemIcon5 = val;
        		break;
        	case 6:
        		InventoryItemIcon6 = val;
        		break;
        	case 7:
        		InventoryItemIcon7 = val;
        		break;
        	case 8:
        		InventoryItemIcon8 = val;
        		break;
        	case 9:
        		InventoryItemIcon9 = val;
        		break;
        	case 10:
        		InventoryItemIcon10 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Sprite GetInventoryItemIcon (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return InventoryItemIcon1;
        	case 2:
        		return InventoryItemIcon2;
        	case 3:
        		return InventoryItemIcon3;
        	case 4:
        		return InventoryItemIcon4;
        	case 5:
        		return InventoryItemIcon5;
        	case 6:
        		return InventoryItemIcon6;
        	case 7:
        		return InventoryItemIcon7;
        	case 8:
        		return InventoryItemIcon8;
        	case 9:
        		return InventoryItemIcon9;
        	case 10:
        		return InventoryItemIcon10;
        	default:
        		return default(Sprite);
        	}
        }
        public bool SetInventoryItemName (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		InventoryItemName1 = val;
        		break;
        	case 2:
        		InventoryItemName2 = val;
        		break;
        	case 3:
        		InventoryItemName3 = val;
        		break;
        	case 4:
        		InventoryItemName4 = val;
        		break;
        	case 5:
        		InventoryItemName5 = val;
        		break;
        	case 6:
        		InventoryItemName6 = val;
        		break;
        	case 7:
        		InventoryItemName7 = val;
        		break;
        	case 8:
        		InventoryItemName8 = val;
        		break;
        	case 9:
        		InventoryItemName9 = val;
        		break;
        	case 10:
        		InventoryItemName10 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetInventoryItemName (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return InventoryItemName1;
        	case 2:
        		return InventoryItemName2;
        	case 3:
        		return InventoryItemName3;
        	case 4:
        		return InventoryItemName4;
        	case 5:
        		return InventoryItemName5;
        	case 6:
        		return InventoryItemName6;
        	case 7:
        		return InventoryItemName7;
        	case 8:
        		return InventoryItemName8;
        	case 9:
        		return InventoryItemName9;
        	case 10:
        		return InventoryItemName10;
        	default:
        		return default(string);
        	}
        }
        public bool SetInventoryItemNum (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		InventoryItemNum1 = val;
        		break;
        	case 2:
        		InventoryItemNum2 = val;
        		break;
        	case 3:
        		InventoryItemNum3 = val;
        		break;
        	case 4:
        		InventoryItemNum4 = val;
        		break;
        	case 5:
        		InventoryItemNum5 = val;
        		break;
        	case 6:
        		InventoryItemNum6 = val;
        		break;
        	case 7:
        		InventoryItemNum7 = val;
        		break;
        	case 8:
        		InventoryItemNum8 = val;
        		break;
        	case 9:
        		InventoryItemNum9 = val;
        		break;
        	case 10:
        		InventoryItemNum10 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetInventoryItemNum (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return InventoryItemNum1;
        	case 2:
        		return InventoryItemNum2;
        	case 3:
        		return InventoryItemNum3;
        	case 4:
        		return InventoryItemNum4;
        	case 5:
        		return InventoryItemNum5;
        	case 6:
        		return InventoryItemNum6;
        	case 7:
        		return InventoryItemNum7;
        	case 8:
        		return InventoryItemNum8;
        	case 9:
        		return InventoryItemNum9;
        	case 10:
        		return InventoryItemNum10;
        	default:
        		return default(string);
        	}
        }
        public string ResourceBundleName { get { return "uiprefabs/common"; } }
        public string ResourceAssetName { get { return "CommonBag"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
        public string UiConfigAssetName { get { return "CommonCross" ; } }
    }
}
