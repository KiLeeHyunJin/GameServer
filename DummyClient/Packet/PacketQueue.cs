using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class PacketQueue
{
    public static PacketQueue Instance { get; } = new();

    Queue<IPacket> _packetQueue = new();
    object _lock = new();

    public void Push(IPacket packet)
    {
        lock (_lock)
        {
            _packetQueue.Enqueue(packet);
        }
    }

    public IPacket Pop()
    {
        lock (_lock)
        {
            if (_packetQueue.Count == 0)
            {
                return null;
            }
            return _packetQueue.Dequeue();
        }
    }
}