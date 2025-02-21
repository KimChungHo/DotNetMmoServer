using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
	public static void ClientLeaveGameHandler(PacketSession session, IPacket packet)
	{
		ClientSession clientSession = session as ClientSession;

		if (clientSession.Room == null)
			return;

		GameRoom room = clientSession.Room;
		room.Push(() => room.Leave(clientSession));
	}

	public static void ClientMoveHandler(PacketSession session, IPacket packet)
	{
		ClientMove movePacket = packet as ClientMove;
		ClientSession clientSession = session as ClientSession;

		if (clientSession.Room == null)
			return;

		GameRoom room = clientSession.Room;
		room.Push(() => room.Move(clientSession, movePacket));
	}
}
