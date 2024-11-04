
using ServerCore;
using ServerCore.Base;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public enum PacketID
{
    PlayerInfoReq = 1,
	Test = 2,
	
}
public class PlayerInfoReq
{
    public long playerId;
	public string name;
	public byte state;
	
	#region Skill
	public struct Skill
	{
	    public long id;
		public short level;
		public float  duration;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        this.id = BitConverter.ToInt64(s.Slice(count, s.Length - count));
			count += sizeof(long);
			
			this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
			count += sizeof(short);
			
			this. duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
			count += sizeof(long);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
			count += sizeof(short);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this. duration);
			count += sizeof(float);
			
	        return success;
	    }
	}
	#endregion Skill
	public List<Skill> skills = new List<Skill>();
	


    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		
		ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += nameLen;
		
		this.state = (byte)segment.Array[segment.Offset + count];
		count += sizeof(byte);
		
		
		this.skills.Clear();
		ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		for (int i = 0; i < skillLen; i++)
		{
		    Skill skill = new Skill();
		    skill.Read(s, ref count);
		    skills.Add(skill);
		}
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(long);
		
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
		count += sizeof(ushort);
		count += nameLen;
		
		 segment.Array[segment.Offset + count] = (byte)this.state;
		count += sizeof(byte);
		
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
		count += sizeof(ushort);
		
		foreach (Skill skill in skills)
		{
		    success &= skill.Write(s, ref count);
		}
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}
public class Test
{
    public long playerId;
	public string name;
	
	#region Skill
	public struct Skill
	{
	    public int id;
		public short level;
		public float  duration;
	
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			
			this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
			count += sizeof(short);
			
			this. duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			
	    }
	
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool success = true;
	        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
			count += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
			count += sizeof(short);
			
			success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this. duration);
			count += sizeof(float);
			
	        return success;
	    }
	}
	#endregion Skill
	public List<Skill> skills = new List<Skill>();
	


    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 4;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        
        this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
		count += sizeof(long);
		
		ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		
		this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
		count += nameLen;
		
		
		this.skills.Clear();
		ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
		count += sizeof(ushort);
		for (int i = 0; i < skillLen; i++)
		{
		    Skill skill = new Skill();
		    skill.Read(s, ref count);
		    skills.Add(skill);
		}
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.Test);
        count += sizeof(ushort);

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(long);
		
		ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
		count += sizeof(ushort);
		count += nameLen;
		
		
		success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
		count += sizeof(ushort);
		
		foreach (Skill skill in skills)
		{
		    success &= skill.Write(s, ref count);
		}
		

        success &= BitConverter.TryWriteBytes(s, count);
        return success ? SendBufferHelper.Close(count) : null;
    }
}

