using App.Shared.Player;
using com.wd.free.@event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player;

namespace App.Server.GameModules.GamePlay.Free.player
{
    public class PlayerStateTimer
    {
        private Dictionary<int, List<long>> uiTimer;
        private Dictionary<int, List<long>> gameTimer;

        private PlayerEntity player;

        public PlayerStateTimer(PlayerEntity player)
        {
            this.uiTimer = new Dictionary<int, List<long>>();
            this.gameTimer = new Dictionary<int, List<long>>();

            this.player = player;
        }

        public void AddGameStateTime(EPlayerGameState state, long serverTime)
        {
            if (!gameTimer.ContainsKey((int)state))
            {
                gameTimer.Add((int)state, new List<long>());
            }

            gameTimer[(int)state].Add(serverTime);
        }

        public void AddUITime(EPlayerUIState state, long serverTime)
        {
            if (!uiTimer.ContainsKey((int)state))
            {
                uiTimer.Add((int)state, new List<long>());
            }

            uiTimer[(int)state].Add(serverTime);
        }

        public int GetUIStateCount(EPlayerUIState state)
        {
            if (uiTimer.ContainsKey((int)state))
            {
                return uiTimer[(int)state].Count;
            }

            return 0;
        }

        public int GetGameStateCount(EPlayerGameState state)
        {
            if (gameTimer.ContainsKey((int)state))
            {
                return gameTimer[(int)state].Count;
            }

            return 0;
        }

        public void AutoRemove(IEventArgs args, long serverTime)
        {
            Dictionary<int, List<long>>.Enumerator uiTimerIt = uiTimer.GetEnumerator();
            while (uiTimerIt.MoveNext()) {
                List<long> times = uiTimerIt.Current.Value;
                int count = times.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (times[i] < serverTime)
                    {
                        times.Remove(times[i]);
                        count--;
                    }
                }
                if (count == 0)
                {
                    PlayerStateUtil.RemoveUIState((EPlayerUIState)uiTimerIt.Current.Key, player.gamePlay);
                }
            }

            Dictionary<int, List<long>>.Enumerator gameTimerIt = gameTimer.GetEnumerator();
            while (gameTimerIt.MoveNext())
            {
                List<long> times = gameTimerIt.Current.Value;
                int count = times.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (times[i] < serverTime)
                    {
                        times.Remove(times[i]);
                        count--;
                    }
                }
                if (count == 0)
                {
                    PlayerStateUtil.RemoveGameState((EPlayerGameState)gameTimerIt.Current.Key, player.gamePlay);
                }
            }
        }
    }
}
