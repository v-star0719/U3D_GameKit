using System;
using System.Collections.Generic;

namespace Kernel.Core
{
	public class EventManager
	{
		public static EventManager Instance = new EventManager();
		private readonly Dictionary<Enum, List<Action<Object>>> subscriptions = new Dictionary<Enum, List<Action<Object>>>();
		private readonly List<Enum> tickEvents = new List<Enum>();
		private readonly List<Object> tickEventsArg = new List<Object>();


		public virtual void FireEvent(Enum id, object arg = null)
		{
			tickEvents.Add(id);
			tickEventsArg.Add(arg);
		}

		public virtual void FireEventImmediate(Enum id, object arg = null)
		{
			List<Action<Object>> subscription = null;
			if(subscriptions.TryGetValue(id, out subscription))
			{
				foreach(var action in subscription)
				{
					action(arg);
				}
			}
		}

		public virtual void Tick(float deltaTime)
		{
			if(tickEvents.Count > 0)
			{
				for (var index = 0; index < tickEvents.Count; index++)
				{
					Enum t = tickEvents[index];
					FireEventImmediate(t, tickEventsArg[index]);
				}

				tickEvents.Clear();
				tickEventsArg.Clear();
			}
		}

		public virtual void SubscribeEvent(Enum id, Action<Object> handler)
		{
			List<Action<Object>> subscription = null;
			if (!subscriptions.TryGetValue(id, out subscription))
			{
				subscription = new List<Action<Object>>();
				subscriptions.Add(id, subscription);
			}

			if (subscription.Contains(handler))
			{
				UnityEngine.Debug.LogErrorFormat("resubscribe, id = {0}, handle = {1}", id, handler);
				return;
			}

			subscription.Add(handler);
		}

		public virtual void UnsubscribeEvent(Enum id, Action<Object> handler)
		{
			List<Action<Object>> subscription = null;
			if(subscriptions.TryGetValue(id, out subscription))
			{
				subscription.Remove(handler);
			}
		}

		public void Destroy()
		{
			tickEvents.Clear();
			subscriptions.Clear();
		}
	}
}