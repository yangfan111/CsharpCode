using App.Server.GameModules.GamePlay;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.skill;
using gameplay.gamerule.free.rule;
using gameplay.gamerule.free.ui;

namespace gameplay.gamerule.free.item
{
    public class ItemInventoryUtil : ParaConstant
    {
        public static void MovePosition(ItemPosition ip, ItemInventory toIn, int x, int y, ISkillArgs args)
        {
            ItemInventory fromIn = ip.GetInventory();
            IInventoryUI fromUI = fromIn.GetInventoryUI();
            IInventoryUI toUI = toIn.GetInventoryUI();
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            Move(fromIn, toIn, ip, x, y, args, fr, fromUI, toUI);
        }

        public static void MovePosition(ItemInventory fromIn, ItemInventory toIn, SimpleInventoryUI fromUI, SimpleInventoryUI toUI, ItemPosition ip, int x, int y, ISkillArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            int oneX = toUI.GetWidth(args) / toIn.GetColumn();
            int countX = x / (oneX);
            int remain = x % (oneX);
            if (fromIn == toIn)
            {
                if (MyMath.Abs(remain) > oneX / 2)
                {
                    if (remain > 0)
                    {
                        countX++;
                    }
                    else
                    {
                        countX--;
                    }
                }
                countX = countX + ip.GetX();
            }
            else
            {
                if (countX < 0)
                {
                    countX = 0;
                }
            }
            int oneY = toUI.GetHeight(args) / toIn.GetRow();
            int countY = y / (oneY);
            remain = y % (oneY);
            if (fromIn == toIn)
            {
                if (MyMath.Abs(remain) > oneY / 2)
                {
                    if (remain > 0)
                    {
                        countY++;
                    }
                    else
                    {
                        countY--;
                    }
                }
                countY = countY + ip.GetY();
            }
            else
            {
                if (countY < 0)
                {
                    countY = 0;
                }
                //countY = toIn.row - countY - 1;
            }
            if (MoveOut(fromIn, toIn, ip, countX, countY, args, fr, fromUI, toUI))
            {
                return;
            }
            Move(fromIn, toIn, ip, countX, countY, args, fr, fromUI, toUI);
        }

        public static bool MoveOut(ItemInventory fromIn, ItemInventory toIn, ItemPosition ip, int countX, int countY, ISkillArgs args, FreeRuleEventArgs fr, IInventoryUI fromUI, IInventoryUI toUI)
        {
            if (fromIn == toIn)
            {
                // 竖格超出
                if (countY >= toIn.GetRow() || countY < 0)
                {
                    if (toUI.MoveOutAction != null)
                    {
                        ip.GetKey().SetCount(ip.GetCount());
                        fr.TempUse(PARA_ITEM, ip.GetKey());
                        fr.Resume(PARA_ITEM);
                        RemoveItem(fromIn, ip, args);

                        toUI.MoveOutAction.Act(args);
                        return true;
                    }
                }
            }
            if (fromIn == toIn)
            {
                // 横格超出
                if (countX >= toIn.GetColumn() || countX < 0)
                {
                    if (toUI.MoveOutAction != null)
                    {
                        ip.GetKey().SetCount(ip.GetCount());
                        fr.TempUse(PARA_ITEM, ip.GetKey());
                        fr.Resume(PARA_ITEM);
                        RemoveItem(fromIn, ip, args);

                        toUI.MoveOutAction.Act(args);
                        return true;
                    }
                }
            }
            return false;
        }

        public static void Move(ItemInventory fromIn, ItemInventory toIn, ItemPosition ip, int countX, int countY, ISkillArgs args, FreeRuleEventArgs fr, IInventoryUI fromUI, IInventoryUI toUI)
        {
            if (fromIn != toIn)
            {
                fr.TempUse(PARA_ITEM, ip);
                bool canDrop = toIn.IsCanDrop(ip, args);
                // 如果toIn 不可以拖入物品
                if (!canDrop)
                {
                    fromUI.UpdateItem(args, fromIn, ip);
                    if (toIn.GetDropAction() != null)
                    {
                        toIn.GetDropAction().Act(args);
                    }
                    //HandleMoveAction(fromIn, toIn, fromUI, toUI, ip, args);
                    fr.Resume(PARA_ITEM);
                    return;
                }
                fr.Resume(PARA_ITEM);
            }
            // 已有
            ItemPosition[] olds = toIn.GetItem(countX, countY, ip.GetKey().GetGridWidth(), ip.GetKey().GetGridHeight());
            ItemPosition old = null;
            if (olds.Length == 1)
            {
                old = olds[0];
                if (old != ip)
                {
                    fr.TempUse(PARA_ITEM, ip);
                    if (old.GetKey().GetKey().Equals(ip.GetKey().GetKey()))
                    {
                        int delta = old.GetKey().GetItemStack() - old.GetCount();
                        if (delta > 0)
                        {
                            // 堆叠物品
                            ChangeItemStack(delta, fromIn, toIn, fromUI, toUI, ip, old, args);
                        }
                        else
                        {
                            // 交换物品位置
                            ExchangeItem(fromIn, toIn, fromUI, toUI, ip, old, args);
                        }
                    }
                    else
                    {
                        if (!ip.DragTo(args, old))
                        {
                            ExchangeItem(fromIn, toIn, fromUI, toUI, ip, old, args);
                        }
                        else
                        {
                            if (ip.GetInventory() != null)
                            {
                                fromUI.UpdateItem(args, fromIn, ip);
                            }
                        }
                    }
                    fr.Resume(PARA_ITEM);
                    return;
                }
            }
            fr.TempUse(PARA_ITEM, ip);
            MoveItem(countX, countY, fromIn, toIn, fromUI, toUI, ip, old, args);
            fr.Resume(PARA_ITEM);
        }

