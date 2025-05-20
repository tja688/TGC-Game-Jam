    using System;
    using System.Collections.Generic;
    
    
    public static class EventCenter
    {
        #region 内部类 
        private interface IEventInfo
        {
            void RemoveAllAction();
        }
    
        private class EventInfo : IEventInfo
        {
            private Action action;
    
            public void AddAction(Action action)
            {
                this.action += action;
    
            }
            public void RemoveAction(Action action)
            {
                this.action -= action;
            }
            public void RemoveAllAction()
            {
                this.action = null;
            }
    
            public void Trigger()
            {
                action?.Invoke();
            }
        }
    
        private class EventInfo<T> : IEventInfo
        {
            public Action<T> Action;
    
            public void AddAction(Action<T> action)
            {
                this.Action = action;
            }
            public void RemoveAction(Action<T> action)
            {
                this.Action -= action;
            }
            public void Trigger(T arg)
            {
                Action?.Invoke(arg);
            }
            public void RemoveAllAction()
            {
                this.Action = null;
            }
        }
    
        private class EventInfo<T0, T1> : IEventInfo
        {
            private Action<T0, T1> action;
    
            public void AddAction(Action<T0, T1> action)
            {
                this.action = action;
            }
            public void RemoveAction(Action<T0, T1> action)
            {
                this.action -= action;
            }
            public void Trigger(T0 arg0, T1 arg1)
            {
                action?.Invoke(arg0, arg1);
            }
            public void RemoveAllAction()
            {
                this.action = null;
            }
        }
    
        private class EventInfo<T0, T1, T2> : IEventInfo
        {
            private Action<T0, T1, T2> action;
    
            public void AddAction(Action<T0, T1, T2> action)
            {
                this.action = action;
            }
            public void RemoveAction(Action<T0, T1, T2> action)
            {
                this.action -= action;
            }
            public void Trigger(T0 arg0, T1 arg1, T2 arg2)
            {
                action?.Invoke(arg0, arg1, arg2);
            }
            public void RemoveAllAction()
            {
                this.action = null;
            }
        }
        private class EventInfo<T0, T1, T2, T3> : IEventInfo
        {
            private Action<T0, T1, T2, T3> action;
    
            public void AddAction(Action<T0, T1, T2, T3> action)
            {
                this.action = action;
            }
            public void RemoveAction(Action<T0, T1, T2, T3> action)
            {
                this.action -= action;
            }
            public void Trigger(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
            {
                action?.Invoke(arg0, arg1, arg2, arg3);
            }
            public void RemoveAllAction()
            {
                this.action = null;
            }
        }
        private class EventInfo<T0, T1, T2, T3, T4> : IEventInfo
        {
            private Action<T0, T1, T2, T3, T4> action;
    
            public void AddAction(Action<T0, T1, T2, T3, T4> action)
            {
                this.action = action;
            }
            public void RemoveAction(Action<T0, T1, T2, T3, T4> action)
            {
                this.action -= action;
            }
            public void Trigger(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                action?.Invoke(arg0, arg1, arg2, arg3, arg4);
            }
            public void RemoveAllAction()
            {
                this.action = null;
            }
        }
    
        #endregion
    
    
        // 事件字典
        private static Dictionary<string, IEventInfo> eventInfoDict = new Dictionary<string, IEventInfo>();
    
        #region 添加事件监听
    
        public static void AddEventListener(string name, Action action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                (eventInfoDict[name] as EventInfo).AddAction(action);
            }
            else
            {
                var eventInfo = new EventInfo();
                eventInfo.AddAction(action);
                eventInfoDict.Add(name, eventInfo);
            }
        }
    
        public static void AddEventListener<T>(string name, Action<T> action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                (eventInfoDict[name] as EventInfo<T>).Action += action;
            }
            else
            {
                var eventInfo = new EventInfo<T>();
                eventInfo.AddAction(action);
                eventInfoDict.Add(name, eventInfo);
            }
        }
    
        public static void AddEventListener<T0, T1>(string name, Action<T0, T1> action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                (eventInfoDict[name] as EventInfo<T0, T1>).AddAction(action);
            }
            else
            {
                EventInfo<T0, T1> eventInfo = new EventInfo<T0, T1>();
                eventInfo.AddAction(action);
                eventInfoDict.Add(name, eventInfo);
            }
        }
    
        public static void AddEventListener<T0, T1, T2>(string name, Action<T0, T1, T2> action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                (eventInfoDict[name] as EventInfo<T0, T1, T2>).AddAction(action);
            }
            else
            {
                var eventInfo = new EventInfo<T0, T1, T2>();
                eventInfo.AddAction(action);
                eventInfoDict.Add(name, eventInfo);
            }
        }
        public static void AddEventListener<T0, T1, T2, T3>(string name, Action<T0, T1, T2, T3> action)
        {
            EventInfo<T0, T1, T2, T3> eventInfo = new EventInfo<T0, T1, T2, T3>();
            if (eventInfoDict.ContainsKey(name))
            {
                (eventInfoDict[name] as EventInfo<T0, T1, T2, T3>).AddAction(action);
            }
            else
            {
                eventInfo.AddAction(action);
                eventInfoDict.Add(name, eventInfo);
            }
        }
        public static void AddEventListener<T0, T1, T2, T3, T4>(string name, Action<T0, T1, T2, T3, T4> action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                (eventInfoDict[name] as EventInfo<T0, T1, T2, T3, T4>).AddAction(action);
            }
            else
            {
                var eventInfo = new EventInfo<T0, T1, T2, T3, T4>();
                eventInfo.AddAction(action);
                eventInfoDict.Add(name, eventInfo);
            }
        }
        #endregion
    
        #region 触发事件 
        public static void TriggerEvent(string name)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo)).Trigger();
            }
        }
    
        public static void TriggerEvent<T>(string name, T args)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo<T>))?.Trigger(args);
            }
        }
    
        public static void TriggerEvent<T0, T1>(string name, T0 arg0, T1 arg1)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo<T0, T1>)).Trigger(arg0, arg1);
            }
        }
    
        public static void TriggerEvent<T0, T1, T2>(string name, T0 arg0, T1 arg1, T2 arg2)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo<T0, T1, T2>)).Trigger(arg0, arg1, arg2);
            }
        }
        public static void TriggerEvent<T0, T1, T2, T3>(string name, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo<T0, T1, T2, T3>)).Trigger(arg0, arg1, arg2, arg3);
            }
        }
        public static void TriggerEvent<T0, T1, T2, T3, T4>(string name, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo<T0, T1, T2, T3, T4>)).Trigger(arg0, arg1, arg2, arg3, arg4);
            }
        }
        #endregion
    
        #region 删除某一个事件的指定回调
        /// <summary>
        /// 删除某一个事件的指定回调
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void RemoveListener(string name, Action action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo)).RemoveAction(action);
            }
        }
        /// <summary>
        /// 删除某一个事件的指定回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void RemoveListener<T>(string name, Action<T> action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo<T>)).RemoveAction(action);
            }
        }
        /// <summary>
        /// 删除某一个事件的指定回调
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void RemoveListener<T0, T1>(string name, Action<T0, T1> action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo<T0, T1>)).RemoveAction(action);
            }
        }
        /// <summary>
        /// 删除某一个事件的指定回调
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void RemoveListener<T0, T1, T2>(string name, Action<T0, T1, T2> action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo<T0, T1, T2>)).RemoveAction(action);
            }
        }
        /// <summary>
        /// 删除某一个事件的指定回调
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void RemoveListener<T0, T1, T2, T3>(string name, Action<T0, T1, T2, T3> action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo<T0, T1, T2, T3>)).RemoveAction(action);
            }
        }
        /// <summary>
        /// 删除某一个事件的指定回调
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void RemoveListener<T0, T1, T2, T3, T4>(string name, Action<T0, T1, T2, T3, T4> action)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                ((eventInfoDict[name] as EventInfo<T0, T1, T2, T3, T4>)).RemoveAction(action);
            }
        }
        #endregion
    
        #region 移除所有事件监听 && 清空事件字典
        /// <summary>
        /// 移除指定事件的所有回调 , 某一个事件不需要了时调用
        /// </summary>
        /// <param name="name"></param>
        public static void RemoveListener(string name)
        {
            if (eventInfoDict.ContainsKey(name))
            {
                eventInfoDict[name].RemoveAllAction();
                eventInfoDict.Remove(name);
            }
        }
    
        /// <summary>
        /// 移除所有事件的所有回调 退出游戏时调用
        /// </summary>
        public static void ClearAllListeners()
        {
            foreach (string name in eventInfoDict.Keys)
            {
                eventInfoDict[name].RemoveAllAction();
            }
            eventInfoDict.Clear();
        }
        #endregion
    }