using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  HostServerConnectionPool.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite
{

	/// <summary>
	/// Used to pool HostServerConnections of a specific type to a specific system.
	/// 
	/// For example:
	/// <pre>
	///   // Get the SystemInfo object used to seed our pool.
	///   SignonConnection signon = SignonConnection.getConnection("system", "user", "password");
	///   SystemInfo info = signon.getInfo();
	///   signon.close();
	/// 
	///   // Construct the pool (initially empty).
	///   HostServerConnectionPool&lt;CommandConnection&gt; commandPool = new HostServerConnectionPool&lt;CommandConnection&gt;(info);
	/// 
	///   // To populate the pool, create connections and check them in.
	///   // To use the pool, check out connections from the pool.
	///   CommandConnection conn = CommandConnection.getConnection(info, "FRED", "password");
	///   commandPool.checkin(conn);
	///   conn = commandPool.checkout("FRED");
	///   commandPool.checkin(conn);
	/// 
	///   // You can check the current size of the pool.
	///   int used = commandPool.getUsedConnectionCount();
	///   int free = commandPool.getFreeConnectionCount();
	/// 
	///   // You can check the number of connections per user.
	///   int fredUsed = commandPool.getUsedConnectionCount("FRED");
	///   int fredFree = commandPool.getFreeConnectionCount("FRED");
	/// 
	///   // Closing the pool will close all connections in the pool, both free and in use.
	///   commandPool.close();
	/// </pre>
	/// 
	/// </summary>
	public class HostServerConnectionPool<T> where T : HostServerConnection
	{
	  private readonly SystemInfo info_;
	  private readonly IDictionary<string, ISet<T>> freeConnections_ = new Dictionary<string, ISet<T>>();
	  private readonly ISet<T> usedConnections_ = new HashSet<T>();

	  private int freeConnectionCount_;
	  private int usedConnectionCount_;

	  /// <summary>
	  /// Constructs a new connection pool for the specified system.
	  /// All connections checked into this pool must have a matching SystemInfo object.
	  /// 
	  /// </summary>
	  public HostServerConnectionPool(SystemInfo info)
	  {
		info_ = info;
	  }

	  /// <summary>
	  /// Returns the system information for this pool.
	  /// 
	  /// </summary>
	  public virtual SystemInfo Info
	  {
		  get
		  {
			return info_;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws Throwable
	  ~HostServerConnectionPool()
	  {
		close();
	  }

	  /// <summary>
	  /// Adds or returns a connection to this pool.
	  /// If the connection is closed or its SystemInfo does not match what was defined for this pool,
	  /// the connection is removed from this pool if it already exists in this pool, but is otherwise ignored.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkin(final T conn) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void checkin(T conn)
	  {
		if (!conn.Closed && conn.Info.Equals(info_))
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String user = conn.getUser();
		  string user = conn.User;

		  ISet<object> systems = freeConnections_[user];
		  if (systems == null)
		  {
			systems = new HashSet<T>();
			freeConnections_[user] = systems;
		  }
		  systems.Add(conn);
		  ++freeConnectionCount_;
		  if (usedConnections_.remove(conn))
		  {
			--usedConnectionCount_;
		  }
		}
		else
		{
		  remove(conn);
		}
	  }

	  /// <summary>
	  /// Obtains a free connection from this pool for the specified user.
	  /// If there are no free connections in the pool for the specified user, null is returned.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T checkout(String user) throws IOException
	  public virtual T checkout(string user)
	  {
		ISet<T> systems = freeConnections_[user];
		if (systems != null)
		{
		  IEnumerator<T> it = systems.GetEnumerator();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  if (it.hasNext())
		  {
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			T conn = it.next();
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			it.remove();
			--freeConnectionCount_;
			usedConnections_.Add(conn);
			++usedConnectionCount_;
			return conn;
		  }
		}
		return default(T);
	  }

	  /// <summary>
	  /// Removes the specified connection from this pool, regardless if it is free or in use.
	  /// If the connection is not in the pool, it is ignored.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void remove(final T conn)
	  public virtual void remove(T conn)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String user = conn.getUser();
		string user = conn.User;
		ISet<object> systems = freeConnections_[user];
		if (systems != null)
		{
		  if (systems.remove(conn))
		  {
			--freeConnectionCount_;
		  }
		}
		if (usedConnections_.remove(conn))
		{
		  --usedConnectionCount_;
		}
	  }

	  /// <summary>
	  /// Closes and removes all connections in this pool, both free and in use.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws IOException
	  public virtual void close()
	  {
		closeFree();
		closeUsed();
	  }

	  /// <summary>
	  /// Closes and removes all free connections in this pool.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void closeFree() throws IOException
	  public virtual void closeFree()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Iterator<String> it = freeConnections_.keySet().iterator();
		IEnumerator<string> it = freeConnections_.Keys.GetEnumerator();
		while (it.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String user = it.Current;
		  string user = it.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Set<T> systems = freeConnections_.get(user);
		  ISet<T> systems = freeConnections_[user];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Iterator<T> it2 = systems.iterator();
		  IEnumerator<T> it2 = systems.GetEnumerator();
		  while (it2.MoveNext())
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T conn = it2.Current;
			T conn = it2.Current;
			try
			{
			  conn.close();
			}
			catch (IOException)
			{
			}
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			it2.remove();
			--freeConnectionCount_;
		  }
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
		  it.remove();
		}
	  }

	  /// <summary>
	  /// Closes and removes all in-use connections in this pool.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void closeUsed() throws IOException
	  public virtual void closeUsed()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Iterator<T> it = usedConnections_.iterator();
		IEnumerator<T> it = usedConnections_.GetEnumerator();
		while (it.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T conn = it.Current;
		  T conn = it.Current;
		  try
		  {
			conn.close();
		  }
		  catch (IOException)
		  {
		  }
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
		  it.remove();
		  --usedConnectionCount_;
		}
	  }

	  /// <summary>
	  /// Closes and removes all connections for the specified user in this pool, both free and in use.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close(final String user) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void close(string user)
	  {
		closeFree(user);
		closeUsed(user);
	  }

	  /// <summary>
	  /// Closes and removes all free connections for the specified user in this pool.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void closeFree(final String user) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void closeFree(string user)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Set<T> systems = freeConnections_.remove(user);
		ISet<T> systems = freeConnections_.Remove(user);
		if (systems != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Iterator<T> it = systems.iterator();
		  IEnumerator<T> it = systems.GetEnumerator();
		  while (it.MoveNext())
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T conn = it.Current;
			T conn = it.Current;
			try
			{
			  conn.close();
			}
			catch (IOException)
			{
			}
			--freeConnectionCount_;
		  }
		}
	  }

	  /// <summary>
	  /// Closes and removes all in-use connections for the specified user in this pool.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void closeUsed(final String user) throws IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void closeUsed(string user)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Iterator<T> it = usedConnections_.iterator();
		IEnumerator<T> it = usedConnections_.GetEnumerator();
		while (it.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T conn = it.Current;
		  T conn = it.Current;
		  if (conn.User.Equals(user))
		  {
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			it.remove();
			try
			{
			  conn.close();
			}
			catch (IOException)
			{
			}
			--usedConnectionCount_;
		  }
		}
	  }

	  /// <summary>
	  /// Returns the total number of connections in this pool, both free and in use.
	  /// 
	  /// </summary>
	  public virtual int ConnectionCount
	  {
		  get
		  {
			return freeConnectionCount_ + usedConnectionCount_;
		  }
	  }

	  /// <summary>
	  /// Returns the number of free connections in this pool.
	  /// 
	  /// </summary>
	  public virtual int FreeConnectionCount
	  {
		  get
		  {
			return freeConnectionCount_;
		  }
	  }

	  /// <summary>
	  /// Returns the number of in-use connections in this pool.
	  /// 
	  /// </summary>
	  public virtual int UsedConnectionCount
	  {
		  get
		  {
			return usedConnectionCount_;
		  }
	  }

	  /// <summary>
	  /// Returns the total number of connections for the specified user in this pool, both free and in use.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getConnectionCount(final String user)
	  public virtual int getConnectionCount(string user)
	  {
		return getFreeConnectionCount(user) + getUsedConnectionCount(user);
	  }

	  /// <summary>
	  /// Returns the number of free connections for the specified user in this pool.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getFreeConnectionCount(final String user)
	  public virtual int getFreeConnectionCount(string user)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Set systems = freeConnections_.get(user);
		ISet<object> systems = freeConnections_[user];
		return systems == null ? 0 : systems.Count;
	  }

	  /// <summary>
	  /// Returns the number of used connections for the specified user in this pool.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getUsedConnectionCount(final String user)
	  public virtual int getUsedConnectionCount(string user)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Iterator<T> it = usedConnections_.iterator();
		IEnumerator<T> it = usedConnections_.GetEnumerator();
		int count = 0;
		while (it.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T conn = it.Current;
		  T conn = it.Current;
		  if (conn.User.Equals(user))
		  {
			  ++count;
		  }
		}
		return count;
	  }

	  /// <summary>
	  /// Returns an array of users of connections in this pool, both free and in use.
	  /// 
	  /// </summary>
	  public virtual string[] Users
	  {
		  get
		  {
			ISet<string> set = freeConnections_.Keys;
			return set.toArray(new string[set.Count]);
		  }
	  }
	}

}