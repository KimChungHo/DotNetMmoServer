using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
	public static PacketManager Instance
	{
		get
		{
			return _instance;
		}
	}

	private static PacketManager _instance = new PacketManager();

	private PacketManager()
	{
		Register();
	}

	private Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	private Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

	public void Register()
	{
		_makeFunc.Add((ushort)PacketID.ServerBroadcastEnterGame, MakePacket<ServerBroadcastEnterGame>);
		_handler.Add((ushort)PacketID.ServerBroadcastEnterGame, PacketHandler.ServerBroadcastEnterGameHandler);
		_makeFunc.Add((ushort)PacketID.ServerBroadcastLeaveGame, MakePacket<ServerBroadcastLeaveGame>);
		_handler.Add((ushort)PacketID.ServerBroadcastLeaveGame, PacketHandler.ServerBroadcastLeaveGameHandler);
		_makeFunc.Add((ushort)PacketID.ServerPlayerList, MakePacket<ServerPlayerList>);
		_handler.Add((ushort)PacketID.ServerPlayerList, PacketHandler.ServerPlayerListHandler);
		_makeFunc.Add((ushort)PacketID.ServerBroadcastMove, MakePacket<ServerBroadcastMove>);
		_handler.Add((ushort)PacketID.ServerBroadcastMove, PacketHandler.ServerBroadcastMoveHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Func<PacketSession, ArraySegment<byte>, IPacket> func = null;

		if(_makeFunc.TryGetValue(id, out func))
		{
			IPacket packet = func.Invoke(session, buffer);

			if(onRecvCallback != null)
			{
				onRecvCallback.Invoke(session, packet);
			}
			else
			{
				HandlePacket(session, packet);
			}
		}
	}

	private T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		T pakcet = new T();
		pakcet.Read(buffer);

		return pakcet;
	}

	public void HandlePacket(PacketSession session, IPacket packet)
	{
		Action<PacketSession, IPacket> action = null;

		if(_handler.TryGetValue(packet.Protocol, out action))
		{
			action.Invoke(session, packet);
		}
	}
}