﻿using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	class GameRoom : IJobQueue
	{
		List<ClientSession> _sessions = new List<ClientSession>();
		JobQueue _jobQueue = new JobQueue();
		List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

		public void Push(Action job)
		{
			_jobQueue.Push(job);
		}

		public void Flush()
		{
			foreach (ClientSession s in _sessions)
				s.Send(_pendingList);
			//Console.WriteLine($"Flushed {_pendingList.Count} items");
			_pendingList.Clear();
		}

		public void Broadcast(ArraySegment<byte> segment)
		{
			_pendingList.Add(segment);			
		}

		public void Enter(ClientSession session)
		{
			// 플레이어 추가하고
			_sessions.Add(session);
			session.Room = this;

			// 신입생한테 모든 플레이어 목록 전송
			ServerPlayerList players = new ServerPlayerList();
			foreach (ClientSession s in _sessions)
			{
				players.players.Add(new ServerPlayerList.Player()
				{
					isSelf = (s == session),
					playerId = s.SessionId,
					posX = s.PosX,
					posY = s.PosY,
					posZ = s.PosZ,
				});
			}
			session.Send(players.Write());

			// 신입생 입장을 모두에게 알린다
			ServerBroadcastEnterGame enter = new ServerBroadcastEnterGame();
			enter.playerId = session.SessionId;
			enter.posX = 0;
			enter.posY = 0;
			enter.posZ = 0;
			Broadcast(enter.Write());
		}

		public void Leave(ClientSession session)
		{
			// 플레이어 제거하고
			_sessions.Remove(session);

			// 모두에게 알린다
			ServerBroadcastLeaveGame leave = new ServerBroadcastLeaveGame();
			leave.playerId = session.SessionId;
			Broadcast(leave.Write());
		}

		public void Move(ClientSession session, ClientMove packet)
		{
			// 좌표 바꿔주고
			session.PosX = packet.posX;
			session.PosY = packet.posY;
			session.PosZ = packet.posZ;

			// 모두에게 알린다
			ServerBroadcastMove move = new ServerBroadcastMove();
			move.playerId = session.SessionId;
			move.posX = session.PosX;
			move.posY = session.PosY;
			move.posZ = session.PosZ;
			Broadcast(move.Write());
		}
	}
}
