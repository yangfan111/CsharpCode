using App.Client.GameModules.Ui.UiAdapter;
using App.Shared;
using Assets.App.Client.GameModules.Ui;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Assets.Sources.Free.Utility;
using Assets.Utils.Configuration;
using Core;
using Core.Free;
using Free.framework;
using I2.Loc;
using Sharpen;
using System;
using System.Linq;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.App
{
    public class CommonTipHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.CommonTipUI || key == FreeMessageConstant.ClearCommonTipUI || key == FreeMessageConstant.ChickenTip;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            var ui = contexts.ui.uI;
            if (data.Key == FreeMessageConstant.CommonTipUI)
            {
                int type = data.Ks[0];

                BaseTipData tip = new BaseTipData
                {
                    Title = data.Ss[0],
                    DurationTime = data.Ins[0]
                };

                if (type == 1)
                {
                    contexts.ui.uISession.UiState[UiNameConstant.CommonOperationTipModel] = true;
                    ui.OperationTipData = tip;
                }
                else if (type == 2)
                {
                    contexts.ui.uISession.UiState[UiNameConstant.CommonSystemTipModel] = true;
                    ui.SystemTipDataQueue.Enqueue(tip);
                }
            }

            if (data.Key == FreeMessageConstant.ClearCommonTipUI)
            {
                int type = data.Ks[0];
                if (type == 1)
                {
                    contexts.ui.uISession.UiState[UiNameConstant.CommonOperationTipModel] = false;
                }
                else if (type == 2)
                {
                    //contexts.ui.uISession.UiState[UiNameConstant.CommonSystemTipModel] = false;
                    ui.SystemTipDataQueue.Clear();
                }
            }

            if (data.Key == FreeMessageConstant.ChickenTip)
            {
                if (contexts.player.flagSelfEntity.gamePlay.TipHideStatus) return;

                string[] tips = StringExtension.Split(data.Ss[0], ",");
                string tipStr = String.Empty;
                switch ("client/chickentip/" + tips[0])
                {
                    case ScriptTerms.client_chickentip.word4:
                        tipStr = ScriptLocalization.client_chickentip.word4;
                        break;
                    case ScriptTerms.client_chickentip.word5:
                        tipStr = ScriptLocalization.client_chickentip.word5;
                        break;
                    case ScriptTerms.client_chickentip.word6:
                        tipStr = ScriptLocalization.client_chickentip.word6;
                        tips[2] = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(short.Parse(tips[2])).S_Name;
                        break;
                    case ScriptTerms.client_chickentip.word13:
                        tipStr = ScriptLocalization.client_chickentip.word13;
                        break;
                    case ScriptTerms.client_chickentip.word50:
                        tipStr = ScriptLocalization.client_chickentip.word50;
                        break;
                    case ScriptTerms.client_chickentip.word51:
                        tipStr = ScriptLocalization.client_chickentip.word51;
                        break;
                    case ScriptTerms.client_chickentip.word52:
                        tipStr = ScriptLocalization.client_chickentip.word52;
                        break;
                    case ScriptTerms.client_chickentip.word53:
                        tipStr = ScriptLocalization.client_chickentip.word53;
                        break;
                    case ScriptTerms.client_chickentip.word54:
                        tipStr = ScriptLocalization.client_chickentip.word54;
                        break;
                    case ScriptTerms.client_chickentip.word55:
                        tipStr = ScriptLocalization.client_chickentip.word55;
                        break;
                    case ScriptTerms.client_chickentip.word56:
                        tipStr = ScriptLocalization.client_chickentip.word56;
                        break;
                    case ScriptTerms.client_chickentip.word57:
                        tipStr = ScriptLocalization.client_chickentip.word57;
                        break;
                    case ScriptTerms.client_chickentip.word58:
                        tipStr = ScriptLocalization.client_chickentip.word58;
                        break;
                    case ScriptTerms.client_chickentip.word59:
                        tipStr = ScriptLocalization.client_chickentip.word59;
                        break;
                    case ScriptTerms.client_chickentip.word60:
                        tipStr = ScriptLocalization.client_chickentip.word60;
                        break;
                    case ScriptTerms.client_chickentip.word61:
                        tipStr = ScriptLocalization.client_chickentip.word61;
                        break;
                    case ScriptTerms.client_chickentip.word62:
                        tipStr = ScriptLocalization.client_chickentip.word62;
                        break;
                    case ScriptTerms.client_chickentip.word63:
                        tipStr = ScriptLocalization.client_chickentip.word63;
                        break;
                    case ScriptTerms.client_chickentip.word64:
                        tipStr = ScriptLocalization.client_chickentip.word64;
                        break;
                    case ScriptTerms.client_chickentip.word65:
                        tipStr = ScriptLocalization.client_chickentip.word65;
                        break;
                    case ScriptTerms.client_chickentip.word66:
                        tipStr = ScriptLocalization.client_chickentip.word66;
                        break;
                    case ScriptTerms.client_chickentip.word67:
                        tipStr = ScriptLocalization.client_chickentip.word67;
                        break;
                    case ScriptTerms.client_chickentip.word68:
                        tipStr = ScriptLocalization.client_chickentip.word68;
                        break;
                    case ScriptTerms.client_chickentip.word69:
                        tipStr = ScriptLocalization.client_chickentip.word69;
                        break;
                    case ScriptTerms.client_chickentip.word70:
                        tipStr = ScriptLocalization.client_chickentip.word70;
                        break;
                    case ScriptTerms.client_chickentip.word71:
                        tipStr = ScriptLocalization.client_chickentip.word71;
                        break;
                    case ScriptTerms.client_chickentip.word72:
                        tipStr = ScriptLocalization.client_chickentip.word72;
                        break;
                    case ScriptTerms.client_chickentip.word73:
                        tipStr = ScriptLocalization.client_chickentip.word73;
                        break;
                    case ScriptTerms.client_chickentip.word74:
                        tipStr = ScriptLocalization.client_chickentip.word74;
                        break;
                    case ScriptTerms.client_chickentip.word75:
                        tipStr = ScriptLocalization.client_chickentip.word75;
                        break;
                    case ScriptTerms.client_chickentip.word76:
                        tipStr = ScriptLocalization.client_chickentip.word76;
                        break;
                    case ScriptTerms.client_chickentip.word77:
                        tipStr = ScriptLocalization.client_chickentip.word77;
                        break;
                    case ScriptTerms.client_chickentip.word78:
                        tipStr = ScriptLocalization.client_chickentip.word78;
                        break;
                    case ScriptTerms.client_chickentip.word79:
                        tipStr = ScriptLocalization.client_chickentip.word79;
                        break;
                    case ScriptTerms.client_chickentip.word80:
                        tipStr = ScriptLocalization.client_chickentip.word80;
                        break;
                    default:
                        break;
                }

                int[] notError = new int[]{4, 5, 6, 72, 75, 80};
                if (!notError.ToList().Contains(int.Parse(tips[0].Substring(4))))
                {
                    contexts.player.flagSelfEntity.AudioController().PlaySimpleAudio((EAudioUniqueId) 5024, false);
                }

                if (tips.Length > 1)
                {
                    tips = tips.SubList(1, tips.Length - 1).ToArray();
                }

                BaseTipData tip = new BaseTipData
                {
                    Title = string.Format(tipStr, tips),
                    DurationTime = 5000
                };

                if (data.Bs.Count > 0 && data.Bs[0])
                {
                    contexts.ui.uISession.UiState[UiNameConstant.CommonSystemTipModel] = true;
                    ui.SystemTipDataQueue.Enqueue(tip);
                }
                else
                {
                    contexts.ui.uISession.UiState[UiNameConstant.CommonOperationTipModel] = true;
                    ui.OperationTipData = tip;
                }

            }
        }

    }
}
