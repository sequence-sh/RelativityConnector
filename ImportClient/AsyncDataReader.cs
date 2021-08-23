using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq;
using Grpc.Core;
using ReductechRelativityImport;

namespace ImportClient
{

public sealed class AsyncDataReader : DbDataReader
{
    private readonly string[] _memberNames;
    private readonly Type[] _effectiveTypes;

    private readonly IAsyncStreamReader<ImportObject> _streamReader;

    public AsyncDataReader(
        string[] memberNames,
        Type[] effectiveTypes,
        IAsyncStreamReader<ImportObject> streamReader)
    {
        _memberNames    = memberNames.Select(x => x.ToLowerInvariant()).ToArray();
        _effectiveTypes = effectiveTypes;
        _streamReader   = streamReader;
    }

    public override int Depth => 0;

    public override void Close()
    {
        Shutdown();
    }

    public override bool HasRows => _active;

    private bool _active = true;

    public override bool NextResult()
    {
        _active = false;
        return false;
    }

    public override bool Read()
    {
        if (_active)
        {
            var tmp = _streamReader;

            if (tmp != null && tmp.MoveNext().Result)
            {
                return true;
            }

            _active = false;
        }

        return false;
    }

    /// <inheritdoc />
    public override DataTable GetSchemaTable()
    {
        return base.GetSchemaTable();
    }

    public override int RecordsAffected => 0;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            Shutdown();
    }

    private void Shutdown()
    {
        _active = false;
    }

    public override int FieldCount => _memberNames.Length;

    public override bool IsClosed => _streamReader == null;

    public override bool GetBoolean(int i)
    {
        return (bool)this[i];
    }

    public override byte GetByte(int i)
    {
        return (byte)this[i];
    }

    public override long GetBytes(
        int i,
        long fieldOffset,
        byte[] buffer,
        int bufferoffset,
        int length)
    {
        var s         = (byte[])this[i];
        var    available = s.Length - (int)fieldOffset;

        if (available <= 0)
            return 0;

        var count = Math.Min(length, available);
        Buffer.BlockCopy(s, (int)fieldOffset, buffer, bufferoffset, count);
        return count;
    }

    public override char GetChar(int i)
    {
        return (char)this[i];
    }

    public override long GetChars(
        int i,
        long fieldoffset,
        char[] buffer,
        int bufferoffset,
        int length)
    {
        var s         = (string)this[i];
        var    available = s.Length - (int)fieldoffset;

        if (available <= 0)
            return 0;

        var count = Math.Min(length, available);
        s.CopyTo((int)fieldoffset, buffer, bufferoffset, count);
        return count;
    }

    protected override DbDataReader GetDbDataReader(int i)
    {
        throw new NotSupportedException();
    }

    public override string GetDataTypeName(int i)
    {
        return (_effectiveTypes == null ? typeof(object) : _effectiveTypes[i]).Name;
    }

    public override DateTime GetDateTime(int i)
    {
        return (DateTime)this[i];
    }

    public override decimal GetDecimal(int i)
    {
        return (decimal)this[i];
    }

    public override double GetDouble(int i)
    {
        return (double)this[i];
    }

    public override Type GetFieldType(int i)
    {
        return _effectiveTypes == null ? typeof(object) : _effectiveTypes[i];
    }

    public override float GetFloat(int i)
    {
        return (float)this[i];
    }

    public override Guid GetGuid(int i)
    {
        return (Guid)this[i];
    }

    public override short GetInt16(int i)
    {
        return (short)this[i];
    }

    public override int GetInt32(int i)
    {
        return (int)this[i];
    }

    public override long GetInt64(int i)
    {
        return (long)this[i];
    }

    public override string GetName(int i)
    {
        return _memberNames[i];
    }

    public override int GetOrdinal(string name)
    {
        var r = Array.IndexOf(_memberNames, name.ToLowerInvariant());

        return r;
    }

    public override string GetString(int i)
    {
        return (string)this[i];
    }

    public override object GetValue(int i)
    {
        return this[i];
    }

    public override IEnumerator GetEnumerator() => new DbEnumerator(this);

    public override int GetValues(object[] values)
    {
        // duplicate the key fields on the stack
        var members = _memberNames;
        var current = _streamReader.Current;

        var count = Math.Min(values.Length, members.Length);

        for (var i = 0; i < count; i++)
            values[i] = current.Values[i].GetValue() ?? DBNull.Value;

        return count;
    }

    public override bool IsDBNull(int i)
    {
        var r = this[i] is DBNull;
        return r;
    }

    public override object this[string name]
    {
        get
        {
            var r = GetOrdinal(name);

            if (r < 0)
                return DBNull.Value;

            return this[r];
        }
    }
        

    /// <summary>
    /// Gets the value of the current object in the member specified
    /// </summary>
    public override object this[int i] => _streamReader.Current.Values[i].GetValue() ?? DBNull.Value;
}

}