        private static void MoveItem(int countX, int countY, ItemInventory fromIn, ItemInventory toIn, IInventoryUI fromUI, IInventoryUI toUI, ItemPosition ip, ItemPosition old, ISkillArgs args)
        {
            if (fromIn.CanMoveTo(ip, toIn, countX, countY))
            {
                fromIn.ChangePosition(ip, toIn, countX, countY);
                HandleMoveAction(fromIn, toIn, fromUI, toUI, ip, args);
                if (fromIn == toIn)
                {
                    fromUI.UpdateItem(args, fromIn, ip);
                }
                else
                {
                    fromUI.DeleteItem(args, fromIn, ip);
                    toUI.AddItem(args, toIn, ip);
                }
            }
            else
            {
                fromIn.ChangePosition(ip, fromIn, ip.GetX(), ip.GetY());
                if (fromUI.CanNotMoveAction != null)
                {
                    fromUI.CanNotMoveAction.Act(args);
                }
                fromUI.UpdateItem(args, fromIn, ip);
            }
        }

        private static void ExchangeItem(ItemInventory fromIn, ItemInventory toIn, IInventoryUI fromUI, IInventoryUI toUI, ItemPosition ip, ItemPosition old, ISkillArgs args)
        {
            if (fromIn.CanExchange(old, ip) && fromIn.IsCanDrop(old, args))
            {
                fromIn.ExchangeItemPosition(old, ip);
                HandleMoveAction(fromIn, toIn, fromUI, toUI, ip, args, true);
                HandleMoveAction(toIn, fromIn, toUI, fromUI, old, args, true);
                HandleMoveAction(fromIn, toIn, fromUI, toUI, ip, args, false);
                HandleMoveAction(toIn, fromIn, toUI, fromUI, old, args, false);
                if (fromIn == toIn)
                {
                    fromUI.UpdateItem(args, fromIn, old);
                    fromUI.UpdateItem(args, fromIn, ip);
                }
                else
                {
                    fromUI.AddItem(args, fromIn, old);
                    toUI.AddItem(args, toIn, ip);
                }
            }
            else
            {
                fromIn.ChangePosition(ip, fromIn, ip.GetX(), ip.GetY());
                if (fromUI.CanNotMoveAction != null)
                {
                    fromUI.CanNotMoveAction.Act(args);
                }
                fromUI.UpdateItem(args, fromIn, ip);
            }
        }

        private static void ChangeItemStack(int delta, ItemInventory fromIn, ItemInventory toIn, IInventoryUI fromUI, IInventoryUI toUI, ItemPosition ip, ItemPosition old, ISkillArgs args)
        {
            if (ip.GetCount() <= delta)
            {
                RemoveItem(fromIn, ip, args);
                HandleMoveAction(fromIn, toIn, fromUI, toUI, ip, args, true);

                old.SetCount(old.GetCount() + (int)MyMath.Min(delta, ip.GetCount()));
                toUI.UpdateItem(args, toIn, old);
            }
            else
            {
                old.SetCount(old.GetKey().GetItemStack());
                ip.SetCount(ip.GetCount() - delta);
                toUI.UpdateItem(args, toIn, old);
                fromUI.UpdateItem(args, fromIn, ip);
            }
        }

        private static void HandleMoveAction(ItemInventory fromIn, ItemInventory toIn, IInventoryUI fromUI, IInventoryUI toUI, ItemPosition ip, IEventArgs args)
        {
            HandleMoveAction(fromIn, toIn, fromUI, toUI, ip, args, true);
            HandleMoveAction(fromIn, toIn, fromUI, toUI, ip, args, false);
        }

        public static void HandleMoveAction(ItemInventory fromIn, ItemInventory toIn, IInventoryUI fromUI, IInventoryUI toUI, ItemPosition ip, IEventArgs args, bool remove)
        {
            args.TempUse(PARA_ITEM, ip);
            args.GetDefault().GetParameters().TempUse(new StringPara(PARA_ITEM_MOVE_FROM, fromIn.GetName()));
            args.GetDefault().GetParameters().TempUse(new StringPara(PARA_ITEM_MOVE_TO, toIn.GetName()));
            if (remove)
            {
                if (fromUI.MoveAction != null)
                {
                    fromUI.MoveAction.Act(args);
                }
            }
            else
            {
                if (toUI != fromUI)
                {
                    if (toUI.MoveAction != null)
                    {
                        toUI.MoveAction.Act(args);
                    }
                }
            }

            args.Resume(PARA_ITEM);
            args.GetDefault().GetParameters().Resume(PARA_ITEM_MOVE_FROM);
            args.GetDefault().GetParameters().Resume(PARA_ITEM_MOVE_TO);
        }

        private static void RemoveItem(ItemInventory inventory, ItemPosition ip, ISkillArgs args)
        {
            inventory.RemoveItem(args, ip);
            RemoveUI(ip, args);
        }

        private static void RemoveUI(ItemPosition ip, ISkillArgs args)
        {
            FreeUIDeleteAction del = new FreeUIDeleteAction();
            del.SetKey(ip.GetUIKey());
            del.SetScope(1);
            del.SetPlayer(PARA_PLAYER_CURRENT);
            del.Act(args);
        }
    }
}
