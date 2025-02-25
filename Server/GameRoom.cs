using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	class GameRoom : IJobQueue
	{
		private List<ClientSession> _sessions = new List<ClientSession>();
		private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
		private JobQueue _jobQueue = new JobQueue();

		public void Push(Action job)
		{
			_jobQueue.Push(job);
		}

		public void Flush()
		{
			foreach(ClientSession s in _sessions)
			{
				s.Send(_pendingList);
			}

			//Console.WriteLine($"Flushed {_pendingList.Count} items");
			_pendingList.Clear();
		}

		public void Broadcast(ArraySegment<byte> segment)
		{
			_pendingList.Add(segment);
		}

		public void Enter(ClientSession session)
		{
			_sessions.Add(session);
			session.Room = this;

			ServerPlayerList players = new ServerPlayerList();

			foreach(ClientSession s in _sessions)
			{
				var player = new ServerPlayerList.Player();

				player.isSelf = (s == session);
				player.playerId = s.SessionId;
				player.posX = s.PosX;
				player.posY = s.PosY;
				player.posZ = s.PosZ;

				players.players.Add(player);
			}

			session.Send(players.Write());

			ServerBroadcastEnterGame enter = new ServerBroadcastEnterGame();
			enter.playerId = session.SessionId;
			enter.posX = 0;
			enter.posY = 0;
			enter.posZ = 0;
			Broadcast(enter.Write());
		}

		public void Leave(ClientSession session)
		{
			_sessions.Remove(session);

			ServerBroadcastLeaveGame leave = new ServerBroadcastLeaveGame();
			leave.playerId = session.SessionId;
			Broadcast(leave.Write());
		}

		public void Move(ClientSession session, ClientMove packet)
		{
			if(session.SessionId == 1)
			{
				session.PosX = packet.posX;
				session.PosY = packet.posY;
				session.PosZ = packet.posZ;
			}

			ServerBroadcastMove move = new ServerBroadcastMove();
			move.playerId = session.SessionId;
			move.posX = session.PosX;
			move.posY = session.PosY;
			move.posZ = session.PosZ;
			Broadcast(move.Write());
		}
	}
}
