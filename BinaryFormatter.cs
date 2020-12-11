using System;
using System.IO;


namespace Platform.Distribution
{
    //二进制序列化
    public class BinaryFormatter
    {
        //序列化
        public void Serialize(Stream stream, object value)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            WriteObject(writer, value, true, true);
        }
        protected void WriteObject(BinaryWriter writer, object value, bool writeType, bool writeValue)
        {
            switch(value)
            {
                //基础类型
                case null:
                    if(writeType)
                        writer.Write((byte)1);
                    break;
                case DBNull:
                    if(writeType)
                        writer.Write((byte)2);
                    break;
                //布尔型
                case Boolean temp:
                    if(writeType)
                        writer.Write((byte)3);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                //数字型
                case Byte temp:
                    if(writeType)
                        writer.Write((byte)11);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                case SByte temp:
                    if(writeType)
                        writer.Write((byte)12);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                case Int16 temp:
                    if(writeType)
                        writer.Write((byte)13);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                case UInt16 temp:
                    if(writeType)
                        writer.Write((byte)14);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                case Int32 temp:
                    if(writeType)
                        writer.Write((byte)15);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                case UInt32 temp:
                    if(writeType)
                        writer.Write((byte)16);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                case Int64 temp:
                    if(writeType)
                        writer.Write((byte)17);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                case UInt64 temp:
                    if(writeType)
                        writer.Write((byte)18);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                //小数型
                case Single temp:
                    if(writeType)
                        writer.Write((byte)20);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                case Double temp:
                    if(writeType)
                        writer.Write((byte)21);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                case Decimal temp:
                    if(writeType)
                        writer.Write((byte)22);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                //字符型
                case Char temp:
                    if(writeType)
                        writer.Write((byte)30);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                case String temp:
                    if(writeType)
                        writer.Write((byte)31);
                    if(writeValue)
                        writer.Write(temp);
                    break;
                //时间型
                case DateTime temp:
                    if(writeType)
                        writer.Write((byte)40);
                    if(writeValue)
                        writer.Write(temp.Ticks);
                    break;
                case TimeSpan temp:
                    if(writeType)
                        writer.Write((byte)41);
                    if(writeValue)
                        writer.Write(temp.Ticks);
                    break;
                //枚举 string,int
                case Enum temp:
                    if(writeType)
                    {
                        writer.Write((byte)42);
                        writer.Write(value.GetType().FullName);
                    }
                    if(writeValue)
                        writer.Write(Convert.ToInt32(temp));
                    break;
                //数组 type,length,[type?,value...]
                case Array temp:
                    Type type = value.GetType().GetElementType();
                    if(writeType)
                    {
                        writer.Write((byte)50);
                        WriteObject(writer, Activator.CreateInstance(type), true, false);
                    }
                    if(writeValue)
                    {
                        writer.Write(temp.Length);
                        for(int i = 0; i < temp.Length; i++)
                        {
                            WriteObject(writer, temp.GetValue(i), !type.IsSealed, true);
                        }
                    }
                    break;
                //对象
                case Object temp:
                    if(value.GetType() == typeof(object))
                    {
                        if(writeType)
                            writer.Write((byte)255);
                    }
                    else
                    {
                        throw new Exception($"not support WriteObject {value}");
                    }
                    break;
            }
        }
        

        //反序列化
        public object Deserialize(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            return ReadObject(reader, false, null, 0);
        } 
        protected object ReadObject(BinaryReader reader, bool onlyReadType, Type type, byte code)
        {
            if(code == 0)
                code = reader.ReadByte();
            switch(code)
            {
                //基础类型
                case 1:
                    if(onlyReadType)
                        return ((Type)null, code);
                    else
                        return null;
                case 2:
                    if(onlyReadType)
                        return (typeof(DBNull), code);
                    else
                        return DBNull.Value;
                //布尔型
                case 3:
                    if(onlyReadType)
                        return (typeof(Boolean), code);
                    else
                        return reader.ReadBoolean();
                //数字型
                case 11:
                    if(onlyReadType)
                        return (typeof(Byte), code);
                    else
                        return reader.ReadByte();
                case 12:
                    if(onlyReadType)
                        return (typeof(SByte), code);
                    else
                        return reader.ReadSByte();
                case 13:
                    if(onlyReadType)
                        return (typeof(Int16), code);
                    else
                        return reader.ReadInt16();
                case 14:
                    if(onlyReadType)
                        return (typeof(UInt16), code);
                    else
                        return reader.ReadUInt16();
                case 15:
                    if(onlyReadType)
                        return (typeof(Int32), code);
                    else
                        return reader.ReadInt32();
                case 16:
                    if(onlyReadType)
                        return (typeof(UInt32), code);
                    else
                        return reader.ReadUInt32();
                case 17:
                    if(onlyReadType)
                        return (typeof(Int64), code);
                    else
                        return reader.ReadInt64();
                case 18:
                    if(onlyReadType)
                        return (typeof(UInt64), code);
                    else
                        return reader.ReadUInt64();
                //小数型
                case 20:
                    if(onlyReadType)
                        return (typeof(Single), code);
                    else
                        return reader.ReadSingle();
                case 21:
                    if(onlyReadType)
                        return (typeof(Double), code);
                    else
                        return reader.ReadDouble();
                case 22:
                    if(onlyReadType)
                        return (typeof(Decimal), code);
                    else
                        return reader.ReadDecimal();
                //字符型
                case 30:
                    if(onlyReadType)
                        return (typeof(Char), code);
                    else
                        return reader.ReadChar();
                case 31:
                    if(onlyReadType)
                        return (typeof(String), code);
                    else
                        return reader.ReadString();
                //时间型
                case 40:
                    if(onlyReadType)
                        return (typeof(DateTime), code);
                    else
                        return new DateTime(reader.ReadInt64());
                case 41:
                    if(onlyReadType)
                        return (typeof(TimeSpan), code);
                    else
                        return new TimeSpan(reader.ReadInt64());
                //枚举 string,int
                case 42:
                    if(onlyReadType)
                        return (Type.GetType(reader.ReadString()), code);
                    else
                        return Enum.ToObject(type, reader.ReadInt32());
                //数组 type,length,[type?,value...]
                case 50:
                    (Type type2, byte code2) = ((Type, byte))ReadObject(reader, true, null, 0);
                    if(onlyReadType)
                    {
                        return (type2.MakeArrayType(), code);
                    }
                    else
                    {
                        Array array = Array.CreateInstance(type2, reader.ReadInt32());
                        for(int i = 0; i < array.Length; i++)
                        {
                            array.SetValue(ReadObject(reader, false, type2, type2.IsSealed ? code2 : 0), i);
                        }
                        return array;
                    }
                //对象
                case 255:
                    if(onlyReadType)
                        return (typeof(object), code);
                    else
                        return new object();
                //其余
                default:
                    throw new Exception($"not support ReadValue {code}");
            }
        } 
       
    }
}