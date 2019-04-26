using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel
{
	public class Event
	{
		public int id;
	}

	public enum Usage
	{
		IMMEDIATE,
		AFTER_TICK
	}

	public class EventManager
	{
		public static EventManager instance = new EventManager();

		private readonly Dictionary<int, List<Action<Event>>> subscriptions = new Dictionary<int, List<Action<Event>>>();
		private readonly List<Event> tickEvents = new List<Event>();

		public virtual void FireEvent(Event e, Usage usage = Usage.IMMEDIATE)
		{
			switch(usage)
			{
				case Usage.IMMEDIATE:
					FireEventImmediate(e);
					break;
				case Usage.AFTER_TICK:
					tickEvents.Add(e);
					break;
			}
		}

		public virtual void Tick(float deltaTime)
		{
			if(tickEvents.Count > 0)
			{
				foreach(var t in tickEvents)
				{
					FireEventImmediate(t);
				}
				tickEvents.Clear();
			}
		}

		public virtual void SubscribeEvent(int id, Action<Event> handler)
		{
			List<Action<Event>> subscription = null;
			if (!subscriptions.TryGetValue(id, out subscription))
			{
				subscription = new List<Action<Event>>();
				subscriptions.Add(id, subscription);
			}

			if (subscription.Contains(handler))
			{
				Debug.LogErrorFormat("resubscribe, id = {0}, handle = {1}", id, handler);
				return;
			}

			subscription.Add(handler);
		}

		public virtual void UnsubscribeEvent(int id, Action<Event> handler)
		{
			List<Action<Event>> subscription = null;
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

		private void FireEventImmediate(Event e)
		{
			List<Action<Event>> subscription = null;
			if(subscriptions.TryGetValue(e.id, out subscription))
			{
				foreach (var action in subscription)
				{
					action(e);
				}
			}
		}
	}
}