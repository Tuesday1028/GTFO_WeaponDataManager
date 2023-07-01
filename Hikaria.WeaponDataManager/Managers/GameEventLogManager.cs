using System;
using System.Collections.Generic;
using CellMenu;
using UnityEngine;

namespace Hikaria.WeaponDataLoader.Managers
{
    internal sealed class GameEventLogManager : MonoBehaviour
    {
        private void Awake()
        {
            Current = this;
        }

        private void Update()
        {
            if (_interval > 0f)
            {
                _interval -= Time.deltaTime;
                return;
            }
            if (_gameEventLogs.Count > 0)
            {
                _puiGameEventLog.AddLogItem(_gameEventLogs[0], eGameEventChatLogType.GameEvent);
                CM_PageLoadout.Current.m_gameEventLog.AddLogItem(_gameEventLogs[0], eGameEventChatLogType.GameEvent);
                _gameEventLogs.RemoveAt(0);
                _interval = 0.5f;
            }
        }

        public void AddLog(string log)
        {
            _gameEventLogs.Add(log);
        }

        private static string[] getstr(string strs, int len)
        {
            string[] array = new string[int.Parse(Math.Ceiling((double)strs.Length / (double)len).ToString())];
            for (int i = 0; i < array.Length; i++)
            {
                len = ((len <= strs.Length) ? len : strs.Length);
                array[i] = strs.Substring(0, len);
                strs = strs.Substring(len, strs.Length - len);
            }
            return array;
        }

        public void AddLogInSplit(string str, int len = 50)
        {
            if (str.Length > len)
            {
                string[] array = getstr(str, len);
                for (int i = 0; i < array.Length; i++)
                {
                    AddLog(array[i]);
                }
                return;
            }
            AddLog(str);
        }

        public void AddLogInSplit(string str, char ch)
        {
            string[] array = str.Split(ch);
            for (int i = 0; i < array.Length; i++)
            {
                AddLog(array[i]);
            }
            return;
        }

        public static GameEventLogManager Current;

        public List<string> _gameEventLogs = new List<string>();

        private float _interval;

        private PUI_GameEventLog _puiGameEventLog
        {
            get
            {
                return GuiManager.PlayerLayer.m_gameEventLog;
            }
        }
    }
}
