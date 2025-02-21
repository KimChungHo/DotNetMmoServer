using System;
using System.Collections.Generic;
using System.Text;

namespace DummyClient
{
	class SessionManager
	{
		public static SessionManager Instance { get { return _session; } }

		static SessionManager _session = new SessionManager();

		List<ServerSession> _sessions = new List<ServerSession>();
		Random _rand = new Random();

		object _lock = new object();

		public void SendForEach()
		{
			lock (_lock)
			{
				foreach (ServerSession session in _sessions)
				{
					ClientMove movePacket = new ClientMove();
					movePacket.posX = _rand.Next(-50, 50);
					movePacket.posY = 0;
					movePacket.posZ = _rand.Next(-50, 50);
					session.Send(movePacket.Write());
				}
			}
		}

		public ServerSession Generate()
		{
			lock (_lock)
			{
				ServerSession session = new ServerSession();
				_sessions.Add(session);
				return session;
			}
		}
	}
}
