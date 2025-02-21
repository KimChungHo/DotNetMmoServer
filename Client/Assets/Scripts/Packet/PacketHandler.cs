using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class PacketHandler
{
	public static void ServerBroadcastEnterGameHandler(PacketSession session, IPacket packet)
	{
		ServerBroadcastEnterGame pkt = packet as ServerBroadcastEnterGame;
		ServerSession serverSession = session as ServerSession;

		PlayerManager.Instance.EnterGame(pkt);
	}

	public static void ServerBroadcastLeaveGameHandler(PacketSession session, IPacket packet)
	{
		ServerBroadcastLeaveGame pkt = packet as ServerBroadcastLeaveGame;
		ServerSession serverSession = session as ServerSession;

		PlayerManager.Instance.LeaveGame(pkt);
	}

	public static void ServerPlayerListHandler(PacketSession session, IPacket packet)
	{
		ServerPlayerList pkt = packet as ServerPlayerList;
		ServerSession serverSession = session as ServerSession;

		PlayerManager.Instance.Add(pkt);
	}

	public static void ServerBroadcastMoveHandler(PacketSession session, IPacket packet)
	{
		ServerBroadcastMove pkt = packet as ServerBroadcastMove;
		ServerSession serverSession = session as ServerSession;

		PlayerManager.Instance.Move(pkt);
	}
}
